using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using DecryptESD.Properties;

namespace DecryptESD
{
   public class CryptoKey
   {
      private const string XML_FILE_NAME = "CryptoKeys.xml";
      private static readonly string xmlFeedUrl = Settings.Default.XmlFeedURL;

      public int FirstBuild { get; }
      public byte[] Key { get; }
      public static CryptoKey[] Keys { get; private set; }


      public string KeyBase64 => Convert.ToBase64String(Key ?? new byte[]
      {
      });

      private static string XmlPath => Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) ?? "", XML_FILE_NAME);

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
         using (FileStream fStr = new FileStream(XmlPath, FileMode.Open, FileAccess.Read, FileShare.Read))
         {
            XDocument xKeys = XDocument.Load(fStr);
            Keys = (from k in xKeys.Descendants("key")
                    orderby int.Parse(k.Attribute("build").Value)
                    select new CryptoKey(k.Attribute("build").Value, k.Attribute("value").Value)).ToArray();
         }
      }

      public static void ReplaceXmlWithWebFeed() => ReplaceXmlWithWebFeed(xmlFeedUrl);

      public static void ReplaceXmlWithWebFeed(string url)
      {
         HttpWebRequest wreq = WebRequest.CreateHttp(url);

         using (HttpWebResponse wres = wreq.GetResponse() as HttpWebResponse)
         using (Stream str = wres.GetResponseStream())
         using (FileStream fStr = new FileStream(XmlPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
         {
            str.CopyTo(fStr);
            fStr.Flush();
         }
      }

      public static void MergeWebFeedIntoXml() => MergeWebFeedIntoXml(xmlFeedUrl);

      public static void MergeWebFeedIntoXml(string url)
      {
         HttpWebRequest wreq = WebRequest.CreateHttp(url);

         using (HttpWebResponse wres = wreq.GetResponse() as HttpWebResponse)
         using (Stream str = wres.GetResponseStream())
         using (FileStream fStr = new FileStream(XmlPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
         {
            XDocument xCurrent = XDocument.Load(fStr);
            List<CryptoKey> ckCurrent = (from k in xCurrent.Descendants("key")
                                         orderby int.Parse(k.Attribute("build").Value)
                                         select new CryptoKey(k.Attribute("build").Value, k.Attribute("value").Value)).ToList();

            XDocument xFeed = XDocument.Load(str);
            CryptoKey[] ckFeed = (from k in xFeed.Descendants("key")
                                  orderby int.Parse(k.Attribute("build").Value)
                                  select new CryptoKey(k.Attribute("build").Value, k.Attribute("value").Value)).ToArray();

            foreach (CryptoKey ck in ckFeed)
            {
               if (ckCurrent.All(k => k.KeyBase64 != ck.KeyBase64))
               {
                  ckCurrent.Add(ck);
               }
            }


            XDocument xOutput = new XDocument(new XElement("keys",
               from r in ckCurrent
               select new XElement("key", new XAttribute("build", r.FirstBuild), new XAttribute("value", r.KeyBase64))));

            fStr.Position = 0;
            fStr.SetLength(0);

            xOutput.Save(fStr);
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