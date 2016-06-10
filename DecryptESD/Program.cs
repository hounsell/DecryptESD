using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Xml.Linq;
using DecryptESD.IO;

namespace DecryptESD
{
   internal class Program
   {
      private static void Main(string[] args)
      {
         if (args.Length == 0)
         {
            Console.WriteLine("decryptesd.exe <path_to_file>");
            return;
         }

         CryptoKey.LoadKeysFromXml();

         using (WimFile wf = new WimFile(args[0]))
         {
            wf.DecryptEsd();
         }
      }
   }
}