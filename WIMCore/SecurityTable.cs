namespace WIMCore
{
    public class SecurityTable
    {
        public SecurityTableHeader Header { get; }
        public byte[][] Descriptors { get; }

        public SecurityTable(SecurityTableHeader header, byte[][] descriptors)
        {
            Header = header;
            Descriptors = descriptors;
        }
    }
}