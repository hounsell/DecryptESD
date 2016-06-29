using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace CryptoKeySite.Models
{
   public class CryptoKey : IModelWithId
   {
      [Display(Name="First Build")]
      public int FirstBuild { get; set; }
      public Guid Id { get; set; }

      public byte[] Key { get; set; }

      [Display(Name = "Key")]
      public string KeyBase64
      {
         get { return Convert.ToBase64String(Key ?? Array.Empty<byte>()); }
         set { Key = Convert.FromBase64String(value); }
      }

      [Display(Name = "Exchange Algorithm")]
      public string KeyExchangeAlgorithm
      {
         get
         {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
               rsa.ImportCspBlob(Key);
               return rsa.KeyExchangeAlgorithm;
            }
         }
      }

      [Display(Name = "Key Size")]
      public int KeySize
      {
         get
         {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
               rsa.ImportCspBlob(Key);
               return rsa.KeySize;
            }
         }
      }
   }
}