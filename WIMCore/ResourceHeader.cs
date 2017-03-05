using System.Runtime.InteropServices;

namespace WIMCore
{
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
}