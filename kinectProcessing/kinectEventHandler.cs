using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KINECTmania.kinectDataInput
{
    class kinectEventHandler     
    {
        private String events {
            get { return events; }
            set { events = value; }
        }
        public kinectEventHandler() {
            
        }
        public kinectEventHandler(String events) {
            this.events = events;
        }
        public void throwEvent() {
            while (true)
                if (events == "UP") { Console.WriteLine("UP"); }
                else
                {
                    if (events == "DOWN") { Console.WriteLine("DOWN"); }
                    else
                    {
                        if (events == "LEFT") { Console.WriteLine("LEFT"); }
                        else
                        {
                            if (events == "RIGHT") { Console.WriteLine("RIGHT"); }
                        }
                    }
                }
        }
    }
}
