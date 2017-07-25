using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KINECTmania.GameLogic
{
    /// <summary>
    /// A note (or "Arrow") that should be hit during a song.
    /// </summary>
    public class Note
    {
        private long startTime;
        private short position;

        public Note(long startTime, short position)
        {
            this.startTime = startTime;
            this.position = position;
        }

        /// <summary>
        /// Parses a note from a Line of a .kmsf file. Convenience method.
        /// </summary>
        /// <param name="line">The line read from the .kmsf file. Format: [startTime][space][position]</param>
        /// <returns>the parsed Note</returns>
        public static Note NoteFromLine(String line)
        {
            long startTime;
            short position;
            String[] parts = line.Split(' ');

            bool startSuccess = long.TryParse(parts[0], out startTime);
            if (!startSuccess)
            {
                throw new FormatException($"Could not parse Note, invalid long for startTime: {parts[0]}");
            }
            bool positionSuccess = short.TryParse(parts[1], out position);
            if (!positionSuccess)
            {
                throw new FormatException($"Could not parse Note, invalid Int16 for position: {parts[1]}");
            }
            return new Note(startTime, position);
        }

        public long StartTime()
        {
            return this.startTime;
        }

        public short Position()
        {
            return this.position;
        }

        public override String ToString()
        {
            return $"Note[Start: {startTime}; Position: {position}]";
        }
    }
}
