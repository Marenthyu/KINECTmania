using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KINECTmania.kinectDataInput
{
    public class KinectEventArgs : EventArgs  
    {
        public KinectEventArgs(String s) {
            message = s;
        }
        private String message;

        public String Message {
            get {return message; }
            set {message = value; }
        } 
    }
    class Publisher{
        public event EventHandler<KinectEventArgs> RaiseKinectEvent;
        public void SendEvent(String s) {
            OnRaiseKinectEvent(new KinectEventArgs(s));
        }
        protected virtual void OnRaiseKinectEvent(KinectEventArgs e) {
            EventHandler<KinectEventArgs> handler = RaiseKinectEvent;
            if (handler != null) {
                e.Message += String.Format(":{0}", DateTime.Now.ToString());
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
