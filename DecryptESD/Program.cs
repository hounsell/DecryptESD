using System;
using System.IO;
using CommandLine;
using DecryptESD.IO;

namespace DecryptESD
{
   internal class Program
   {
      public static object Options;
      public static string OptionVerb;

      private static void Main(string[] args)
      {
         CliOptions baseOptions = new CliOptions();
         if (!Parser.Default.ParseArguments(args,
            baseOptions,
            (s, o) =>
            {
               OptionVerb = s;
               Options = o;
            }))
         {
            Environment.Exit(Parser.DefaultExitCodeFail);
         }


         switch (OptionVerb)
         {
            case "decrypt":
               {
                  DecryptEsd((DecryptOptions)Options);
                  break;
               }

            case "update":
               {
                  UpdateKeys((UpdateOptions)Options);
                  break;
               }
         }
      }

      private static void DecryptEsd(DecryptOptions options)
      {
         if (string.IsNullOrEmpty(options.CustomKey))
         {
            CryptoKey.LoadKeysFromXml();
         }
         else
         {
            try
            {
               CryptoKey.UseCustomKey(options.CustomKey);
            }
            catch (FormatException)
            {
               Console.WriteLine("The key you have specified is not valid. Loading the keys from the XML file instead...");
               CryptoKey.LoadKeysFromXml();
            }
         }

         foreach (string file in options.EsdFiles)
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

      private static void UpdateKeys(UpdateOptions options)
      {
         if (string.IsNullOrEmpty(options.CustomUrl))
         {
            if (options.ForceUpdate)
            {
               CryptoKey.ReplaceXmlWithWebFeed();
            }
            else
            {
               CryptoKey.MergeWebFeedIntoXml();
            }
         }
         else
         {
            if (options.ForceUpdate)
            {
               CryptoKey.ReplaceXmlWithWebFeed(options.CustomUrl);
            }
            else
            {
               CryptoKey.MergeWebFeedIntoXml(options.CustomUrl);
            }
         }
      }
   }
}