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

namespace KINECTmania.GUI
{
    
    /// <summary>
    /// Interaktionslogik für GamePage.xaml
    /// </summary>
    public partial class GamePage : Page, Menu
    {
        public event EventHandler<MenuStateChanged> RaiseMenuStateChanged;
        private Song currentSong;
        private System.Windows.Threading.DispatcherTimer countdownTimer;
        private int secondsBeforeGameStarts = 5;

        public GamePage()
        {
            InitializeComponent();
            countdownTimer = new System.Windows.Threading.DispatcherTimer(); //This timer will realize a countdown before the game starts, similiar to a "ready, set go!" before the start of a race
            countdownTimer.Tick += new EventHandler(countdownTimer_Tick);
            countdownTimer.Interval = new TimeSpan(0, 0, 1); //Timespan(hours, minutes, seconds)

            countdownTimer.Start();
        }

        private void countdownTimer_Tick(object sender, EventArgs e)
        {
            countdownTimer.Stop();
            switch (secondsBeforeGameStarts)
            {
                case 5:
                    {
                        CountdownDisplayer5.Visibility = Visibility.Visible;
                        break;
                    }
                case 4:
                    {
                        CountdownDisplayer5.Visibility = Visibility.Hidden;
                        CountdownDisplayer4.Visibility = Visibility.Visible;
                        break;
                    }
                case 3:
                    {
                        CountdownDisplayer4.Visibility = Visibility.Hidden;
                        CountdownDisplayer3.Visibility = Visibility.Visible;
                        break;
                    }
                case 2:
                    {
                        CountdownDisplayer3.Visibility = Visibility.Hidden;
                        CountdownDisplayer2.Visibility = Visibility.Visible;
                        break;
                    }
                case 1:
                    {
                        CountdownDisplayer2.Visibility = Visibility.Hidden;
                        CountdownDisplayer1.Visibility = Visibility.Visible;
                        break;
                    }
                default:
                    {
                        CountdownDisplayer1.Visibility = Visibility.Hidden;
                        break;
                    } 
            }
            secondsBeforeGameStarts -= 1;

            countdownTimer.Interval = new TimeSpan(0, 0, 1);
            countdownTimer.Start();
        }

        public virtual void OnRaiseMenuStateChanged(MenuStateChanged e)
        {
            RaiseMenuStateChanged?.Invoke(this, e);
        }

        public void setPublisher(SongLoadedPublisher slp)
        {
            slp.RaiseSongLoaded += HandleSongLoaded;
        }

        void HandleSongLoaded(object sender, SongLoaded s)
        {
            currentSong = s.Song;
            Console.WriteLine("Info: currentSong is set to " + s.Song.ToString());
        }
        
    }
}
