namespace WIMCore
{
    public class LookupTable
    {
        public LookupEntry[] Entries { get; }

        public LookupTable(LookupEntry[] entries)
        {
            Entries = entries;
        }
    }
}