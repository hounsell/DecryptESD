using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DecryptESD
{
   public class CryptoKey
   {
      public static CryptoKey[] Keys { get; private set; }

      public int FirstBuild { get; }
      public byte[] Key { get; }

      public CryptoKey(string firstBuild, string key)
      {
         FirstBuild = int.Parse(firstBuild);
         Key = Convert.FromBase64String(key);
      }

      public static void LoadKeysFromXml()
      {
         using (FileStream fStr = new FileStream("CryptoKeys.xml", FileMode.Open, FileAccess.Read, FileShare.Read))
         {
            XDocument xKeys = XDocument.Load(fStr);
            Keys = (from k in xKeys.Descendants("key")
                     orderby int.Parse(k.Attribute("build").Value)
                     select new CryptoKey(k.Attribute("build").Value, k.Attribute("value").Value)).ToArray();
         }
      }
   }
}