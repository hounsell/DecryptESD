using System.Runtime.InteropServices;

namespace WIMCore
{
    [StructLayout(LayoutKind.Explicit, Size = 102)]
    public unsafe struct DirectoryEntry
    {
        [FieldOffset(0)]
        public ulong EntryLength;

        [FieldOffset(8)]
        public uint Attributes;

        [FieldOffset(12)]
        public uint SecurityId;

        [FieldOffset(16)]
        public ulong SubDirectoryOffset;

        [FieldOffset(24)]
        public ulong Reserved1;

        [FieldOffset(32)]
        public ulong Reserved2;

        [FieldOffset(40)]
        public ulong CreationTime;

        [FieldOffset(48)]
        public ulong LastAccessTime;

        [FieldOffset(56)]
        public ulong LastWriteTime;

        [FieldOffset(64)]
        public fixed byte Hash [20];

        [FieldOffset(88)]
        public uint ReparseTag;

        [FieldOffset(92)]
        public ushort ReparseReserved;

        [FieldOffset(94)]
        public ushort ReparseFlags;

        [FieldOffset(88)]
        public ulong HardLink;

        [FieldOffset(96)]
        public ushort Streams;

        [FieldOffset(98)]
        public ushort ShortFileNameLength;

        [FieldOffset(100)]
        public ushort FileNameLength;
    }
}