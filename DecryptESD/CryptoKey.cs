using System;

namespace DecryptESD
{
   public class CryptoKey
   {
      public int FirstBuild { get; }

      public byte[] Key { get; }

      public CryptoKey(string firstBuild, string key)
      {
         FirstBuild = int.Parse(firstBuild);
         Key = Convert.FromBase64String(key);
      }
   }
}