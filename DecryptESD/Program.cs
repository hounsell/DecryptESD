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
      private static CryptoKey[] keys;

      private static unsafe void Main(string[] args)
      {
         if (args.Length == 0)
         {
            Console.WriteLine("decryptesd.exe <path_to_file>");
            return;
         }

         XDocument xmlKeys = XDocument.Load("CryptoKeys.xml");
         keys =
            (from k in xmlKeys.Descendants("key")
             orderby int.Parse(k.Attribute("build").Value)
             select new CryptoKey(k.Attribute("build").Value, k.Attribute("value").Value)).ToArray();

         using (FileStream fStr = new FileStream(args[0], FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
         {
            WimHeader header;

            var headerBytes = new byte[sizeof(WimHeader)];
            fStr.Read(headerBytes, 0, headerBytes.Length);
            fixed (byte* headerPtr = headerBytes)
            {
               header = Marshal.PtrToStructure<WimHeader>((IntPtr)headerPtr);
            }

            fStr.Position = header.XmlData.OffsetInWim;
            var xmlBytes = new byte[header.XmlData.SizeInWim];
            fStr.Read(xmlBytes, 0, xmlBytes.Length);

            XDocument xdoc;
            using (MemoryStream mStr = new MemoryStream(xmlBytes))
            {
               xdoc = XDocument.Load(mStr);
            }

            string aesKey = xdoc.Descendants("KEY")
                                .First()
                                .Value;

            int build = int.Parse(xdoc.Descendants("BUILD").Last().Value);

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
               rsa.ImportCspBlob(keys.First(k => k.FirstBuild >= build).Key);
               byte[] aeskey = rsa.Decrypt(Convert.FromBase64String(aesKey), true);

               foreach (var range in xdoc.Descendants("RANGE"))
               {
                  int encByteSize = ((int.Parse(range.Attribute("Bytes").Value) / 16) + 1) * 16;
                  byte[] encBytes = new byte[encByteSize];

                  fStr.Position = long.Parse(range.Attribute("Offset")
                                               .Value);
                  fStr.Read(encBytes, 0, encBytes.Length);

                  using (var rijndaelManaged = new RijndaelManaged() { Key = aeskey, IV = Convert.FromBase64String("AAAAAAAAAAAAAAAAAAAAAA=="), Mode = CipherMode.CBC, Padding = PaddingMode.None })
                  using (var memoryStream = new MemoryStream(encBytes))
                  using (var cryptoStream = new CryptoStream(memoryStream, rijndaelManaged.CreateDecryptor(aeskey, Convert.FromBase64String("AAAAAAAAAAAAAAAAAAAAAA==")), CryptoStreamMode.Read))
                  {
                     byte[] decBytes = new byte[encByteSize - 16];
                     cryptoStream.Read(decBytes, 0, decBytes.Length);

                     fStr.Position = long.Parse(range.Attribute("Offset").Value);
                     fStr.Write(decBytes, 0, decBytes.Length);
                     fStr.Flush();
                  }
               }
            }
         }
      }
   }
}