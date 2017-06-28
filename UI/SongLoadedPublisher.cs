using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KINECTmania.GUI
{
    public interface SongLoadedPublisher
    {
        event EventHandler<SongLoaded> RaiseSongLoaded;

        void OnRaiseSongLoaded(SongLoaded e);
    }
}
