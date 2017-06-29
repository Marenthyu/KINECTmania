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
using NAudio.Wave;
using Timer = System.Timers.Timer;

namespace KINECTmania.GameLogic
{
    class Tester
    {

        [STAThread]
        public static void Main(String[] args)
        {
            GameStateManager gms = new GameStateManager();
            gms.RaiseGameEvent += GmsOnRaiseGameEvent;
            gms.RaiseDummyEvent();
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
        }
        
    }
}
