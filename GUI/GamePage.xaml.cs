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
        private bool isLive = false;

        public GamePage()
        {
            InitializeComponent();
            countdownTimer = new System.Windows.Threading.DispatcherTimer(); //This timer will realize a countdown before the game starts, similiar to a "ready, set go!" before the start of a race
            countdownTimer.Tick += new EventHandler(countdownTimer_Tick);
            countdownTimer.Interval = new TimeSpan(0, 0, 1); //Timespan(hours, minutes, seconds)
        }

        #region Event handling

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

        void HandleMenuStateChanged(object sender, MenuStateChanged e)
        {
            if (e.MenuState == 3)
            {
                countdownTimer.Start();
                KinectDataInput kdi = new KinectDataInput(); //Mach mal den Kinect Stream feddich
                kdi.Start(); //starte den Kinect Stream
            }
        }

        void HandleSongLoaded(object sender, SongLoaded s)
        {
            this.currentSong = s.LoadedSong;
        }

        private void countdownTimer_Tick(object sender, EventArgs e)
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

        private void playGame() //Called by an ebent handler (countdownTimer_Tick)
        {
            while (isLive)
            {
                isLive = checkIfStillLive();
            }
        }

        private bool checkIfStillLive()
        {
            int eventsRecieved = 0;
            if (currentSong.GetNotes().Count == eventsRecieved)
            {
                return true;
            }
            return false;
        }

    }
}
