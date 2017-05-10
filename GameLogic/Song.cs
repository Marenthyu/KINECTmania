using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace KINECTmania.GameLogic
{
    class Song
    {
        private List<Note> notes;
        private String songTitle, songArtist, songFile;
        private long length;
        public const String FILEINFO_SECTION = "FileInfo", SONGINFO_SECTION = "SongInfo", NOTE_SECTION = "Notes";

        public Song(String songFile)
        {
            
        }

        private Song(String songTitle, String songArtist, String songFile, long length, List<Note> notes)
        {
            this.songTitle = songTitle;
            this.songArtist = songArtist;
            this.songFile = songFile;
            this.length = length;
            this.notes = notes;
        }

        private Song loadSong(String songFile)
        {
            FileParser fp = new FileParser(songFile);
            List<String> lines = fp.readFile();
            if (lines == null)
            {
                throw new FileNotFoundException("Song not found.");
            }
            String currentSection = null;
            int currLine = 0;
            foreach (String line in lines)
            {
                currLine++;
                if (line.Equals(""))
                {
                    Console.WriteLine("Ignoring empty line in file {0} at line {1}, continuing...", songFile, currLine);
                    continue;
                }
                if (line.StartsWith("#"))
                {
                    String section = line.Replace("# ", "");
                    currentSection = section;
                    continue;
                }
                switch (currentSection)
                {
                    case NOTE_SECTION:

                        break;
                }
            }
            return null;
        }

    }
}
