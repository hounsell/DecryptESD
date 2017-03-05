using System.Runtime.InteropServices;

namespace WIMCore
{
    [StructLayout(LayoutKind.Explicit, Size = 50)]
    public unsafe struct LookupEntry
    {
        [FieldOffset(0)]
        public ulong SizeInWimWithFlags;

        [FieldOffset(7)]
        public ResourceHeaderFlags Flags;

        [FieldOffset(8)]
        public long OffsetInWim;

        [FieldOffset(16)]
        public ulong OriginalSize;

        [FieldOffset(24)]
        public ushort PartNumber;

        [FieldOffset(26)]
        public uint ReferenceCount;

        [FieldOffset(30)]
        public fixed byte Hash [20];

        public ulong SizeInWim => SizeInWimWithFlags & 0xFFFFFFFFFFFFFF;
    }
}