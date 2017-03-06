namespace WIMCore
{
    public unsafe struct StreamEntry
    {
        public ulong Length;
        public ulong Reserved1;
        public fixed byte Hash [20];
        public ushort StreamNameLength;
    }
}