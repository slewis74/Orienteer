using System;
using System.Diagnostics;
using Orienteer.Data;

namespace Sample.Shared.Model
{
    [DebuggerDisplay("Song - {TrackNumber} {Title}")]
    public class Song : BindableBase
    {
        public Song()
        {
            DiscNumber = 1;
        }

        public uint DiscNumber { get; set; }
        public uint TrackNumber { get; set; }
        
        public string Title { get; set; }
        public string Path { get; set; }

        public TimeSpan Duration { get; set; }
    }
}