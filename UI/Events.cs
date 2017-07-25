using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KINECTmania.GameLogic;

namespace KINECTmania.GUI
{
    /// <summary>
    /// Wird gefeuert, wenn ein Menüwechsel stattfinden soll
    /// </summary>
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
    
    /// <summary>
    /// Wird gefeuert, wenn die Spieloptionen (eng. game options) gesetzt werden
    /// </summary>
    public class GameOptionsSet : EventArgs
    {
        
        private int ms; //wieviele ms zwischen dem Auftauchen am unteren Bildschirmrand und den (perfekten) Eintreffen auf der Hitbox vergehen sollen

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

    /// <summary>
    /// Übergibt den geladenen Song an alle Subscriber
    /// </summary>
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

    

    [Obsolete("Wird nicht mehr benötigt")]
    public class CountdownFinished : EventArgs
    {

    }
    [Obsolete("Wird nicht mehr benötigt")]
    public interface CountdownFinishedPublisher
    {
        event EventHandler<CountdownFinished> RaiseCountdownFinished;
        void OnRaiseCountdownFinished(CountdownFinished CdownFin);
    }
}
