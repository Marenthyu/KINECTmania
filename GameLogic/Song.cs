using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace KINECTmania.GameLogic
{
    /// <summary>
    /// A Playable Song including all the information from a .kmsf file
    /// </summary>
    public class Song
    {
        public List<Note> Notes { get; private set; }
        public string SongTitle { get; private set; }
        public string SongArtist { get; private set; }
        public string SongFile { get; private set; }
        public long Length { get; private set; }
        public const String FILEINFO_SECTION = "FileInfo", SONGINFO_SECTION = "SongInfo", NOTE_SECTION = "Notes";

        public Song(String songFile)
        {
            LoadSong(songFile);
        }

        private Song(String songTitle, String songArtist, String songFile, long length, List<Note> notes)
        {
            this.SongTitle = songTitle;
            this.SongArtist = songArtist;
            this.SongFile = songFile;
            this.Length = length;
            this.Notes = notes;
        }

        /// <summary>
        /// Loads a Song from a .kmsf File. Convenience Method.
        /// </summary>
        /// <param name="kmsfFile">Path to the kmsf File to be parsed.</param>
        private void LoadSong(String kmsfFile)
        {
            FileParser fp = new FileParser(kmsfFile);
            List<String> lines = fp.readFile();
            if (lines == null)
            {
                throw new FileNotFoundException("Song not found.");
            }
            String currentSection = null;
            int currLine = 0;
            Notes = new List<Note>();
            foreach (String line in lines)
            {
                currLine++;
                if (line.Equals(""))
                {
                    Console.WriteLine("Ignoring empty line in file {0} at line {1}, continuing...", kmsfFile, currLine);
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
                        Note noteToAdd = Note.NoteFromLine(line);
                        Notes.Add(noteToAdd);
                        Console.WriteLine("Added note: " + noteToAdd);
                        break;
                    case FILEINFO_SECTION:
                        if (!line.Equals("version=1"))
                        {
                            throw new InvalidSongFileVersionException();
                        }
                        break;
                    case SONGINFO_SECTION:
                        string[] parts = line.Split('=');
                        string key = parts[0];
                        string value = "";
                        bool keyskipped = false;
                        bool firstDone = false;
                        foreach (string s in parts)
                        {
                            if (!keyskipped)
                            {
                                keyskipped = true;
                                continue;
                            }
                            else
                            {   
                                if (firstDone)
                                    value += "=";
                                else
                                {
                                    firstDone = true;
                                }
                            }
                            value += s;
                        }
                        switch (key)
                        {
                            case "SongFile":
                                SongFile = value;
                                break;
                            case "Length":
                                Length = long.Parse(value);
                                break;
                            case "SongTitle":
                                SongTitle = value;
                                break;
                            case "SongArtist":
                                SongArtist = value;
                                break;
                            default:
                                Console.WriteLine("Unknown Attribute {0}, skipping.", value);
                                break;
                        }
                        break;
                    default:
                        Console.WriteLine("Unknown Section {0}. Skipping Line {1}", currentSection, currLine);
                        break;
                }
            }
        }

        public override String ToString()
        {
            return
                $"Song[Title: {SongTitle}; Artist: {SongArtist}; Length: {Length}; Song File: {SongFile}; Notes: [{string.Join(", ", Notes)}]]";
        }

    }

    /// <summary>
    /// Gets thrown when a Song tries to be loaded from an Unsupported Version of a kmsf File. Currently, only Version 1 is supported.
    /// </summary>
    class InvalidSongFileVersionException : Exception
    {
        
    }
}
