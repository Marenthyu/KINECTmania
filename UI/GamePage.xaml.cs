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
        private int secondsBeforeGameStarts;
        static GamePage staticGamePage;
        List<ArrowMover> arrowMovers;
        Song currentSong;
        int reactiontime, lastNoteStarted, dealtNotes;
        double startTime = (DateTime.Now - new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0, 0)).TotalMilliseconds;
        private BitmapImage colorBitmap = new BitmapImage();
        private int colorBitmapStride = 0;
        public ImageSource sauce { get { return colorBitmap; } }
        KinectDataInput kdi;
        public static Image DownTarget, RightTarget, UpTarget, LeftTarget;
        static List<ArrowMover> toRemove = new List<ArrowMover>();
        public static Grid TargetGrid;
        private static int accuracyDisplayRemaining = 0;

        public GamePage()
        {
            InitializeComponent();
            countdownTimer = new System.Windows.Threading.DispatcherTimer(); //This timer will realize a countdown before the game starts, similiar to a "ready, set go!" before the start of a race
            countdownTimer.Tick += new EventHandler(countdownTimer_Tick);
            countdownTimer.Interval = new TimeSpan(0, 0, 1); //Timespan(hours, minutes, seconds)
            gameoverClock = new DispatcherTimer();
            gameoverClock.Tick += new EventHandler(gameoverClock_Tick);
            gameoverClock.Interval = new TimeSpan(0, 0, 3);
            staticGamePage = this;
            DownTarget = downTarget;
            RightTarget = rightTarget;
            UpTarget = upTarget;
            LeftTarget = leftTarget;
            TargetGrid = targetGrid;
            App.Gms.RaiseGameEvent += Gms_RaiseGameEvent;
        }

        public static Canvas getKinectStreamVisualizer() //Für kinectDataInput.ImageProcessing(...)
        {
            return staticGamePage.KinectStreamVisualizer;

        }

        public static Canvas getArrowTravelLayer()
        {
            return staticGamePage.arrowTravelLayer;
        }

        #region <Event handling>

        //I have to set the publishers manually in the constructor of the GaemWindow :(

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
                //kdi = new KinectDataInput(); //TODO https://stackoverflow.com/a/13360509
                PlayGameTAP.StartupDate = DateTime.Today;
                secondsBeforeGameStarts = 5;
                startTime = (DateTime.Now - new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0, 0)).TotalMilliseconds;
                dealtNotes = lastNoteStarted = 0;
                ingameClock = new Thread(mainLoop);
                //new Thread(kdi.Start).Start();
                //colorBitmap = new WriteableBitmap((int)SystemParameters.MaximizedPrimaryScreenWidth, (int)SystemParameters.MaximizedPrimaryScreenHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
                //colorBitmapStride = (int)colorBitmap.Width * ((colorBitmap.Format.BitsPerPixel + 7) / 8);
                //colorBitmap = new BitmapImage();
                //colorBitmap.StreamSource = kdi.GetFrameStream();
                //await Task.Factory.StartNew(() => drawStream(), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.FromCurrentSynchronizationContext());


            }
        }

        void HandleSongLoaded(object sender, SongLoaded s)
        {
            PlayGameTAP.CurrentSong = s.LoadedSong;
            currentSong = s.LoadedSong;
        }

        void HandleGameOptionsSet(object sender, GameOptionsSet gs)
        {
            PlayGameTAP.Reactiontime = gs.MS;
            reactiontime = gs.MS;
        }

        async void countdownTimer_Tick(object sender, EventArgs e)//Implements the Countdown... nothing else, really!
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
                                    Canvas.SetTop(newAM.Arrow, (SystemParameters.PrimaryScreenHeight * (n.StartTime() / (double)reactiontime)) + 100);
                                    Console.WriteLine("Setting top to: {0}", (SystemParameters.PrimaryScreenHeight * (n.StartTime() / (double)reactiontime)) + 100);
                                    if (reactiontime > n.StartTime())
                                    {
                                        newAM.MovingState = 1;
                                    }
                                    arrowMovers.Add(newAM);
                                }
                                arrowTravelLayer.InvalidateVisual();
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

        private void drawStream()
        {
            
            
        }

        private void Gms_RaiseGameEvent(object sender, GameEventArgs e)
        {
            //TODO Implemeteren, wenn GamestateMEanager Event erzeugt
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
                                PlayGameTAP.Now2ms() - startTime >=
                                (currentSong.Notes[lastNoteStarted].Position() - reactiontime))
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
                                            (currentArrowMover.note.StartTime() * commonValue + 100) -
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

        void gameoverClock_Tick(object Sender, EventArgs e)
        {
            gameoverClock.Stop();
            Application.Current.Dispatcher.Invoke(new Action((() =>
            {
                lastAccuracyLabel.Visibility = Visibility.Hidden;
            })));
            //kdi.Stop();
            App.Gms.ToScores();
            OnRaiseMenuStateChanged(new MenuStateChanged(0));
        }

        public virtual void OnRaiseMenuStateChanged(MenuStateChanged e)
        {
            RaiseMenuStateChanged?.Invoke(this, e);
        }

        #endregion

    }
    #region <Convenience methods + class for playGame()>

    public static class PlayGameTAP
    {
        private static Song currentSong;
        public static Song CurrentSong
        {
            get { return currentSong; }
            set { currentSong = value; }
        }
        private static List<ArrowMover> arrowmovers = new List<ArrowMover>();
        public static List<ArrowMover> Arrowmovers
        {
            get { return arrowmovers; }
            set { arrowmovers = value; }
        }
        private static int reactiontime;
        public static int Reactiontime
        {
            get { return reactiontime; }
            set { reactiontime = value; }
        }
        private static DateTime startupDate;
        public static DateTime StartupDate
        {
            get { return startupDate; }
            set { startupDate = value; }
        }
        private static DateTime songStart;
        public static DateTime SongStart
        {
            get { return songStart; }
            set { songStart = value; }
        }
        private static int eventsRecieved = 0;
        public static int EventsRecieved
        {
            get { return eventsRecieved; }
            set { eventsRecieved = value; }
        }
        private static Canvas arrowTravelLayer;
        public static Canvas ArrowtravelLayer
        {
            get { return arrowTravelLayer; }
            set { arrowTravelLayer = value; }
        }
        private static GamePage callingInstance;
        public static GamePage CallingInstance
        {
            get { return callingInstance; }
            set { callingInstance = value; }
        }

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
            /*
            TimeSpan duration = DateTime.Now - startupDate;
            return duration.Milliseconds;
            */
        }

        public static bool IsLive(int lastNoteTriggered)
        {

            if (currentSong.Notes.Count <= eventsRecieved || currentSong.Notes.Count <= lastNoteTriggered)
            {
                return false;
            }
            return true;
        }
    }


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
            //end of arrowGenerator(short code)  
        }
    }


    #endregion


    #region Class for DrawPoint(...)
    public static class Draw2Canvas
    {
        public static Joint ScaleTo(this Joint joint, double width, double height)
        {
            joint.Position = new CameraSpacePoint
            {
                X = Scale(width, 1.0f, joint.Position.X),
                Y = Scale(height, 1.0f, -joint.Position.Y),
                Z = joint.Position.Z
            };
            return joint;
        }

        private static float Scale(double maxPixel, double maxSkeleton, float position)
        {
            float value = (float)((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));
            if (value > maxPixel)
            {
                return (float)maxPixel;
            }
            if (value < 0)
            {
                return 0;
            }
            return value;
        }

        public static void DrawPoint(this Canvas canvas, Joint joint)
        {
            //Joint tracked?
            if (joint.TrackingState == TrackingState.NotTracked) { return; }

            //Map real-world coordinates to screen pixels
            joint = joint.ScaleTo(canvas.ActualWidth, canvas.ActualHeight);

            //create WPF ellipse
            Ellipse e = new Ellipse { Width = 20, Height = 20, Fill = new SolidColorBrush(Colors.LightBlue) };

            //set Ellipse's position to where joint lies
            Canvas.SetLeft(e, joint.Position.X - e.Width / 2);
            Canvas.SetTop(e, joint.Position.Y - e.Height / 2);

            //draw Ellipse e on Canvas canvas
            canvas.Children.Add(e);
        }
    } //End of class Draw2Canvas

    #endregion

}
