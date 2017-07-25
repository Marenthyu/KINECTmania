using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KINECTmania.kinectProcessing
{
    /// <summary>
    /// Event mit einem short als Message, um zu signalisieren, dass ein Pfeil getroffen wurde
    /// </summary>
    public class KinectArrowHitEventArgs : EventArgs
    {
        public KinectArrowHitEventArgs(short s)
        {
            message = s;
        }
        private short message;

        public short Message
        {
            get { return message; }
            set { message = value; }
        }
    }
    /// <summary>
    /// Publisher für KinectArrowHitEvent
    /// </summary>
    public class ArrowHitPublisher
    {
        public event EventHandler<KinectArrowHitEventArgs> RaiseKinectEvent;
        public void SendEvent(short s)
        {
            OnRaiseKinectEvent(new KinectArrowHitEventArgs(s));
        }
        protected virtual void OnRaiseKinectEvent(KinectArrowHitEventArgs e)
        {
            EventHandler<KinectArrowHitEventArgs> handler = RaiseKinectEvent;
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
    /// <summary>
    /// Subscriber für KinectArrowHitEvent
    /// </summary>
    public class ArrowHitSubscriber
    {
        private string id;
        public ArrowHitSubscriber(string ID, ArrowHitPublisher pub)
        {
            id = ID;
            pub.RaiseKinectEvent += HandleKinectArrowHitEvent;
        }
        public void HandleKinectArrowHitEvent(object sender, KinectArrowHitEventArgs e)
        {
            throw new NotImplementedException();

        }

    }
}
