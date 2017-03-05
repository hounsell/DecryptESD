using System;
using System.Runtime.InteropServices;

namespace WIMCore
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
}