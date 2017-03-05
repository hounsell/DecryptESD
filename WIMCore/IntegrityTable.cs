namespace WIMCore
{
    public class IntegrityTable
    {
        public IntegrityTableHeader Header { get; }
        public byte[][] Hashes { get; }

        public IntegrityTable(IntegrityTableHeader header, byte[][] hashes)
        {
            Header = header;
            Hashes = hashes;
        }
    }
}