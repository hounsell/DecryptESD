using System;

namespace CryptoKeySite.Models
{
   public interface IModelWithId
   {
      Guid Id { get; set; }
   }
}