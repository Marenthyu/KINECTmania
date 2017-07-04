using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KINECTmania.kinectProcessing;
using NAudio.Utils;
using NAudio.Wave;

namespace KINECTmania.GameLogic
{
    class GameStateManager
    {
        private WaveOutEvent _waveOut;
        private Thread _playThread;
        public GameState State { get; private set; }
        private Song CurrentSong { get; set; }
        private ArrowHitPublisher Ahp { get; set; }
        private List<Note> remainingNotes { get; set; }
        private static long CurrentTime { get; set; }
    

        public GameStateManager()
        {
            State = GameState.MAIN_MENU;
            CurrentSong = null;
            Ahp = KinectDataInput.arrowPub;

            Ahp.RaiseKinectEvent += AhpOnRaiseKinectEvent;
            
        }

        private void AhpOnRaiseKinectEvent(object sender, KinectArrowHitEventArgs kinectArrowHitEventArgs)
        {
            Console.WriteLine("Checking event catched for note at " + kinectArrowHitEventArgs.Message + "; time: " + CurrentTime);
            foreach (Note n in remainingNotes)
            {
                long startTime = n.StartTime();
                if (startTime > CurrentTime - 200 && startTime < CurrentTime + 200 && n.Position().Equals(kinectArrowHitEventArgs.Message))
                {
                    long delta = CurrentTime - startTime;
                    if (delta < 0)
                    {
                        delta = delta * -1;
                    }
                    if (delta < 33)
                    {
                        OnRaiseGameEvent(new GameEventArgs(n, Accuracy.MARVELOUS, 10000));
                        remainingNotes.Remove(n);
                        return;
                    }
                    if (delta < 66)
                    {
                        OnRaiseGameEvent(new GameEventArgs(n, Accuracy.PERFECT, 6666));
                        remainingNotes.Remove(n);
                        return;
                    }
                    if (delta < 100)
                    {
                        OnRaiseGameEvent(new GameEventArgs(n, Accuracy.GREAT, 3333));
                        remainingNotes.Remove(n);
                        return;
                    }
                    if (delta < 133)
                    {
                        OnRaiseGameEvent(new GameEventArgs(n, Accuracy.GOOD, 500));
                        remainingNotes.Remove(n);
                        return;
                    }
                    if (delta < 166)
                    {
                        OnRaiseGameEvent(new GameEventArgs(n, Accuracy.BAD, 0));
                        remainingNotes.Remove(n);
                        return;
                    }
                    OnRaiseGameEvent(new GameEventArgs(n, Accuracy.BOO, 0));
                    remainingNotes.Remove(n);
                    return;
                }
            }
        }

        public event GameEventHandler RaiseGameEvent;

        protected virtual void OnRaiseGameEvent(GameEventArgs e)
        {
            RaiseGameEvent?.Invoke(this, e);
        }

        public void RaiseDummyEvent()
        {
            AhpOnRaiseKinectEvent(this, new KinectArrowHitEventArgs(1));
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

            Console.WriteLine(Path.GetFullPath(CurrentSong.SongFile).ToString());
            Mp3FileReader reader = new Mp3FileReader(Path.GetFullPath(CurrentSong.SongFile));
            _waveOut = new WaveOutEvent();
            _waveOut.Init(reader);
            _waveOut.PlaybackStopped += WaveOutOnPlaybackStopped;

            remainingNotes = new List<Note>(CurrentSong.Notes);

            return CurrentSong;
        }

        private void WaveOutOnPlaybackStopped(object sender, StoppedEventArgs stoppedEventArgs)
        {

            Console.WriteLine("Playback stopped. {0}", stoppedEventArgs.ToString());
            State = GameState.SCORES;
        }

        public void Start()
        {
            if (State == GameState.IN_GAME || State == GameState.PAUSED)
            {
                throw new InvalidGameStateTransitionException("Please end a game before starting.");
            }

            if (CurrentSong == null)
            {
                throw new NoSongLoadedException("Please load a Song before starting!");
            }

            _playThread = new Thread(playSong);
            _playThread.Start();

            State = GameState.IN_GAME;

        }

        private void playSong()
        {
            Console.WriteLine("In playSong");
            _waveOut.Play();
            bool wasPaused = false;

            Console.WriteLine("Remaining notes: " + remainingNotes.Count);
            while (remainingNotes.Count > 0)
            {
                long elapsed = (long) _waveOut.GetPositionTimeSpan().TotalMilliseconds;
                CurrentTime = elapsed;
                if (State.Equals(GameState.PAUSED))
                {
                    if (!wasPaused)
                    {
                        Console.WriteLine("Game paused");
                        wasPaused = true;
                        _waveOut.Pause();
                    }
                    
                    continue;
                }

                if (wasPaused && State.Equals(GameState.IN_GAME))
                {
                    Console.WriteLine("Was paused, resuming...");
                    _waveOut.Play();
                    wasPaused = false;
                }

                Note current;
                while ((current = remainingNotes.First()).StartTime() <= elapsed -200)
                {
                   
                        Console.WriteLine("Missed note at " + current.Position());
                        OnRaiseGameEvent(new GameEventArgs(current, Accuracy.MISS, 0));
                        remainingNotes.Remove(current);
                        Console.WriteLine("Removed a note.");
                    if (remainingNotes.Count == 0)
                    {
                        break;
                    }
                    
                }
                

            }
            Console.WriteLine("After loop");
            
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
                case GameState.READY:
                    throw new InvalidGameStateTransitionException("Can not pause before starting.");
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        public void Resume()
        {
            State = GameState.IN_GAME;
        }
    }


    public enum GameState
    {
        MAIN_MENU, PAUSED, IN_GAME, LOADING_SONG, READY, OPTIONS, SCORES
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
