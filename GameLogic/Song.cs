using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace KINECTmania.GameLogic
{
    public class Song
    {
        private List<Note> notes;
        private String songTitle, songArtist, songFile;
        private long length;
        public const String FILEINFO_SECTION = "FileInfo", SONGINFO_SECTION = "SongInfo", NOTE_SECTION = "Notes";

        public Song(String songFile)
        {
            LoadSong(songFile);
        }

        private Song(String songTitle, String songArtist, String songFile, long length, List<Note> notes)
        {
            this.songTitle = songTitle;
            this.songArtist = songArtist;
            this.songFile = songFile;
            this.length = length;
            this.notes = notes;
        }

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
            notes = new List<Note>();
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
                        notes.Add(noteToAdd);
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
                                songFile = value;
                                break;
                            case "Length":
                                length = long.Parse(value);
                                break;
                            case "SongTitle":
                                songTitle = value;
                                break;
                            case "SongArtist":
                                songArtist = value;
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
                $"Song[Title: {songTitle}; Artist: {songArtist}; Length: {length}; Song File: {songFile}; Notes: [{string.Join(", ", notes)}]]";
        }

        public List<Note> GetNotes()
        {
            return notes;
        }

        public long GetLength()
        {
            return length;
        }

    }

    class InvalidSongFileVersionException : Exception
    {
        
    }
}
