using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KINECTmania.GameLogic;
using Timer = System.Timers.Timer;

namespace KINECTmania
{
    class FileParser
    {

        private String fileName;

        public FileParser(String fileName)
        {
            this.fileName = fileName;
        }

        public List<String> readFile()
        {
            List<String> list = new List<string>();

            if (!File.Exists(fileName))
            {
                Console.WriteLine("File not found! Create it forst, please. Path: {0}", Path.GetFullPath(fileName));
                return null;
            }
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            TextReader tr = new StreamReader(fs);
            String line;
            while ((line = tr.ReadLine()) != null)
            {
                list.Add(line);
                //Console.WriteLine(line);
            }
            tr.Close();
            fs.Close();
            return list;
        }

        

    }
}
