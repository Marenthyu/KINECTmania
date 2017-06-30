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


namespace KINECTmania.GUI
{
    
    /// <summary>
    /// Interaktionslogik für GamePage.xaml
    /// </summary>
    public partial class GamePage : Page, Menu
    {
        public event EventHandler<MenuStateChanged> RaiseMenuStateChanged;
        private System.Windows.Threading.DispatcherTimer countdownTimer;
        private int secondsBeforeGameStarts = 5;
        private Song currentSong;
        private DateTime startupDate;
        int reactiontime = -1;
        int eventsRecieved = 0;
        public static GamePage staticGamePage;

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
                startupDate = DateTime.Today;

            }
        }

        void HandleSongLoaded(object sender, SongLoaded s)
        {
            this.currentSong = s.LoadedSong;
        }

        void HandleGameOptionsSet(object sender, GameOptionsSet gs)
        {
            this.reactiontime = gs.MS;
        }

        private void countdownTimer_Tick(object sender, EventArgs e)//Implements the Countdown... nothing else, really!
        {
            countdownTimer.Stop();
            switch (secondsBeforeGameStarts) 
            {
                case 5:
                    {
                        CountdownDisplayer5.Visibility = Visibility.Visible;
                        Console.Write("Info: Game starts in 5... ");
                        break;
                    }
                case 4:
                    {
                        CountdownDisplayer5.Visibility = Visibility.Hidden;
                        CountdownDisplayer4.Visibility = Visibility.Visible;
                        Console.Write("4... ");
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
                default:
                    {
                        CountdownDisplayer1.Visibility = Visibility.Hidden;
                        Console.WriteLine("Info: Game starts now!");
                        playGame();
                        break;
                    } 
            }
            secondsBeforeGameStarts -= 1;

            countdownTimer.Interval = new TimeSpan(0, 0, 1);
            if (secondsBeforeGameStarts != -1){
                countdownTimer.Start();
            }
        }

        public virtual void OnRaiseMenuStateChanged(MenuStateChanged e)
        {
            RaiseMenuStateChanged?.Invoke(this, e);
        }

        #endregion

        private void playGame() //Called by an event handler (countdownTimer_Tick)
        {
            double startTime = (DateTime.Now - new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0, 0)).TotalMilliseconds;
            int lastNoteTriggered = -1;
            Image[] imageBuffer = new Image[20];
            while (IsLive())
            {
                if ((Now2ms() - startTime - reactiontime ) > currentSong.GetNotes().ElementAt(lastNoteTriggered).StartTime())
                {
                    new Thread(moveImageUpwards(arrowGenerator(currentSong.GetNotes().ElementAt(++lastNoteTriggered).Position()), reactiontime)).Start();
                    
                }
            }
        }

        #region <Convenience methods for playGame()>
        
        private void moveImageUpwards(Image i, int reactiontime) //reactiontime in ms
        {
            double speed = System.Windows.SystemParameters.PrimaryScreenHeight / reactiontime; // px/ms
            // solange das Event für die Position der Note noch nicht gefeurt wurde, mach bitte das:
            Canvas.SetTop(i, Canvas.GetTop(i) - speed);
            //nach dem while:
            i.Visibility = Visibility.Hidden;
            i = null;


        }
        private double Now2ms() //Converts DateTime.Now to how many ms have passed since midnight of the day the GamePage was loaded
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

        private Image arrowGenerator(short direction)
        {
            Image retVal = new Image();
            switch (direction)
            {
                case 0:
                    retVal.Source = new BitmapImage(new Uri(@"/KINECTmania/UI/res/f_up.gif"));
                    Canvas.SetLeft(retVal, 1350);
                    Canvas.SetTop(retVal, System.Windows.SystemParameters.PrimaryScreenHeight + 50);
                    retVal.Width = 100;
                    retVal.Height = 100;
                    return retVal;
                case 1:
                    retVal.Source = new BitmapImage(new Uri(@"/KINECTmania/UI/res/f_down.gif"));
                    Canvas.SetLeft(retVal, 1500);
                    Canvas.SetTop(retVal, System.Windows.SystemParameters.PrimaryScreenHeight + 50);
                    retVal.Width = 100;
                    retVal.Height = 100;
                    return retVal;
                case 2:
                    retVal.Source = new BitmapImage(new Uri(@"/KINECTmania/UI/res/f_left.gif"));
                    Canvas.SetLeft(retVal, 1650);
                    Canvas.SetTop(retVal, System.Windows.SystemParameters.PrimaryScreenHeight + 50);
                    retVal.Width = 100;
                    retVal.Height = 100;
                    return retVal;
                case 3:
                    retVal.Source = new BitmapImage(new Uri(@"/KINECTmania/UI/res/f_right.gif"));
                    Canvas.SetLeft(retVal, 1800);
                    Canvas.SetTop(retVal, System.Windows.SystemParameters.PrimaryScreenHeight + 50);
                    retVal.Width = 100;
                    retVal.Height = 100;
                    return retVal;
                default:
                    throw new Exception("Error: Wrong code for arrow specified (Method name: KINECTmania.GUI.GamePage.arrowGenerator(short direction)");
            }
            
        }
        private bool IsLive()
        {
            
            if (currentSong.GetNotes().Count <= eventsRecieved)
            {
                return false;
            }
            return true;
        }
        #endregion

        
                
    }

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
    }

    #endregion

}
