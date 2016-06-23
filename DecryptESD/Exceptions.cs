using System;

namespace DecryptESD
{
   public class NoValidCryptoKeyException : Exception { }

   public class UnencryptedImageException : Exception { }

   public class NoImageKeyException : Exception { }
}