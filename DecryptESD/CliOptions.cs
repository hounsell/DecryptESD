using System;
using System.Diagnostics;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace DecryptESD
{
   public class CliOptions
   {
      [Option('k', "custom-key", HelpText = "A custom decryption key to use.")]
      public string CustomKey { get; set; }

      [OptionArray('f', "files", HelpText = "A list of ESD files to decrypt.", Required = true)]
      public string[] EsdFiles { get; set; }

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
   }
}