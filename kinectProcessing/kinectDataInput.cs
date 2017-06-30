using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Kinect;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Threading.Tasks;
using KINECTmania.GUI;
using System.Windows.Controls;

namespace KINECTmania.kinectProcessing
{
    public class kinectDataInput
    {

        private KinectSensor kSensor = null;
        private Body[] bodies = null;
        private MemoryStream FrameStream;
        private bool keepRunning = false;
        private Joint arrowUp, arrowDown, arrowLeft, arrowRight = new Joint();
        private bool[] stillHittingLeft = new bool[4];
        private bool[] stillHittingRight = new bool[4];
        private Joint LeftHand, RightHand = new Joint();
        private float buttonSize = 0.3F;
        private MultiSourceFrameReader multiSource = null;

        ArrowHitPublisher arrowPub = new ArrowHitPublisher();
        public kinectDataInput()
        {
            for (int i = 0; i < stillHittingLeft.Length; i++) {
                stillHittingLeft[i] = false;
            }
            for (int i = 0; i < stillHittingRight.Length; i++)
            {
                stillHittingRight[i] = false;
            }
            arrowUp.Position.X = 0.0F;
            arrowUp.Position.Y = 0.5F;
            arrowDown.Position.X = 0.0F;
            arrowDown.Position.Y = -0.5F;
            arrowLeft.Position.X = -0.5F;
            arrowLeft.Position.Y = 0.0F;
            arrowRight.Position.X = 0.5F;
            arrowRight.Position.Y = 0.0F;
        }
        public MemoryStream GetFrameStream() { return this.FrameStream; }

        public void Start() {
            if (kSensor == null || multiSource == null) { InitialiseKinect(); }
            Console.WriteLine("Start");
            if (keepRunning != true)
            {
                keepRunning = true;
                while (keepRunning)
                {
                    if (multiSource != null)
                    {
                        multiSource.MultiSourceFrameArrived += MultiSource_MultiSourceFrameArrived;
                    }
                }
            }
        }
        public void Stop() {
            if (keepRunning != false)
            {
                keepRunning = false;
                multiSource.Dispose();
                kSensor.Close();
                FrameStream.Close();
            }

        }
        public bool IsRunning() { return this.keepRunning; }

        private void InitialiseKinect()
        {
            kSensor = KinectSensor.GetDefault();
            if (kSensor != null)
            {
                //starts the Kinect
                kSensor.Open();
                Console.WriteLine($"Available: {kSensor.IsAvailable}");
                Console.WriteLine($"UniqueKinectId: {kSensor.UniqueKinectId}");
            }
            multiSource = kSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body|FrameSourceTypes.Color);
            try
            {
                FrameStream = new MemoryStream();
            }
            catch (Exception e) { Console.WriteLine(e.Message.ToString()); }
        }

        private void MultiSource_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            Console.WriteLine("Event mitbekommen");
            if (e.FrameReference.AcquireFrame()!= null) {
                if (e.FrameReference.AcquireFrame().BodyFrameReference.AcquireFrame() != null)
                {
                    ArrowDetection(e.FrameReference.AcquireFrame().BodyFrameReference.AcquireFrame());
                }
                if (e.FrameReference.AcquireFrame().ColorFrameReference.AcquireFrame() != null)
                {
                    Imageprocessing(e.FrameReference.AcquireFrame().ColorFrameReference.AcquireFrame());
                }                
            }
        }

        private void ArrowDetection(BodyFrame bodyFrame)
        {
            bool dataReceived = false;

            Console.WriteLine("Arrow");
                if (bodyFrame != null)
                {
                    if (bodies == null)
                    {
                        bodies = new Body[bodyFrame.BodyCount];
                    }
                    bodyFrame.GetAndRefreshBodyData(bodies);
                    dataReceived = true;
                }

            
            if (dataReceived)
            {
                foreach (Body body in bodies)
                {
                    
                    if (body.IsTracked)
                    {
                        Console.WriteLine("Körper erkannt!");
                        IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
                        Dictionary<JointType, System.Windows.Point> jointPoints = new Dictionary<JointType, System.Windows.Point>(); 
                        short rHit = ButtonHit(joints[JointType.HandRight],stillHittingRight);
                        short lHit = ButtonHit(joints[JointType.HandLeft],stillHittingLeft);
                        this.LeftHand = joints[JointType.HandLeft];
                        this.RightHand = joints[JointType.HandRight];
                        switch (rHit) {
                            case 1:
                                if (!(stillHittingRight[0]))
                                {
                                    //UP
                                    arrowPub.SendEvent(1);
                                    stillHittingRight[0] = true;
                                    Console.WriteLine("UPr");
                                }
                                break;
                            case 2:
                                if (!(stillHittingRight[1]))
                                {
                                    //DOWN
                                    arrowPub.SendEvent(2);
                                    stillHittingRight[1] = true;
                                    Console.WriteLine("DOWNr");
                                }
                                break;
                            case 3:
                                if (!(stillHittingRight[2]))
                                {
                                    //LEFT
                                    arrowPub.SendEvent(3);
                                    stillHittingRight[2] = true;
                                    Console.WriteLine("LEFTr");
                                }
                                break;
                            case 4:
                                if (!(stillHittingRight[3]))
                                {
                                    //RIGHT
                                    arrowPub.SendEvent(4);
                                    stillHittingRight[3] = true;
                                    Console.WriteLine("RIGHTr");
                                }
                                break;
                        }
                        switch (lHit)
                        {
                            case 1:
                                if (!(stillHittingLeft[0]))
                                {
                                    arrowPub.SendEvent(1);
                                    stillHittingLeft[0] = true;
                                }
                                break;
                            case 2:
                                if (!(stillHittingLeft[1]))
                                {
                                    arrowPub.SendEvent(2);
                                    stillHittingLeft[1] = true;
                                }
                                break;
                            case 3:
                                if (!(stillHittingLeft[2]))
                                {
                                    arrowPub.SendEvent(3);
                                    stillHittingLeft[2] = true;
                                }
                                break;
                            case 4:
                                if (!(stillHittingLeft[3]))
                                {
                                    arrowPub.SendEvent(4);
                                    stillHittingLeft[3] = true;
                                }
                                break;
                        }
                    }
                }
            }
        }
        public void DefineArrowsRightHand(short s) {
            Joint p =  this.RightHand;
            DefineArrows(s, p);
        }

        public void DefineArrowsLeftHand(short s)
        {
            Joint p = this.LeftHand;
            DefineArrows(s, p);
        }

        private bool DefineArrows(short direction,Joint point)
        {
            bool success = false;

            // This Function defines the Points where are the Buttons later on in the game

            switch (direction)
            {
                case 1:
                    this.arrowUp.Position = point.Position;
                    success = true;
                    break;
                case 2:
                    this.arrowDown.Position = point.Position;
                    success = true;
                    break;
                case 3:
                    this.arrowLeft.Position = point.Position;
                    success = true;
                    break;
                case 4:
                    this.arrowRight.Position = point.Position;
                    success = true;
                    break;
            }
            return success;
        }
        private short ButtonHit(Joint handJoint, bool[] stillHitting)
        {
            short buttonNumber = -1;
            try
            {
                if (handJoint != null)
                {
                    if (CalDistance(handJoint, arrowUp) < buttonSize)
                    {
                        buttonNumber = 1;

                    }
                    else {
                        if (stillHitting[0] == true) {
                            arrowPub.SendEvent(-1);
                        }
                        stillHitting[0] = false;
                    }
                    if (CalDistance(handJoint, arrowDown) < buttonSize)
                    {
                        buttonNumber = 2;
                    }
                    else
                    {
                        if (stillHitting[1] == true)
                        {
                            arrowPub.SendEvent(-2);
                        }
                        stillHitting[1] = false;
                    }
                    if (CalDistance(handJoint, arrowLeft) < buttonSize)
                    {
                        buttonNumber = 3;
                    }
                    else
                    {
                        if (stillHitting[2] == true)
                        {
                            arrowPub.SendEvent(-3);
                        }
                        stillHitting[2] = false;
                    }
                    if (CalDistance(handJoint, arrowRight) < buttonSize)
                    {
                        buttonNumber = 4;
                    }
                    else
                    {
                        if (stillHitting[3] == true)
                        {
                            arrowPub.SendEvent(-4);
                        }
                        stillHitting[3] = false;
                    }
                }
                return buttonNumber;

            }
            catch (NullReferenceException)
            {
                return buttonNumber;
            }
        }
        private double CalDistance(Joint hand, Joint button)
        {
            double xHelp, yHelp = 0.0;
            if ((hand.Position.X - button.Position.X) >= 0)
            {
                xHelp = hand.Position.X - button.Position.X;
            }
            else {
                xHelp = button.Position.X - hand.Position.X;
            }
            if ((hand.Position.Y - button.Position.Y) >= 0) {
                yHelp = hand.Position.Y - button.Position.Y;
            }
            else {
                yHelp = button.Position.Y - hand.Position.Y;
            }
            double distance = Math.Sqrt((Math.Pow(xHelp, 2.0) + (Math.Pow(yHelp, 2.0))));
            return distance;
        }

        private void Imageprocessing(ColorFrame cf)
        {
            Console.WriteLine("Imageprocessing");
            int width = cf.FrameDescription.Width;
            int height = cf.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;
            /* 
            Still have to mark up the Arrows and the hands in the Image with a Canvas
            */
            byte[] pixels = new byte[cf.FrameDescription.LengthInPixels * 4];
            
            if (cf.RawColorImageFormat == ColorImageFormat.Bgra)
            {
                cf.CopyRawFrameDataToArray(pixels);
            }
            else
            {
                cf.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);
            }

            WritePixelsToStream(pixels);
            Canvas canvas = GamePage.getKinectStreamVisualizer();
            canvas.DrawPoint(this.LeftHand);
            canvas.DrawPoint(this.RightHand);
            canvas.DrawPoint(this.arrowUp);
            canvas.DrawPoint(this.arrowDown);
            canvas.DrawPoint(this.arrowLeft);
            canvas.DrawPoint(this.arrowRight);
            Console.WriteLine("Bild gesendet");

        }
        private async void WritePixelsToStream(byte[] pixels) {
            try
            {
                await FrameStream.WriteAsync(pixels, 0, pixels.Length);
            }
            catch (Exception e) { Console.WriteLine(e.Message.ToString()); }
        }
    }
}
