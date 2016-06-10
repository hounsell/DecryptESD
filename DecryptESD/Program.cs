using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace DecryptESD
{
   internal class Program
   {
      private static CryptoKey[] _keys;

      private static void Main(string[] args)
      {
         if (args.Length == 0)
         {
            Console.WriteLine("decryptesd.exe <path_to_file>");
            return;
         }

         LoadKeyXml();

         ReadWimFile(args[0]);
      }

      private static unsafe void ReadWimFile(string path)
      {
         using (FileStream fStr = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
         using (BinaryReader br = new BinaryReader(fStr))
         {
            WimHeader whHeader;
            byte[] bHeader = br.ReadBytes(sizeof(WimHeader));
            fixed (byte* pHeader = bHeader)
            {
               whHeader = Marshal.PtrToStructure<WimHeader>((IntPtr) pHeader);
            }

            fStr.Position = whHeader.XmlData.OffsetInWim;
            var bXml = br.ReadBytes((int) whHeader.XmlData.SizeInWim);

            XDocument xdWimData;
            using (MemoryStream mStr = new MemoryStream(bXml))
            {
               xdWimData = XDocument.Load(mStr);
            }

            XElement xeEsdData = xdWimData.Root?.Element("ESD");
            if (xeEsdData == null)
            {
               throw new Exception("The provided file is not an ESD");
            }

            byte[] bAesKey = Convert.FromBase64String(xeEsdData.Element("KEY")?.Value ?? "");

            int build = int.Parse(xdWimData.Descendants("BUILD").Last().Value);

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
               rsa.ImportCspBlob(_keys.Last(k => k.FirstBuild <= build).Key);
               var aeskey = rsa.Decrypt(bAesKey, true);

               foreach (XElement range in xdWimData.Descendants("RANGE"))
               {
                  int encByteSize = (int.Parse(range.Attribute("Bytes").Value) / 16 + 1) * 16;
                  var encBytes = new byte[encByteSize];

                  fStr.Position = long.Parse(range.Attribute("Offset").Value);
                  fStr.Read(encBytes, 0, encBytes.Length);

                  using (
                     RijndaelManaged rijndaelManaged = new RijndaelManaged
                     {
                        Key = aeskey,
                        IV = Convert.FromBase64String("AAAAAAAAAAAAAAAAAAAAAA=="),
                        Mode = CipherMode.CBC,
                        Padding = PaddingMode.None
                     })
                  using (MemoryStream memoryStream = new MemoryStream(encBytes))
                  using (
                     CryptoStream cryptoStream = new CryptoStream(memoryStream,
                        rijndaelManaged.CreateDecryptor(aeskey, Convert.FromBase64String("AAAAAAAAAAAAAAAAAAAAAA==")),
                        CryptoStreamMode.Read))
                  {
                     var decBytes = new byte[encByteSize - 16];
                     cryptoStream.Read(decBytes, 0, decBytes.Length);

                     fStr.Position = long.Parse(range.Attribute("Offset").Value);
                     fStr.Write(decBytes, 0, decBytes.Length);
                     fStr.Flush();
                  }
               }
            }
         }
      }

      private static void LoadKeyXml()
      {
         using (FileStream fStr = new FileStream("CryptoKeys.xml", FileMode.Open, FileAccess.Read, FileShare.Read))
         {
            XDocument xKeys = XDocument.Load(fStr);
            _keys = (from k in xKeys.Descendants("key")
                     orderby int.Parse(k.Attribute("build").Value)
                     select new CryptoKey(k.Attribute("build").Value, k.Attribute("value").Value)).ToArray();
         }
      }
   }
}