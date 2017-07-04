using System;
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
    public partial class GamePage : Page, Menu, CountdownFinishedPublisher
    {
        public event EventHandler<MenuStateChanged> RaiseMenuStateChanged;
        public event EventHandler<CountdownFinished> RaiseCountdownFinished;
        private System.Windows.Threading.DispatcherTimer countdownTimer;
        private int secondsBeforeGameStarts = 5;
        static GamePage staticGamePage;

        public GamePage()
        {
            InitializeComponent();
            countdownTimer = new System.Windows.Threading.DispatcherTimer(); //This timer will realize a countdown before the game starts, similiar to a "ready, set go!" before the start of a race
            countdownTimer.Tick += new EventHandler(countdownTimer_Tick);
            countdownTimer.Interval = new TimeSpan(0, 0, 1); //Timespan(hours, minutes, seconds)
            staticGamePage = this;
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
                //    kinectDataInput kdi = new kinectDataInput();      UNCOMMENT ME WHEN
                //    kdi.Start();                                      WE GOT A KINECT
                //    kdi.Stop();                                       PLUGGED IN
                PlayGameTAP.StartupDate = DateTime.Today;

            }
        }

        void HandleSongLoaded(object sender, SongLoaded s)
        {
            PlayGameTAP.CurrentSong = s.LoadedSong;
        }

        void HandleGameOptionsSet(object sender, GameOptionsSet gs)
        {
            PlayGameTAP.Reactiontime = gs.MS;
        }

        void HandleCountdownFinished(object sender, CountdownFinished CdownFin)
        {
            //Thread t = new Thread(new ThreadStart(PlayGame));
            //t.SetApartmentState(ApartmentState.STA);
            //t.Start();

            //PlayGame();
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
                        CountdownDisplayer1.Visibility = Visibility.Hidden;
                        KinectStreamVisualizer.InvalidateVisual();

                        
                        await Task.Factory.StartNew(() => {
                            PlayGameTAP.CallingInstance = this;
                            PlayGameTAP.ArrowtravelLayer = this.arrowTravelLayer;
                            PlayGameTAP.PlayGame(); 
                            }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.FromCurrentSynchronizationContext());

                        break;
                    }
            }




            if (secondsBeforeGameStarts > 0)
            {
                countdownTimer.Interval = new TimeSpan(0, 0, 1);
                countdownTimer.Start();
            }
        }

        public virtual void OnRaiseMenuStateChanged(MenuStateChanged e)
        {
            RaiseMenuStateChanged?.Invoke(this, e);
        }

        public virtual void OnRaiseCountdownFinished(CountdownFinished CdownFin)
        {
            RaiseCountdownFinished?.Invoke(this, CdownFin);
        }

        #endregion

    }

    
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

        
        public static void PlayGame() //Called by an event handler (countdownTimer_Tick)
        {

            double startTime = (DateTime.Now - new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0, 0)).TotalMilliseconds;
            int lastNoteStarted = 0;
            ArrowMover currentArrowMover;

            foreach (Note n in currentSong.GetNotes())
            {
                currentArrowMover = new ArrowMover(currentSong.GetNotes().ElementAt(lastNoteStarted).Position(), reactiontime);
                arrowmovers.Add(currentArrowMover);
                arrowTravelLayer.Children.Add(currentArrowMover.Arrow);
                if (reactiontime < currentSong.GetNotes()[lastNoteStarted].StartTime())
                {
                    Canvas.SetTop(currentArrowMover.Arrow, currentArrowMover.Speed * (reactiontime / currentSong.GetNotes()[lastNoteStarted].StartTime()));
                    currentArrowMover.MovingState = 1;
                }
            }
            while (IsLive(lastNoteStarted))
            {
                if ((currentSong.GetNotes().Count > lastNoteStarted) && (Now2ms() - startTime - reactiontime) >= currentSong.GetNotes().ElementAt(lastNoteStarted).StartTime()) //Wenn die Zeit ran ist, den Pfeil zu starten - Detaillierte Erklrung gibts in der SummaryDE.txt
                { //Pfeile hinzufügen
                    currentArrowMover = arrowmovers[lastNoteStarted];
                    currentArrowMover.Arrow.Visibility = Visibility.Visible;
                    currentArrowMover.MovingState = 1;
                    Console.WriteLine("Info: next ArrowMover on it's way! @ " + DateTime.Now.ToString());
                    moveImageUpwards(currentArrowMover);
                    arrowTravelLayer.InvalidateVisual();
                    lastNoteStarted++;
                }
                else
                {
                    currentArrowMover = null;
                }

                for (int i = 0; i < arrowmovers.Count; i++)
                {
                    if (currentArrowMover?.MovingState == 2)
                    {
                        arrowTravelLayer.Children.Remove(currentArrowMover.Arrow);
                        arrowmovers.RemoveAt(i);
                        Console.WriteLine("    Info: Removed an arrow @ " + DateTime.Now.ToString());
                    }
                    if (currentArrowMover?.MovingState == 1)
                    {
                        moveImageUpwards(arrowmovers[i]);
                    }
                }

                arrowTravelLayer.InvalidateVisual();
                //Thread.Sleep(1);
            }
            Console.WriteLine("Info: Game over!");
            //callingInstance.OnRaiseMenuStateChanged(new MenuStateChanged(0));


        } //end of PlayGame() method

        private static void moveImageUpwards(ArrowMover am)
        {
            if (Canvas.GetTop(am.Arrow) > -100 && am.MovingState == 1)
            {
                Canvas.SetTop(am.Arrow, Canvas.GetTop(am.Arrow) - am.Speed);
                arrowTravelLayer.InvalidateVisual();
            }
            else { am.MovingState = 2; }

        }

        private static double Now2ms() //Converts DateTime.Now to how many ms have passed since midnight of the day the GamePage was loaded
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

        private static bool IsLive(int lastNoteTriggered)
        {

            if (currentSong.GetNotes().Count <= eventsRecieved || currentSong.GetNotes().Count <= lastNoteTriggered)
            {
                return false;
            }
            return true;
        }
    }



    #region <Convenience methods + class for playGame()>

    public class ArrowMover
    {
        int code;
        Image arrow;
        public Image Arrow { get { return arrow; } }
        int movingState = 0; //0: Remain at the bottom | 1: Moving up | 2: Reached top
        double speed;
        public double Speed { get { return speed; } }
        public int MovingState
        {
            get { return movingState; }
            set { movingState = value; }
        }
        public ArrowMover(int code, int reactiontime)
        {
            this.code = code;
            this.arrow = arrowGenerator((short)code);
            speed = System.Windows.SystemParameters.PrimaryScreenHeight / reactiontime; //in px/ms
        }
        private void destroyME()
        {
            arrow.Visibility = Visibility.Hidden;
            arrow = null;
            code = 0;
        }
        private static Image arrowGenerator(short direction)
        {
            Image retVal = new Image();
            switch (direction)
            {
                case 1:
                    retVal.Source = new BitmapImage(new Uri(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"/UI/res/f_up.gif", UriKind.Absolute));
                    Canvas.SetLeft(retVal, 1350);
                    Canvas.SetTop(retVal, System.Windows.SystemParameters.PrimaryScreenHeight - 1050);
                    retVal.Width = 100;
                    retVal.Height = 100;
                    retVal.Visibility = Visibility.Visible;

                    return retVal;
                case 2:
                    retVal.Source = new BitmapImage(new Uri(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"/UI/res/f_down.gif", UriKind.Absolute));
                    Canvas.SetLeft(retVal, 1500);
                    Canvas.SetTop(retVal, System.Windows.SystemParameters.PrimaryScreenHeight - 1050);
                    retVal.Width = 100;
                    retVal.Height = 100;
                    retVal.Visibility = Visibility.Visible;
                    return retVal;
                case 3:
                    retVal.Source = new BitmapImage(new Uri(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"/UI/res/f_left.gif", UriKind.Absolute));
                    Canvas.SetLeft(retVal, 1650);
                    Canvas.SetTop(retVal, System.Windows.SystemParameters.PrimaryScreenHeight - 1050);
                    retVal.Width = 100;
                    retVal.Height = 100;
                    retVal.Visibility = Visibility.Visible;
                    return retVal;
                case 4:
                    retVal.Source = new BitmapImage(new Uri(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + @"/UI/res/f_right.gif", UriKind.Absolute));
                    Canvas.SetLeft(retVal, 1800);
                    Canvas.SetTop(retVal, System.Windows.SystemParameters.PrimaryScreenHeight - 1050);
                    retVal.Width = 100;
                    retVal.Height = 100;
                    retVal.Visibility = Visibility.Visible;
                    return retVal;
                default:
                    throw new Exception("Error: Wrong code for arrow specified (Method name: KINECTmania.GUI.GamePage.arrowGenerator(short direction)");
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