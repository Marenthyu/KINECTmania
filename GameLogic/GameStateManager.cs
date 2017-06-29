using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace KINECTmania.GameLogic
{
    class GameStateManager
    {
        public GameState State { get; private set; }
        private Song CurrentSong { get; set; }

        public GameStateManager()
        {
            State = GameState.MAIN_MENU;
            CurrentSong = null;
        }

        public event GameEventHandler RaiseGameEvent;

        protected virtual void OnRaiseGameEvent(GameEventArgs e)
        {
            RaiseGameEvent?.Invoke(this, e);
        }

        public void RaiseDummyEvent()
        {
            OnRaiseGameEvent(new GameEventArgs(new Note(888L, (short) 4), Accuracy.MARVELOUS, 10000));
        }

        public Song LoadSong(String path)
        {
            CurrentSong = new Song(path);

            if (!File.Exists(CurrentSong.SongFile))
            {
                String Location = CurrentSong.SongFile;
                CurrentSong = null;
                throw new SongFileNotFoundException("Could not find Song file. Please make sure " + Path.GetFullPath(Location) +
                                                    " exists.");
            }
            return CurrentSong;
        }

        public void Start()
        {
            if (State == GameState.IN_GAME)
            {
                throw new InvalidGameStateTransitionException("Please pause or end a game before starting.");
            }

            if (CurrentSong == null)
            {
                throw new NoSongLoadedException("Please load a Song before starting!");
            }
        }

        public void Pause()
        {
            switch (State)
            {
                case GameState.MAIN_MENU:
                {
                    throw new InvalidGameStateTransitionException("Can not transition from main menu to paused.");
                }
                case GameState.PAUSED:
                    throw new InvalidGameStateTransitionException("Already paused");
                  
                case GameState.IN_GAME:
                    State = GameState.PAUSED;
                    break;
                case GameState.LOADING_SONG:
                    throw new InvalidGameStateTransitionException("Can not pause while loading song.");
                case GameState.OPTIONS:
                    throw new InvalidGameStateTransitionException("Can't pause in Options Menu.");
                case GameState.SCORES:
                    throw new InvalidGameStateTransitionException("Can not pause on scores.");
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }
    }

    public enum GameState
    {
        MAIN_MENU, PAUSED, IN_GAME, LOADING_SONG, OPTIONS, SCORES
    }

    public class GameEventArgs : EventArgs
    {
        public GameEventArgs(Note n, Accuracy acc, int points)
        {
            Note = n;
            Accuracy = acc;
            Points = points;
        }
        public Note Note { get; private set; }
        public Accuracy Accuracy { get; private set; }
        public int Points { get; private set; }

    }

    public delegate void GameEventHandler(object sender, GameEventArgs e);

    public enum Accuracy
    {
        MARVELOUS, PERFECT, GREAT, GOOD, BAD, BOO, MISS
    }

    // Auto-Generated Exceptions
    [Serializable]
    public class InvalidGameStateTransitionException : Exception
    {
        public InvalidGameStateTransitionException()
        {
        }

        public InvalidGameStateTransitionException(string message) : base(message)
        {
        }

        public InvalidGameStateTransitionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidGameStateTransitionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class SongFileNotFoundException : Exception
    {
        public SongFileNotFoundException()
        {
        }

        public SongFileNotFoundException(string message) : base(message)
        {
        }

        public SongFileNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SongFileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class NoSongLoadedException : Exception
    {
        public NoSongLoadedException()
        {
        }

        public NoSongLoadedException(string message) : base(message)
        {
        }

        public NoSongLoadedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoSongLoadedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

 
}
