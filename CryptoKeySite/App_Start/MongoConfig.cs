using System.Configuration;

namespace CryptoKeySite
{
   internal static class MongoConfig
   {
      public static string Database { get; }
      public static string Host { get; }
      public static int Port { get; }

      static MongoConfig()
      {
         Host = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["data:MongoHost"])
            ? ConfigurationManager.AppSettings["data:MongoHost"]
            : "localhost";

         int port;
         bool success = int.TryParse(ConfigurationManager.AppSettings["data:MongoPort"], out port);
         if (!success)
         {
            port = 27017; // mongo default port
         }
         Port = port;

         Database = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["data:MongoDB"])
            ? ConfigurationManager.AppSettings["data:MongoDB"]
            : "MongoAuth";
      }

      public static void SetupIndexes()
      {
      }
   }
}