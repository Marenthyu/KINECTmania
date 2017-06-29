using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Microsoft.SqlServer.Server;
using NAudio.Utils;
using NAudio.Wave;
using Timer = System.Timers.Timer;

namespace KINECTmania.GameLogic
{
    class Tester
    {

        [STAThread]
        public static void Main(String[] args)
        {
            GameStateManager gms = new GameStateManager(null);

            gms.RaiseGameEvent += GmsOnRaiseGameEvent;

            gms.LoadSong("C:\\Users\\Peter Fredebold\\Downloads\\ShakeItOff.kmsf");
            gms.Start();
            Thread.Sleep(10000);
            gms.Pause();
            Thread.Sleep(5000);
            gms.Resume();
            
        }

        private static void GmsOnRaiseGameEvent(object sender, GameEventArgs gameEventArgs)
        {
            Console.WriteLine("Got event. Note: " + gameEventArgs.Note + "; Accuracy: " + gameEventArgs.Accuracy + "; Points: " + gameEventArgs.Points);
        }

        public static void playMedia()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = "mp3 file|*.mp3";
            DialogResult result = ofd.ShowDialog();
            if (result.ToString() != "OK")
            {
                Console.WriteLine("Please select a File.");
                return;
            }
            Mp3FileReader reader = new Mp3FileReader(ofd.FileName);
            WaveOut wavOut = new WaveOut();
            wavOut.Init(reader);
            wavOut.Play();
            while (true)
            {
                Console.WriteLine("Status: {0}, Time: {1}", wavOut.PlaybackState, wavOut.GetPositionTimeSpan().TotalMilliseconds.ToString());
                Thread.Sleep(1000);
            }
        }
        
    }
}
