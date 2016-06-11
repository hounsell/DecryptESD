using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace DecryptESD.IO
{
   public class WimFile : IDisposable
   {
      private readonly FileStream _file;
      private readonly BinaryReader _reader;
      private readonly BinaryWriter _writer;

      public int BuildNumber => int.Parse(XmlMetadata.Descendants("BUILD").LastOrDefault()?.Value ?? "0");

      public WimHeader Header { get; private set; }
      public IntegrityTable IntegrityTable { get; private set; }
      public XDocument XmlMetadata { get; private set; }

      public WimFile(string path)
      {
         _file = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
         _reader = new BinaryReader(_file, Encoding.Default, true);
         _writer = new BinaryWriter(_file, Encoding.Default, true);

         ReadWimHeader();
         ReadXmlMetadata();
         ReadIntegrityTable();
      }

      public void Dispose()
      {
         _writer.Flush();

         _reader.Dispose();
         _writer.Dispose();
         _file.Dispose();
      }

      private unsafe void ReadWimHeader()
      {
         _file.Position = 0;

         byte[] bHeader = _reader.ReadBytes(sizeof(WimHeader));
         fixed (byte* pHeader = bHeader)
         {
            Header = Marshal.PtrToStructure<WimHeader>((IntPtr) pHeader);
         }
      }

      private void ReadXmlMetadata()
      {
         _file.Position = Header.XmlData.OffsetInWim;

         byte[] bXml = _reader.ReadBytes((int) Header.XmlData.SizeInWim);
         using (MemoryStream mStr = new MemoryStream(bXml))
         {
            XmlMetadata = XDocument.Load(mStr);
         }
      }

      private unsafe void ReadIntegrityTable()
      {
         _file.Position = Header.IntegrityTable.OffsetInWim;

         byte[] bTable = _reader.ReadBytes(sizeof(IntegrityTableHeader));
         IntegrityTableHeader itHeader;
         fixed (byte* pTable = bTable)
         {
            itHeader = Marshal.PtrToStructure<IntegrityTableHeader>((IntPtr) pTable);
         }

         byte[][] bbHashes = new byte[itHeader.TableRowCount][];
         for (int i = 0; i < bbHashes.Length; i++)
         {
            bbHashes[i] = _reader.ReadBytes(20);
         }

         IntegrityTable = new IntegrityTable(itHeader, bbHashes);
      }

      public unsafe void DecryptEsd()
      {
         XElement xeEsdData = XmlMetadata.Root?.Element("ESD");
         if (xeEsdData == null)
         {
            throw new Exception("The provided file is not an ESD");
         }

         string b64AesKey = xeEsdData.Element("KEY")?.Value;
         if (string.IsNullOrEmpty(b64AesKey))
         {
            throw new Exception("The file key is missing from the metadata");
         }

         byte[] bAesKey = Convert.FromBase64String(b64AesKey);

         using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
         {
            rsa.ImportCspBlob(CryptoKey.Keys.Last(k => k.FirstBuild <= BuildNumber).Key);
            bAesKey = rsa.Decrypt(bAesKey, true);
         }

         foreach (XElement range in xeEsdData.Descendants("RANGE"))
         {
            int rangeLength = int.Parse(range.Attribute("Bytes").Value);
            long rangeOffset = long.Parse(range.Attribute("Offset").Value);

            _file.Position = rangeOffset;
            int bEncryptedSize = (rangeLength / 16 + 1) * 16;
            byte[] bEncrypted = _reader.ReadBytes(bEncryptedSize);

            using (
               RijndaelManaged aesCipher = new RijndaelManaged
               {
                  Key = bAesKey,
                  IV = Convert.FromBase64String("AAAAAAAAAAAAAAAAAAAAAA=="),
                  Mode = CipherMode.CBC,
                  Padding = PaddingMode.None
               })
            using (MemoryStream msEncrypted = new MemoryStream(bEncrypted))
            using (
               CryptoStream strCrypto = new CryptoStream(msEncrypted, aesCipher.CreateDecryptor(), CryptoStreamMode.Read)
               )
            {
               byte[] bDecrypted = new byte[bEncryptedSize - 16];
               strCrypto.Read(bDecrypted, 0, bDecrypted.Length);

               _file.Position = rangeOffset;
               _file.Write(bDecrypted, 0, bDecrypted.Length);
               _file.Flush();
            }
         }

         xeEsdData.Remove();

         byte[] bXml;
         using (MemoryStream msXml = new MemoryStream())
         {
            using (
               XmlWriter xw = XmlWriter.Create(msXml,
                  new XmlWriterSettings { OmitXmlDeclaration = true, Encoding = Encoding.Unicode, Indent = true }))
            {
               XmlMetadata.Save(xw);
            }

            long xmlSize = msXml.Length;

            long fileSize = xmlSize + Header.XmlData.OffsetInWim + (long)Header.IntegrityTable.SizeInWim;
            XmlMetadata.Root?.Element("TOTALBYTES")?.SetValue(fileSize.ToString());

            msXml.Position = 0;
            msXml.SetLength(0);
            using (
               XmlWriter xw = XmlWriter.Create(msXml,
                  new XmlWriterSettings { OmitXmlDeclaration = true, Encoding = Encoding.Unicode, Indent = true }))
            {
               XmlMetadata.Save(xw);
            }

            bXml = msXml.ToArray();
         }

         WimHeader whUpdated = Header;
         whUpdated.XmlData.OriginalSize = (ulong) bXml.LongLength;
         whUpdated.XmlData.SizeInWimWithFlags = (ulong) bXml.LongLength | ((ulong) whUpdated.XmlData.Flags << 56);

         _file.Position = whUpdated.XmlData.OffsetInWim;
         _file.SetLength(whUpdated.XmlData.OffsetInWim);
         _writer.Write(bXml);

         whUpdated.IntegrityTable.OffsetInWim = _file.Position;

         byte[] bIntegrityTableHeader = new byte[sizeof(IntegrityTableHeader)];
         IntegrityTableHeader ith = IntegrityTable.Header;
         Marshal.Copy((IntPtr) (&ith), bIntegrityTableHeader, 0, bIntegrityTableHeader.Length);
         _writer.Write(bIntegrityTableHeader);

         foreach (byte[] b in IntegrityTable.Hashes)
         {
            _writer.Write(b);
         }

         byte[] bWhUpdated = new byte[sizeof(WimHeader)];
         Marshal.Copy((IntPtr) (&whUpdated), bWhUpdated, 0, bWhUpdated.Length);
         _file.Position = 0;
         _writer.Write(bWhUpdated);
      }
   }
}