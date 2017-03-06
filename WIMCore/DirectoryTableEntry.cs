using System.Collections.Generic;

namespace WIMCore
{
    public class DirectoryTableEntry
    {
        public string Name { get; }
        public DirectoryEntry Entry { get; }

        public Dictionary<string, StreamEntry> AlternateStreams { get; }

        public DirectoryTableEntry(string name, DirectoryEntry entry, Dictionary<string, StreamEntry> alternateStreams)
        {
            Name = name;
            Entry = entry;
            AlternateStreams = alternateStreams;
        }
    }
}