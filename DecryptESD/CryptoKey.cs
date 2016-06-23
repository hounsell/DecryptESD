using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DecryptESD
{
   public class CryptoKey
   {
      public int FirstBuild { get; }
      public byte[] Key { get; }
      public static CryptoKey[] Keys { get; private set; }

      public CryptoKey(string firstBuild, string key)
      {
         FirstBuild = int.Parse(firstBuild);
         Key = Convert.FromBase64String(key);
      }

      public CryptoKey(int firstBuild, string key)
      {
         FirstBuild = firstBuild;
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

      public static void UseCustomKey(string key)
      {
         Keys = new[]
         {
            new CryptoKey(0, key)
         };
      }
   }
}