using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KINECTmania.kinectProcessing
{
    public class KinectColorFrameEventArgs : EventArgs
    {
        public KinectColorFrameEventArgs(WriteableBitmap s)
        {
            message = s;
        }
        private WriteableBitmap message;

        public WriteableBitmap Message
        {
            get { return message; }
            set { message = value; }
        }
    }
    class ColorFramePublisher
    {
        public event EventHandler<KinectColorFrameEventArgs> RaiseKinectEvent;
        public void SendEvent(WriteableBitmap s)
        {
            OnRaiseKinectEvent(new KinectColorFrameEventArgs(s));
        }
        protected virtual void OnRaiseKinectEvent(KinectColorFrameEventArgs e)
        {
            EventHandler<KinectColorFrameEventArgs> handler = RaiseKinectEvent;
            if (handler != null)
            {
                handler(this, e);
            }
            else
            {
                Console.WriteLine("No Subs");
            }

        }
    }
    class ColorFrameSubscriber
    {
        private string id;
        public ColorFrameSubscriber(string ID, ColorFramePublisher pub)
        {
            id = ID;
            pub.RaiseKinectEvent += HandleKinectColorFrameEvent;
        }
        void HandleKinectColorFrameEvent(object sender, KinectColorFrameEventArgs e)
        {
            throw new NotImplementedException();

        }

    }
}
