using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KINECTmania.GameLogic;
using KINECTmania.kinectProcessing;
using Microsoft.Kinect;
using System.Threading;
using System.IO;
using System.Windows.Threading;


namespace KINECTmania.GUI
{

    /// <summary>
    /// Interaktionslogik für GamePage.xaml
    /// </summary>
    public partial class GamePage : Page, Menu 
    {
        public event EventHandler<MenuStateChanged> RaiseMenuStateChanged;
        private System.Windows.Threading.DispatcherTimer countdownTimer, gameoverClock;
        private Thread ingameClock;
        private int secondsBeforeGameStarts; //für visuellen Countdown benötigt, um richtige Zeit anzuzeigen
        List<ArrowMover> arrowMovers; //Die Pfeile, die über den Bildschirm huschen. Enthält alle gerade sichtbaren Pfeile.
        Song currentSong;
        int reactiontime, lastNoteStarted, dealtNotes;
        double startTime = (DateTime.Now - new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0, 0)).TotalMilliseconds;
        private WriteableBitmap colorBitmap;
        public ImageSource sauce { get { return colorBitmap; } } //Binding mit Image KinectStreamDisplayer in der XAML-Datei
        KinectDataInput kdi; //SW-Darstellung der Kinect-HW
        public static Image DownTarget, RightTarget, UpTarget, LeftTarget; //Binding mit den Zeilen (aka umrandete Pfeile)
        static List<ArrowMover> toRemove = new List<ArrowMover>(); //Helfer-Liste
        public static Grid TargetGrid;
        private static int accuracyDisplayRemaining = 0;
        private readonly double yOffset;
        private static DateTime startupDate;

        public GamePage()
        {
            InitializeComponent();
            countdownTimer = new System.Windows.Threading.DispatcherTimer(); //This timer will realize a countdown before the game starts, similiar to a "ready, set go!" before the start of a race
            countdownTimer.Tick += new EventHandler(countdownTimer_Tick);
            countdownTimer.Interval = new TimeSpan(0, 0, 1); //Timespan(hours, minutes, seconds)
            gameoverClock = new DispatcherTimer();
            gameoverClock.Tick += new EventHandler(gameoverClock_Tick);
            gameoverClock.Interval = new TimeSpan(0, 0, 3);
            DownTarget = downTarget;
            RightTarget = rightTarget;
            UpTarget = upTarget;
            LeftTarget = leftTarget;
            TargetGrid = targetGrid;
            App.Gms.RaiseGameEvent += Gms_RaiseGameEvent;
            yOffset = GamePage.UpTarget.TransformToVisual(GamePage.TargetGrid).Transform(new Point(0, 0)).Y;
        }

        #region <Event handling>

        //I have to set the publishers manually in the constructor of the GameWindow :(

        public void setPublisherMenuStateChanged(Menu g)
        {
            if (g != null)
            {
                g.RaiseMenuStateChanged += HandleMenuStateChanged;
            }
            else
            {
                throw new NullReferenceException("Error: The given menu is null!");
            }
        }

        public void setSongLoadedPublisher(SongLoadedPublisher sp)
        {
            if (sp != null)
            {
                sp.RaiseSongLoaded += HandleSongLoaded;
                Console.WriteLine("Info: Song sucessfully handed over to GamePage!");
            }
            else
            {
                throw new NullReferenceException("Error: The given SongLoadedPublisher is null!");
            }
        }

        public void setGameOptionsSetter(GameOptionsSetter gos)
        {
            if (gos != null)
            {
                gos.RaiseGameOptionsSet += HandleGameOptionsSet;
            }
            else
            {
                throw new NullReferenceException("Error: The given GameOptionsSetter is null!");
            }
        }

        //Actual Event handling

        void HandleMenuStateChanged(object sender, MenuStateChanged e)
        {
            if (e.MenuState == 3)
            {
                countdownTimer.Start();
                kdi = new KinectDataInput(); 
                kdi.RaiseBitmapGenerated += HandleBitmapGenerated;

                startupDate = DateTime.Today;
                secondsBeforeGameStarts = 5;
                startTime = (DateTime.Now - new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0, 0)).TotalMilliseconds;
                dealtNotes = lastNoteStarted = 0;
                ingameClock = new Thread(mainLoop);
                kdi.Start();
              

            }
        }

        private void HandleBitmapGenerated(object sender, BitmapGenerated e)
        {
            KinectStreamDisplay.Source = e.Bitmap;   

        }

        void HandleSongLoaded(object sender, SongLoaded s)
        {
            currentSong = s.LoadedSong;
        }

        void HandleGameOptionsSet(object sender, GameOptionsSet gs)
        {
            reactiontime = gs.MS;
        }

        /// <summary>
        /// Implements the Countdown... nothing else, really!
        /// </summary>
        async void countdownTimer_Tick(object sender, EventArgs e)
        {
            countdownTimer.Stop();
            switch (--secondsBeforeGameStarts)
            {
                case 4:
                    {
                        CountdownDisplayer5.Visibility = Visibility.Hidden;
                        CountdownDisplayer4.Visibility = Visibility.Visible;
                        Console.Write("Info: Game starts in 4... ");
                        await Task.Factory.StartNew(() =>
                            {
                                arrowMovers = new List<ArrowMover>();
                                foreach (Note n in currentSong.Notes)
                                {
                                    ArrowMover newAM = new ArrowMover(n, reactiontime, arrowTravelLayer);
                                    Canvas.SetTop(newAM.Arrow, (SystemParameters.PrimaryScreenHeight * (n.StartTime() / (double)reactiontime)) + yOffset);
                                    Console.WriteLine("Setting top to: {0}", (SystemParameters.PrimaryScreenHeight * (n.StartTime() / (double)reactiontime)) + yOffset);
                                    if (reactiontime > n.StartTime())
                                    {
                                        newAM.MovingState = 1;
                                    }
                                    arrowMovers.Add(newAM);
                                }
                                arrowTravelLayer.InvalidateVisual();
                                KinectStreamDisplay.Source = colorBitmap;
                            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.FromCurrentSynchronizationContext());
                        break;
                    }
                case 3:
                    {
                        CountdownDisplayer4.Visibility = Visibility.Hidden;
                        CountdownDisplayer3.Visibility = Visibility.Visible;
                        Console.Write("3... ");
                        break;
                    }
                case 2:
                    {
                        CountdownDisplayer3.Visibility = Visibility.Hidden;
                        CountdownDisplayer2.Visibility = Visibility.Visible;
                        Console.Write("2... ");
                        break;
                    }
                case 1:
                    {
                        CountdownDisplayer2.Visibility = Visibility.Hidden;
                        CountdownDisplayer1.Visibility = Visibility.Visible;
                        Console.WriteLine("1... ");
                        break;

                    }
                case 0:
                    {
                        await Task.Factory.StartNew(() => { CountdownDisplayer1.Visibility = Visibility.Hidden; }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext());
                        ingameClock.Start();
                        App.Gms.Start();                       
                        break;
                    }
            }
            if (secondsBeforeGameStarts > 0)
            {
                countdownTimer.Interval = new TimeSpan(0, 0, 1);
                countdownTimer.Start();
            }
        }

        /// <summary>
        /// Triggered, whenever the gms fires an event (i.e. whenever a note was either hit or missed)
        /// </summary>
        private void Gms_RaiseGameEvent(object sender, GameEventArgs e)
        {
            Note eventNote = e.Note;
            foreach (ArrowMover a in arrowMovers)
            {
                if (a.note.Equals(eventNote))
                {
                    dealtNotes++;
                    toRemove.Add(a);
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        a.destroyME();
                        pointsLabel.Content = (Int32.Parse(pointsLabel.Content.ToString()) + e.Points).ToString();
                        Console.WriteLine("Added {0} points.", e.Points);
                        lastAccuracyLabel.Content = e.Accuracy.ToString("G");
                        lastAccuracyLabel.Visibility = Visibility.Visible;
                        accuracyDisplayRemaining = 50;
                    }));
                }
            }
            foreach (ArrowMover a in toRemove)
            {
                arrowMovers.Remove(a);
            }
            toRemove = new List<ArrowMover>();

        }

        /// <summary>
        /// *magic* Makes the game work. Creates, refreshes and deletes the arrows.
        /// </summary>
        void mainLoop()
        {
            
            double commonValue = SystemParameters.PrimaryScreenHeight / (double)reactiontime;
            long lastElapsed = 0;
            
            try
            {
                while (true)
                {
                    long elapsed = GameStateManager.CurrentTime;
                    //Console.WriteLine("Let's Lag!");
                    if (lastElapsed < elapsed)
                    {
                        lastElapsed = elapsed;
                        if (accuracyDisplayRemaining >= 1)
                        {
                            if (elapsed % 30 == 0)
                            {
                                accuracyDisplayRemaining--;
                                if (accuracyDisplayRemaining == 1)
                                {
                                    Application.Current.Dispatcher.Invoke(new Action((() =>
                                    {
                                        lastAccuracyLabel.Visibility = Visibility.Hidden;
                                    })));
                                }
                            }
                        }

                        ArrowMover[] l = new ArrowMover[arrowMovers.Count];
                        arrowMovers.CopyTo(l);
                        foreach (ArrowMover currentArrowMover in l)
                        {

                            if (currentArrowMover.MovingState == 0 &&
                                Now2ms() - startTime >= (currentSong.Notes[lastNoteStarted].Position() - reactiontime))
                            {
                                Console.WriteLine("Started note");
                                currentArrowMover.MovingState = 1;
                                lastNoteStarted++;
                            }
                            if (currentArrowMover.MovingState == 1)
                            {
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    try
                                    {
                                        Canvas.SetTop(currentArrowMover.Arrow,
                                            (currentArrowMover.note.StartTime() * commonValue + yOffset) -
                                            (elapsed * commonValue));
                                    }
                                    catch
                                    {
                                        Console.WriteLine("ERROR in dispatch");
                                    }
                                }));


                            }
                        }

                        if (dealtNotes == currentSong.Notes.Count)
                        {

                            Console.WriteLine("dealtNotes: {0}, count: {1}", dealtNotes, currentSong.Notes.Count);
                            Console.WriteLine("Game Over!");
                            gameoverClock.Start();
                            break;
                        }

                        if (!arrowMovers.Any())
                        {
                            break;
                        }

                    }
                }
            }
            catch
            {
                Console.WriteLine("ERROR");
            }
            Console.WriteLine("Out of loop");
        }

        /// <summary>
        /// Preparation before switching back to MainMenu (code: 0)
        /// </summary>
        void gameoverClock_Tick(object Sender, EventArgs e)
        {
            gameoverClock.Stop();
            Application.Current.Dispatcher.Invoke(new Action((() =>
            {
                lastAccuracyLabel.Visibility = Visibility.Hidden;
            })));
            kdi.Stop();
            App.Gms.ToScores();
            OnRaiseMenuStateChanged(new MenuStateChanged(0));
        }

        public virtual void OnRaiseMenuStateChanged(MenuStateChanged e)
        {
            RaiseMenuStateChanged?.Invoke(this, e);
        }

        #endregion

        public static double Now2ms() //Converts DateTime.Now to how many ms have passed since midnight of the day the GamePage was loaded
        {
            DateTime now = DateTime.Now;
            if (now.Day == startupDate.Day)
            {
                return (now.Hour * 3600000) + (now.Minute * 60000) + (now.Second * 1000) + now.Millisecond;
            }
            else //If you e.g. start the game at 11:59 PM and still wanna be able to play at 00:01 AM
            {
                return 86400000 + (now.Hour * 3600000) + (now.Minute * 60000) + (now.Second * 1000) + now.Millisecond;
            }
        }

    }
    #region <ArrowMover>

    /// <summary>
    /// Basically, this class serves adds some info to our arrow image, like its moving state (at bottom, moving, deleted) or the speed of the arrow
    /// </summary>
    public class ArrowMover
    {
        public Note note;
        Image arrow;
        public Image Arrow { get { return arrow; } set { arrow = value; } }
        int movingState = 0; //0: Remain at the bottom | 1: Moving up | 2: Reached top
        double speed;
        int? worker;
        public int? Worker
        {
            get { return worker; }
            set { worker = value; }
        }
        public double Speed { get { return speed; } }
        public int MovingState
        {
            get { return movingState; }
            set { movingState = value; }
        }
        public ArrowMover(Note note, int reactiontime, Canvas canvasWithArrows)
        {
            this.note = note;
            this.arrow = arrowGenerator((short)note.Position(), canvasWithArrows);
            speed = 30 * System.Windows.SystemParameters.PrimaryScreenHeight / reactiontime; //in px per 30 ms
        }
        public void destroyME()
        {
            try
            {
                arrow.Visibility = Visibility.Hidden;
                arrow = null;

            }
            catch { }
            finally
            {
                //note = null;
            }
        }

        /// <summary>
        /// Selects the correct arrow image and sets the correct values to it (e.g. position, speed, ...)
        /// </summary>
        /// <param name="direction">Which direction the arrow faces (1: up, 2: down, 3: left, 4: right)</param>
        /// <param name="parent">The Canvas on which the arrows shall be drawn</param>
        /// <returns></returns>
        private static Image arrowGenerator(short direction, Canvas parent)
        {
            Image retVal = new Image();
            switch (direction)
            {
                case 1:
                    retVal.Source = new BitmapImage(new Uri(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"/UI/res/f_up.gif", UriKind.Absolute));
                    parent.Children.Add(retVal);
                    Canvas.SetLeft(retVal, GamePage.UpTarget.TransformToVisual(GamePage.TargetGrid).Transform(new Point(0, 0)).X);
                    Canvas.SetTop(retVal, System.Windows.SystemParameters.PrimaryScreenHeight + 100);
                    retVal.Width = 100;
                    retVal.Height = 100;
                    retVal.Visibility = Visibility.Visible;
                    return retVal;
                case 2:
                    retVal.Source = new BitmapImage(new Uri(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"/UI/res/f_down.gif", UriKind.Absolute));
                    parent.Children.Add(retVal);
                    Canvas.SetLeft(retVal, GamePage.DownTarget.TransformToVisual(GamePage.TargetGrid).Transform(new Point(0, 0)).X);
                    Canvas.SetTop(retVal, System.Windows.SystemParameters.PrimaryScreenHeight + 100);
                    retVal.Width = 100;
                    retVal.Height = 100;
                    retVal.Visibility = Visibility.Visible;
                    return retVal;
                case 3:
                    retVal.Source = new BitmapImage(new Uri(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"/UI/res/f_left.gif", UriKind.Absolute));
                    parent.Children.Add(retVal);
                    Canvas.SetLeft(retVal, GamePage.LeftTarget.TransformToVisual(GamePage.TargetGrid).Transform(new Point(0, 0)).X);
                    Canvas.SetTop(retVal, System.Windows.SystemParameters.PrimaryScreenHeight + 100);
                    retVal.Width = 100;
                    retVal.Height = 100;
                    retVal.Visibility = Visibility.Visible;
                    return retVal;
                case 4:
                    retVal.Source = new BitmapImage(new Uri(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"/UI/res/f_right.gif", UriKind.Absolute));
                    parent.Children.Add(retVal);
                    Canvas.SetLeft(retVal, GamePage.RightTarget.TransformToVisual(GamePage.TargetGrid).Transform(new Point(0, 0)).X);
                    Canvas.SetTop(retVal, System.Windows.SystemParameters.PrimaryScreenHeight + 100);
                    retVal.Width = 100;
                    retVal.Height = 100;
                    retVal.Visibility = Visibility.Visible;
                    return retVal;
                default:
                    throw new ArgumentException("Error: Wrong code for arrow specified (Method name: KINECTmania.GUI.GamePage.arrowGenerator(short direction)");
            }
            //end of arrowGenerator(code: short)  
        }
    }


    #endregion


   

}
