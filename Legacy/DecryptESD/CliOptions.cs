using System;
using System.Diagnostics;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace DecryptESD
{
   public class CliOptions
   {
      [VerbOption("decrypt", HelpText = "Decrypt one or more ESD files")]
      public DecryptOptions DecryptVerb { get; set; }

      [VerbOption("update", HelpText = "Update the list of known RSA keys used for decrypting ESD files")]
      public UpdateOptions UpdateVerb { get; set; }

      public CliOptions()
      {
         DecryptVerb = new DecryptOptions();
         UpdateVerb = new UpdateOptions();
      }

      [HelpOption]
      public string GetUsage()
      {
         Assembly ass = Assembly.GetExecutingAssembly();
         FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(ass.Location);

         HelpText ht = new HelpText
         {
            Heading = new HeadingInfo(fvi.FileDescription, ass.GetName().Version.ToString()),
            Copyright = new CopyrightInfo(fvi.CompanyName, DateTime.Today.Year),
            AddDashesToOption = true
         };
         ht.AddOptions(this);
         return ht;
      }

      [HelpVerbOption]
      public string GetUsage(string verb) => HelpText.AutoBuild(this, verb);
   }

   public class DecryptOptions
   {
      [Option('k', "custom-key", HelpText = "A custom decryption key to use.")]
      public string CustomKey { get; set; }

      [OptionArray('f', "files", HelpText = "A list of ESD files to decrypt.", Required = true)]
      public string[] EsdFiles { get; set; }
   }

   public class UpdateOptions
   {
      [Option('f', "force", HelpText = "Overwrite the XML file with the current CryptoKey database rather than merge.")]
      public bool ForceUpdate { get; set; }

      [Option('u', "custom-url", HelpText = "A custom url to read the feed from.")]
      public string CustomUrl { get; set; }
   }
}