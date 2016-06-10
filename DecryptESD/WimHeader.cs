using System;
using System.Runtime.InteropServices;

namespace DecryptESD
{
   [StructLayout(LayoutKind.Sequential, Pack = 4)]
   public unsafe struct WimHeader
   {
      public fixed byte Magic [8];
      public uint HeaderSize;
      public uint WimVersion;
      public WimHeaderFlags WimFlags;
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

      [FieldOffset(7)]
      public ResourceHeaderFlags Flags;

      [FieldOffset(8)]
      public long OffsetInWim;

      [FieldOffset(16)]
      public ulong OriginalSize;

      public ulong SizeInWim => SizeInWimWithFlags & 0xFFFFFFFFFFFFFF;
   }

   [Flags]
   public enum WimHeaderFlags : uint
   {
      None = 0,
      Reserved = 1,
      Compressed = 2,
      ReadOnly = 4,
      Spanned = 8,
      ResourcesOnly = 0x10,
      MetadataOnly = 0x20,
      WriteInProgress = 0x40,
      ReparsePointFixup = 0x80,

      CompressReserved = 0x10000,
      CompressXpress = 0x20000,
      CompressLzx = 0x40000,
      CompressLzms = 0x80000,

      CompressXpress2 = 0x200000
   }

   [Flags]
   public enum ResourceHeaderFlags : byte
   {
      Free = 0x1,
      Metadata = 0x2,
      Compressed = 0x4,
      Spanned = 0x8
   }
}