using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using WIMCore.Exceptions;

namespace WIMCore
{
    public class WimFile : IDisposable
    {
        private readonly FileStream _file;

        private WimHeader _header;

        private readonly WimImage[] _images;
        private IntegrityTable _integrityTable;
        private LookupTable _lookupTable;
        private readonly BinaryReader _reader;
        private readonly BinaryWriter _writer;
        private XDocument _xmlMetadata;

        public uint ImageCount => _header.ImageCount;
        public WimHeaderFlags Flags => _header.WimFlags;

        public WimFile(string path)
        {
            _file = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            _reader = new BinaryReader(_file, Encoding.Unicode, true);
            _writer = new BinaryWriter(_file, Encoding.Unicode, true);

            ReadWimHeader();
            _images = new WimImage[_header.ImageCount];

            ReadLookupTable();
        }

        public void Dispose()
        {
            _writer.Flush();

            _reader.Dispose();
            _writer.Dispose();
            _file.Dispose();
        }

        public unsafe bool CheckIntegrity()
        {
            if (_header.IntegrityTable.OffsetInWim == 0)
            {
                throw new WimIntegrityException(WimIntegrityExceptionType.NoIntegrityData);
            }

            if (_integrityTable == null)
            {
                ReadIntegrityTable();
            }

            bool verified = true;

            long chunkStart = 0;
            long chunkStartNum = chunkStart / _integrityTable.Header.ChunkSize;
            long chunkEnd = _header.XmlData.OffsetInWim - sizeof(WimHeader);
            long chunkEndNum = chunkEnd / _integrityTable.Header.ChunkSize;

            using (SHA1 sha = SHA1.Create())
            {
                for (long i = chunkStartNum; i <= chunkEndNum; i++)
                {
                    _file.Position = sizeof(WimHeader) + i * _integrityTable.Header.ChunkSize;

                    int chunkSize = _file.Position + _integrityTable.Header.ChunkSize > _header.XmlData.OffsetInWim
                        ? (int)(_header.XmlData.OffsetInWim - _file.Position)
                        : (int)_integrityTable.Header.ChunkSize;

                    byte[] data = _reader.ReadBytes(chunkSize);
                    byte[] computedHash = sha.ComputeHash(data, 0, data.Length);

                    for (int j = 0; j < computedHash.Length; j++)
                    {
                        verified &= computedHash[j] == _integrityTable.Hashes[i][j];
                    }
                }
            }

            return verified;
        }

        public void LoadImageMetadata()
        {
            for (int i = 0; i < ImageCount; i++)
            {
                LoadImageMetadata(i);
            }
        }

        private unsafe void ReadWimHeader()
        {
            if (_file.Length < sizeof(WimHeader))
            {
                throw new WimInvalidException(WimInvalidExceptionType.InvalidSize);
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
                throw new WimInvalidException(WimInvalidExceptionType.InvalidMagic);
            }
        }

        private unsafe void ReadLookupTable()
        {
            _file.Position = _header.LookupTable.OffsetInWim;
            ulong entries = _header.LookupTable.OriginalSize / (ulong)sizeof(LookupEntry);

            var entryArray = new LookupEntry[entries];
            for (ulong i = 0; i < entries; i++)
            {
                byte[] bEntry = _reader.ReadBytes(sizeof(LookupEntry));
                fixed (byte* pEntry = bEntry)
                {
                    entryArray[i] = Marshal.PtrToStructure<LookupEntry>((IntPtr)pEntry);
                }
            }

            _lookupTable = new LookupTable(entryArray);
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

        private unsafe void LoadImageMetadata(int imageIndex)
        {
            if (_lookupTable == null)
            {
                ReadLookupTable();
            }

            if (imageIndex >= _header.ImageCount)
            {
                throw new ArgumentOutOfRangeException(nameof(imageIndex));
            }

            LookupEntry metadataEntry = _lookupTable.Entries.Where(l => (l.Flags & ResourceHeaderFlags.Metadata) == ResourceHeaderFlags.Metadata).OrderBy(l => l.OffsetInWim).Skip(imageIndex).First();

            _file.Position = metadataEntry.OffsetInWim;

            byte[] bTable = _reader.ReadBytes(sizeof(SecurityTableHeader));
            SecurityTableHeader stHeader;
            fixed (byte* pTable = bTable)
            {
                stHeader = Marshal.PtrToStructure<SecurityTableHeader>((IntPtr)pTable);
            }

            var sdLengths = new ulong[stHeader.EntryCount];
            for (int i = 0; i < sdLengths.Length; i++)
            {
                sdLengths[i] = _reader.ReadUInt64();
            }

            var sdDescriptors = new byte[stHeader.EntryCount][];
            for (int i = 0; i < sdLengths.Length; i++)
            {
                sdDescriptors[i] = _reader.ReadBytes((int)sdLengths[i]);
            }

            SecurityTable st = new SecurityTable(stHeader, sdDescriptors);

            var dirEntries = new List<DirectoryTableEntry>();
            while (_file.Position < metadataEntry.OffsetInWim + (long)metadataEntry.SizeInWim)
            {
                long startPosition = _file.Position;
                byte[] bdEntry = _reader.ReadBytes(sizeof(DirectoryEntry));
                DirectoryEntry dEntry;
                fixed (byte* pdEntry = bdEntry)
                {
                    dEntry = Marshal.PtrToStructure<DirectoryEntry>((IntPtr)pdEntry);
                }

                string name = Encoding.Unicode.GetString(_reader.ReadBytes(dEntry.FileNameLength));

                var altStreams = new Dictionary<string, StreamEntry>(dEntry.Streams);
                for (int i = 0; i < dEntry.Streams; i++)
                {
                    byte[] bsEntry = _reader.ReadBytes(sizeof(StreamEntry));
                    StreamEntry sEntry;
                    fixed (byte* psEntry = bsEntry)
                    {
                        sEntry = Marshal.PtrToStructure<StreamEntry>((IntPtr)psEntry);
                    }

                    string sName = Encoding.Unicode.GetString(_reader.ReadBytes(sEntry.StreamNameLength));

                    altStreams.Add(sName, sEntry);
                }

                dirEntries.Add(new DirectoryTableEntry(name, dEntry, altStreams));

                // TODO: Transveral logic
            }

            _images[imageIndex] = new WimImage(st, dirEntries.ToArray());
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
    }
}