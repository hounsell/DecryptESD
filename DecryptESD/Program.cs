using System;
using System.IO;
using CommandLine;
using DecryptESD.IO;

namespace DecryptESD
{
   internal class Program
   {
      public static CliOptions Options;

      private static void Main(string[] args)
      {
         Options = new CliOptions();
         if (!Parser.Default.ParseArguments(args, Options)) return;

         if (string.IsNullOrEmpty(Options.CustomKey))
         {
            CryptoKey.LoadKeysFromXml();
         }
         else
         {
            try
            {
               CryptoKey.UseCustomKey(Options.CustomKey);
            }
            catch (FormatException)
            {
               Console.WriteLine("The key you have specified is not valid. Loading the keys from the XML file instead...");
               CryptoKey.LoadKeysFromXml();
            }
         }

         foreach (string file in Options.EsdFiles)
         {
            if (File.Exists(file))
            {
               try
               {
                  using (WimFile wf = new WimFile(file))
                  {
                     wf.DecryptEsd();
                  }
               }
               catch (NoValidCryptoKeyException)
               {
                  Console.WriteLine($"We could not find the correct CryptoKey for \"{file}\".");
               }
               catch (UnencryptedImageException)
               {
                  Console.WriteLine($"You are trying to decrypt \"{file}\", but it is already decrypted.");
               }
            }
            else
            {
               Console.WriteLine($"The file \"{file}\" does not exist.");
            }
         }
      }
   }
}