using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Kinect;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using KINECTmania.GUI;
using System.Threading;
using System.Windows.Shapes;

namespace KINECTmania.kinectProcessing
{
    /// <summary>
    /// Klasse, die die Verwaltung und Datenverarbeitung f�r die Kinect sicherstellt
    /// </summary>
    public class KinectDataInput : BitmapGenerator
    {
        #region Globale Variablen
        private KinectSensor kSensor = null;
        private Body[] bodies = null;
        private bool keepRunning = false;
        private Joint arrowUp, arrowDown, arrowLeft, arrowRight = new Joint();
        private bool[] stillHittingLeft = new bool[4];
        private bool[] stillHittingRight = new bool[4];
        private Joint LeftHand, RightHand = new Joint();
        private static float buttonSize = 0.3F;
        private MultiSourceFrameReader multiSource = null;
        private System.Windows.Controls.Canvas canvas;
        public event EventHandler<BitmapGenerated> RaiseBitmapGenerated;
        public static ArrowHitPublisher arrowPub = new ArrowHitPublisher();
        #endregion
        /// <summary>
        /// Defaultkonstruktor
        /// </summary>
        public KinectDataInput()
        {
            for (int i = 0; i < stillHittingLeft.Length; i++)
            {
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

        #region Event handling

        public void OnRaiseBitmapGenerated(BitmapGenerated b)
        {
            RaiseBitmapGenerated?.Invoke(this, b);
        }
        private void MultiSource_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            //Console.WriteLine("Frame arrived");
            var frame = e.FrameReference.AcquireFrame();
            if (frame != null)
            {
                var bodyFrame = frame.BodyFrameReference.AcquireFrame();
                if (bodyFrame != null)
                {
                    ArrowDetection(bodyFrame);
                    bodyFrame.Dispose();
                }
                var colorFrame = frame.ColorFrameReference.AcquireFrame();
                if (colorFrame != null)
                {
                    Imageprocessing(colorFrame);
                    colorFrame.Dispose();
                }
            }
        }

        #endregion
        /// <summary>
        /// Startfunktion der Kinect, die auch die Initialisierung �berpr�ft
        /// </summary>
        #region Kinectmanaging
        public void Start()
        { 
            if (kSensor == null || multiSource == null) { InitialiseKinect(); }
            //if (canvas == null) { canvas = GUI.GamePage.KinectStreamVisualizer; }
            kSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            if (keepRunning != true)
            {
                keepRunning = true;
                multiSource.MultiSourceFrameArrived += MultiSource_MultiSourceFrameArrived;
            }


        }
        /// <summary>
        /// Funktion zum Beenden der Kinect und der Bild�bertragung
        /// </summary>
        public void Stop()
        {
            if (keepRunning != false)
            {
                keepRunning = false;
                multiSource.Dispose();
                kSensor.Close();
            }

        }
        public bool IsRunning() { return this.keepRunning; }
        /// <summary>
        /// Funktion zum erstmaligen starten der Kinect
        /// </summary>
        private void InitialiseKinect()
        {
            if (canvas == null)
            {
                canvas = GamePage.getKinectStreamVisualizer();
            }
            kSensor = KinectSensor.GetDefault();
            if (kSensor != null)
            {
                //starts the Kinect
                kSensor.Open();
                Console.WriteLine($"Available: {kSensor.IsAvailable}");
                Console.WriteLine($"UniqueKinectId: {kSensor.UniqueKinectId}");
            }
            multiSource = kSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.Color);
        }
        #endregion
        #region Datenverarbeitung der Kinect
        /// <summary>
        /// Managed die Personen im Bild und stellt Sie der Verarbeitung zur Verf�gung
        /// </summary>
        /// <param name="bodyFrame"></param>
        private void ArrowDetection(BodyFrame bodyFrame)
        {
            bool dataReceived = false;
            
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
                        IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
                        Dictionary<JointType, System.Windows.Point> jointPoints = new Dictionary<JointType, System.Windows.Point>();
                        short rHit = ButtonHit(joints[JointType.HandRight], stillHittingRight);
                        short lHit = ButtonHit(joints[JointType.HandLeft], stillHittingLeft);
                        this.LeftHand = joints[JointType.HandLeft];
                        this.RightHand = joints[JointType.HandRight];
                        switch (rHit)
                        {
                            case 1:
                                if (!(stillHittingRight[0]))
                                {
                                    //UP
                                    arrowPub.SendEvent(1);
                                    stillHittingRight[0] = true;
                                }
                                break;
                            case 2:
                                if (!(stillHittingRight[1]))
                                {
                                    //DOWN
                                    arrowPub.SendEvent(2);
                                    stillHittingRight[1] = true;
                                }
                                break;
                            case 3:
                                if (!(stillHittingRight[2]))
                                {
                                    //LEFT
                                    arrowPub.SendEvent(3);
                                    stillHittingRight[2] = true;
                                }
                                break;
                            case 4:
                                if (!(stillHittingRight[3]))
                                {
                                    //RIGHT
                                    arrowPub.SendEvent(4);
                                    stillHittingRight[3] = true;
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
        public void DefineArrowsRightHand(short s)
        {
            Joint p = this.RightHand;
            DefineArrows(s, p);
        }

        public void DefineArrowsLeftHand(short s)
        {
            Joint p = this.LeftHand;
            DefineArrows(s, p);
        }

        private bool DefineArrows(short direction, Joint point)
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
        /// <summary>
        /// Pr�ft ob einer der vorgegebenen Bereiche mit den H�nden getroffen wurde
        /// </summary>
        /// <param name="handJoint"></param>
        /// <param name="stillHitting"></param>
        /// <returns></returns>
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
                    else
                    {
                        if (stillHitting[0] == true)
                        {
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

        /// <summary>
        /// Berechnet die Distanz zwischen 2 Punkten
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        private double CalDistance(Joint hand, Joint button)
        {
            double xHelp, yHelp = 0.0;
            if ((hand.Position.X - button.Position.X) >= 0)
            {
                xHelp = hand.Position.X - button.Position.X;
            }
            else
            {
                xHelp = button.Position.X - hand.Position.X;
            }
            if ((hand.Position.Y - button.Position.Y) >= 0)
            {
                yHelp = hand.Position.Y - button.Position.Y;
            }
            else
            {
                yHelp = button.Position.Y - hand.Position.Y;
            }
            double distance = Math.Sqrt((Math.Pow(xHelp, 2.0) + (Math.Pow(yHelp, 2.0))));
            return distance;
        }

        public static Joint ScaleTo(Joint joint, double width, double height)
        {
            joint.Position = new CameraSpacePoint
            {
                X = Scale(width, 1.0f, joint.Position.X),
                Y = Scale(height, 1.0f, -joint.Position.Y),
                Z = joint.Position.Z
            };
            return joint;
        }


        private static float Scale(double maxPixel, double maxSkeleton, float position)
        {
            float value = (float)((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));
            if (value > maxPixel)
            {
                return (float)maxPixel;
            }
            if (value < 0)
            {
                return 0;
            }
            return value;
        }

        public static WriteableBitmap DrawPoint(WriteableBitmap wbmp, Joint joint, bool isArrow)
        {

            //Map real-world coordinates to screen pixels
            joint = ScaleTo(joint, wbmp.Width, wbmp.Height);

            //create WPF ellipse
            Ellipse e = new Ellipse { Width = 20, Height = 20, Fill = new SolidColorBrush(Colors.LightBlue) };

            //set Ellipse's position to where joint lies
            if (!isArrow)
            {
                int X = (int)joint.Position.X;
                int Y = (int)joint.Position.Y;
                int helpx = wbmp.PixelWidth / 2;
                int helpy = wbmp.PixelHeight / 2;
                int dx = 0;
                int dy = 0;
                dx = X - helpx;
                dy = Y - helpy;
                dx = dx * -1;
                wbmp.FillEllipseCentered(X+dx/10, Y+dy/2, (int)(buttonSize * 200), (int)(buttonSize * 200), Colors.LightGreen);
            }
            else
            {
                wbmp.FillEllipseCentered((int)joint.Position.X, (int)joint.Position.Y, (int)(buttonSize * 200), (int)(buttonSize * 200), Colors.LightSkyBlue);
            }
            return wbmp;
        }

        /// <summary>
        /// Generiert Bilder, die dann �ber ein Event an den UI-Thread �bergeben werden
        /// </summary>
        /// <param name="cf"></param>
        private void Imageprocessing(ColorFrame cf)
        {
            Console.WriteLine("Image");
            int width = cf.FrameDescription.Width;
            int height = cf.FrameDescription.Height;
            PixelFormat format = PixelFormats.Bgr32;

            byte[] pixels = new byte[cf.FrameDescription.LengthInPixels * 4];

            if (cf.RawColorImageFormat == ColorImageFormat.Bgra)
            {
                cf.CopyRawFrameDataToArray(pixels);
            }
            else
            {
                cf.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);
            }

            int stride = width * format.BitsPerPixel / 8;
            BitmapSource bmpSource = BitmapSource.Create(width, height, 96.0, 96.0, format, null, pixels, stride);
            WriteableBitmap wbmp = new WriteableBitmap(bmpSource);

            //Bearbeiten von Bitmap

            wbmp = DrawPoint(wbmp, this.LeftHand, false);
            wbmp = DrawPoint(wbmp, this.RightHand, false);
            wbmp = DrawPoint(wbmp, this.arrowDown, true);
            wbmp = DrawPoint(wbmp, this.arrowUp, true);
            wbmp = DrawPoint(wbmp, this.arrowLeft, true);
            wbmp = DrawPoint(wbmp, this.arrowRight, true);

            OnRaiseBitmapGenerated(new BitmapGenerated(wbmp));
        }
#endregion
    }
}
