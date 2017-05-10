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
using Timer = System.Timers.Timer;

namespace KINECTmania.GameLogic
{
    class Tester
    {
        private static long elapsedTime = 0;
        private static List<Note> notes;
        private static List<Note>.Enumerator enumerator;
        private static Note nextNote;
        private static Song testSong;
        private static System.Timers.Timer t = new Timer(1);
        private static DateTime startTime;

        [STAThread]
        public static void Main(String[] args)
        {
            Console.WriteLine("Starting up....");
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.Filter = "KINECTmania Song Files (*.kmsf)|*.kmsf";
            DialogResult result = ofd.ShowDialog();
            if (result.ToString() != "OK")
            {
                Console.WriteLine("Please select a File.");
                return;
            }
            Console.WriteLine(ofd.FileName);

            testSong = new Song(ofd.FileName);
            Console.WriteLine(testSong.ToString());

            notes = testSong.GetNotes();
            enumerator = notes.GetEnumerator();
            enumerator.MoveNext();
            nextNote = enumerator.Current;

            t.Elapsed += (s_, e_) =>  Trigger();

            startTime = DateTime.Now;
            t.Start();
            while (t.Enabled)
            {
                
            }
        }

        public static void Trigger()
        {

            elapsedTime = (long) (DateTime.Now - startTime).TotalMilliseconds;

            while (nextNote!=null && nextNote.StartTime() < elapsedTime)
            {
                
                Console.WriteLine("Timestamp: {0}, Note: {1}", elapsedTime, nextNote);
                if (enumerator.MoveNext())
                {
                    nextNote = enumerator.Current;
                }
                else
                {
                    //t.Stop();
                    Console.WriteLine("End of Song.");
                    nextNote = null;
                    break;
                }
                
            }

            if (elapsedTime > (testSong.GetLength() * 1000))
            {
                t.Stop();
                Console.WriteLine("End of Song.");
            }
        }
    }
}
