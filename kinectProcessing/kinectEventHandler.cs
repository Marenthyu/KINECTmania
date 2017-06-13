using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KINECTmania.kinectDataInput
{
    public class KinectEventArgs : EventArgs  
    {
        public KinectEventArgs(short s) {
            message = s;
        }
        private short message;

        public short Message {
            get {return message; }
            set {message = value; }
        } 
    }
    class Publisher{
        public event EventHandler<KinectEventArgs> RaiseKinectEvent;
        public void SendEvent(short s) {
            OnRaiseKinectEvent(new KinectEventArgs(s));
        }
        protected virtual void OnRaiseKinectEvent(KinectEventArgs e) {
            EventHandler<KinectEventArgs> handler = RaiseKinectEvent;
            if (handler != null) {
                handler(this, e);
            } else {
                Console.WriteLine("No Subs");
            }
            
        }
    }
    class Subscriber {
        private string id;
        public Subscriber(string ID, Publisher pub) {
            id = ID;
            pub.RaiseKinectEvent += HandleKinectEvent;
        }
        void HandleKinectEvent(object sender, KinectEventArgs e) {
            Console.WriteLine("Did it!");

        }
        
    }
}
