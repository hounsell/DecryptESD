using System;
using System.Runtime.InteropServices;

namespace DecryptESD
{
   public unsafe struct WimHeader
   {
      public fixed byte Magic [8];
      public uint HeaderSize;
      public uint WimVersion;
      public uint WimFlags;
      public uint ChunkSize;
      public Guid WimGuid;
      public ushort PartNumber;
      public ushort TotalParts;
      public uint ImageCount;
      public ResourceHeader LookupTable;
      public ResourceHeader XmlData;
      public ResourceHeader BootMetadata;
      public uint BootIndex;
      public ResourceHeader IntegrityTable;
      public fixed byte Unused [60];
   }

   [StructLayout(LayoutKind.Explicit, Size = 24)]
   public struct ResourceHeader
   {
      [FieldOffset(0)]
      public ulong SizeInWimWithFlags;

      [FieldOffset(0)]
      public byte Flags;

      [FieldOffset(8)]
      public long OffsetInWim;

      [FieldOffset(16)]
      public ulong OriginalSize;

      public ulong SizeInWim => SizeInWimWithFlags & 0xFFFFFFFFFFFFFF;
   }
}