using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KINECTmania.GameLogic;

namespace KINECTmania.GUI
{
    public class MenuStateChanged : EventArgs
    {
        private int menuState;

        public int MenuState
        {
            get { return menuState; }
            set { menuState = value; }
        }

        public MenuStateChanged(int b)
        {
            ///<summary>
            /// 0: main menu
            /// 1: options menu (changing volume, ...)
            /// 2: game settings menu (changing bpm, song, ... just before start of the game)
            /// 3: during game
            /// -1: exiting the game window
            ///</summary>
            menuState = b;
        }
    }

    public interface Menu
    {
        event EventHandler<MenuStateChanged> RaiseMenuStateChanged;

        void OnRaiseMenuStateChanged(MenuStateChanged e);

    }
    
    public class GameOptionsSet : EventArgs
    {
        private int ms;

        public int MS
        {
            get { return ms; }
            set { ms = value; }
        }

        public GameOptionsSet(int ms)
        {
            this.ms = ms;
        }

        public GameOptionsSet(float sec)
        {
            this.ms = (int) (sec * 1000); //Convert to ms (by multiplying by 1000) and then truncating the float
        }
    
    }

    public interface GameOptionsSetter
    {
        event EventHandler<GameOptionsSet> RaiseGameOptionsSet;
        void OnRaiseGameOptionsSet(GameOptionsSet g);
    }

    public class SongLoaded : EventArgs
    {
        private Song loadedSong;

        public Song LoadedSong
        {
            get { return loadedSong; }
            set { loadedSong = value; }
        }

        public SongLoaded(Song s)
        {
            loadedSong = s;
        }
    }

    public interface SongLoadedPublisher
    {
        event EventHandler<SongLoaded> RaiseSongLoaded;
        void OnRaiseSongLoaded(SongLoaded s);
    }

    public class KinectStreamRequested : EventArgs
    {

    }

    public interface KinectStreamRequestedPublisher
    {
        event EventHandler<KinectStreamRequested> RaiseKinectStreamRequested;

        void OnRaiseKinectStreamRequested(KinectStreamRequested k);
    }

    public class CountdownFinished : EventArgs
    {

    }

    public interface CountdownFinishedPublisher
    {
        event EventHandler<CountdownFinished> RaiseCountdownFinished;
        void OnRaiseCountdownFinished(CountdownFinished CdownFin);
    }
}
