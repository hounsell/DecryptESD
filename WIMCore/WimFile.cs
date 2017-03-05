using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace WIMCore
{
    public class WimFile : IDisposable
    {
        private readonly FileStream _file;

        private WimHeader _header;
        private IntegrityTable _integrityTable;
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;
        private XDocument _xmlMetadata;

        public WimHeaderFlags Flags => _header.WimFlags;

        public WimFile(string path)
        {
            _file = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            _reader = new BinaryReader(_file, Encoding.Unicode, true);
            _writer = new BinaryWriter(_file, Encoding.Unicode, true);

            ReadWimHeader();
        }

        public void Dispose()
        {
            _writer.Flush();

            _reader.Dispose();
            _writer.Dispose();
            _file.Dispose();
        }

        private unsafe void ReadWimHeader()
        {
            if (_file.Length < sizeof(WimHeader))
            {
                throw new WimNotValidException(WimNotValidException.ErrorType.InvalidSize);
            }

            _file.Position = 0;

            byte[] bHeader = _reader.ReadBytes(sizeof(WimHeader));
            fixed (byte* pHeader = bHeader)
            {
                _header = Marshal.PtrToStructure<WimHeader>((IntPtr)pHeader);
            }

            byte[] magic =
            {
                0x4D,
                0x53,
                0x57,
                0x49,
                0x4D,
                0,
                0,
                0
            };

            bool valid = true;
            fixed (byte* magicFile = _header.Magic)
            {
                for (int i = 0; i < magic.Length; i++)
                {
                    valid &= magic[i] == magicFile[i];
                }
            }

            if (!valid)
            {
                throw new WimNotValidException(WimNotValidException.ErrorType.InvalidMagic);
            }
        }

        private void ReadXmlMetadata()
        {
            _file.Position = _header.XmlData.OffsetInWim;

            byte[] bXml = _reader.ReadBytes((int)_header.XmlData.SizeInWim);
            using (MemoryStream mStr = new MemoryStream(bXml))
            {
                _xmlMetadata = XDocument.Load(mStr);
            }
        }

        private unsafe void ReadIntegrityTable()
        {
            _file.Position = _header.IntegrityTable.OffsetInWim;

            byte[] bTable = _reader.ReadBytes(sizeof(IntegrityTableHeader));
            IntegrityTableHeader itHeader;
            fixed (byte* pTable = bTable)
            {
                itHeader = Marshal.PtrToStructure<IntegrityTableHeader>((IntPtr)pTable);
            }

            var bbHashes = new byte[itHeader.TableRowCount][];
            for (int i = 0; i < bbHashes.Length; i++)
            {
                bbHashes[i] = _reader.ReadBytes(20);
            }

            _integrityTable = new IntegrityTable(itHeader, bbHashes);
        }
    }
}