using System;

namespace DecryptESD.IO
{
   [Flags]
   public enum ResourceHeaderFlags : byte
   {
      Free = 0x1,
      Metadata = 0x2,
      Compressed = 0x4,
      Spanned = 0x8
   }
}