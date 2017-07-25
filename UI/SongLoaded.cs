using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KINECTmania.GameLogic;

namespace KINECTmania.GUI
{
    public class SongLoaded : EventArgs
    {
        private Song s;

        public Song Song
        {
            get { return s; }
        }

        public SongLoaded(Song s)
        {
            this.s = s;
        }
    }
}
