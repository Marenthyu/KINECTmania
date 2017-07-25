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
using Microsoft.Win32; //Für OpenFileDialog
using KINECTmania.GameLogic;

namespace KINECTmania.GUI

   
{
    
    /// <summary>
    /// Interaktionslogik für GameOptionsMenu.xaml
    /// </summary>
    public partial class GameOptionsMenu : Page, Menu, KinectStreamRequestedPublisher, GameOptionsSetter, SongLoadedPublisher
    {
        public event EventHandler<MenuStateChanged> RaiseMenuStateChanged;
        public event EventHandler<KinectStreamRequested> RaiseKinectStreamRequested; //kinectDataInput hat schon eine Methode, die mir einen byte[]-Stream zurückgibt. Besser die nehmen.
        public event EventHandler<GameOptionsSet> RaiseGameOptionsSet;
        public event EventHandler<SongLoaded> RaiseSongLoaded;
        public GameOptionsMenu()
        {
            InitializeComponent();

        }

        #region <Event handling>

        public virtual void OnRaiseMenuStateChanged(MenuStateChanged e)
        {
            RaiseMenuStateChanged?.Invoke(this, e);
        }

        public virtual void OnRaiseKinectStreamRequested(KinectStreamRequested k)
        {
            RaiseKinectStreamRequested?.Invoke(this, k);
        }

        public virtual void OnRaiseGameOptionsSet(GameOptionsSet g)
        {
            RaiseGameOptionsSet?.Invoke(this, g);
        }

        public virtual void OnRaiseSongLoaded(SongLoaded s)
        {
            RaiseSongLoaded?.Invoke(this, s);
        }

        #endregion

        private void BackToMainMenu_Click(object sender, RoutedEventArgs e)
        {
            OnRaiseMenuStateChanged(new MenuStateChanged(0));
        }

        private void StartGameBtn_Click(object sender, RoutedEventArgs e)
        {
            OnRaiseGameOptionsSet(new GameOptionsSet((int) ReactionTimeChanger.Value));
            OnRaiseKinectStreamRequested(new KinectStreamRequested());
            OnRaiseMenuStateChanged(new MenuStateChanged(3));

        }

        private void OpenSong(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "KINECTMania Song Files (*.kmsf)|*.kmsf|All files (*.*)|*.*";
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (ofd.ShowDialog() == true) //is only true if user selects "Open" in the dialog
            { 
                Console.WriteLine("Info: Song " + ofd.FileName + " successfully loaded!");
                FileLocationMeasurer.Text = ofd.FileName;
                this.StartGameBtn.IsEnabled = true;
                ReactionTimeChanger.IsEnabled = true;
                
                Song loaded = App.Gms.LoadSong(ofd.FileName);
                OnRaiseSongLoaded(new SongLoaded(loaded));
            }
        }

        private void ReactionTimeChanger_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ReactionTimeChanger.Value % 1000 != 0) //Damit ich immer das Format "_,_ s" habe
            {
                try
                {
                    ReactionTimeMeasurer.Content = ReactionTimeChanger.Value / 1000 + " s";
                }
                catch 
                {
                    Console.WriteLine("wat");
                }
            }
            else
            {
                try { 

                    ReactionTimeMeasurer.Content = ReactionTimeChanger.Value / 1000 + ",0 s";
                }
                catch
                {
                    Console.WriteLine("wat");
                }
            }
        }

        private void StartGameBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
