namespace WIMCore
{
    public class WimImage
    {
        private DirectoryTableEntry[] _directoryTable;
        private SecurityTable _securityTable;

        public WimImage(SecurityTable securityTable, DirectoryTableEntry[] directoryTable)
        {
            _securityTable = securityTable;
            _directoryTable = directoryTable;
        }
    }
}