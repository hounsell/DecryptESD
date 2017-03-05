using System;

namespace WIMCore
{
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
}