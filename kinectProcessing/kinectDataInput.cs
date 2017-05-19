using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Kinect;
using System.Threading;
namespace KINECTmania.kinectDataInput
{
    public class kinectDataInput
    {
        public static void Main(String [] args) {
            kinectDataInput kdi = new kinectDataInput();
            bool keepRunning = true;
            while (keepRunning) {
                kdi.initialiseKinect();
            }
        }
        private KinectSensor kSensor = null;
        private BodyFrameReader bodyFrameReader = null;
        private Body[] bodies = null;
        private Joint arrowUp, arrowDown, arrowLeft, arrowRight = new Joint();
        private float buttonSize = 0.5F;
        public kinectDataInput()
        {
            kinectEventHandler keh = new kinectEventHandler();
            Thread eventSlave = new Thread(keh.throwEvent);
            arrowUp.Position.X = 0.0F;
            arrowUp.Position.Y = 0.5F;
            arrowDown.Position.X = 0.0F;
            arrowDown.Position.Y = -0.5F;
            arrowLeft.Position.X = -0.5F;
            arrowLeft.Position.Y = 0.0F;
            arrowRight.Position.X = 0.5F;
            arrowRight.Position.Y = 0.0F;

            initialiseKinect();
        }

        private void initialiseKinect()
        {
            kSensor = KinectSensor.GetDefault();
            if (kSensor != null)
            {
                //starts the Kinect
                kSensor.Open();
            }
            bodyFrameReader = kSensor.BodyFrameSource.OpenReader();

            if (bodyFrameReader != null)
            {
                bodyFrameReader.FrameArrived += Reader_FrameArrived;
            }

        }
        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (bodies == null)
                    {
                        bodies = new Body[bodyFrame.BodyCount];
                    }

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
                        Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();
                        Joint rightHandJoint = joints[JointType.HandRight];
                        Joint leftHandJoint = joints[JointType.HandLeft];
                        Console.WriteLine(rightHandJoint.Position.X + rightHandJoint.Position.Y);
                        Console.WriteLine(leftHandJoint.Position.X + leftHandJoint.Position.Y);
                        short rHit = buttonHit(rightHandJoint);
                        short lHit = buttonHit(leftHandJoint);
                        switch (rHit) {
                            case 1:
                                Console.WriteLine("UP");
                                break;
                            case 2:
                                Console.WriteLine("DOWN");
                                break;
                            case 3:
                                Console.WriteLine("LEFT");
                                break;
                            case 4:
                                Console.WriteLine("RIGHT");
                                break;
                        }
                        switch (lHit)
                        {
                            case 1:
                                Console.WriteLine("UP");
                                break;
                            case 2:
                                Console.WriteLine("DOWN");
                                break;
                            case 3:
                                Console.WriteLine("LEFT");
                                break;
                            case 4:
                                Console.WriteLine("RIGHT");
                                break;
                        }
                    }
                }
            }
        }


        private Joint defineArrows(Joint joint)
        {

            // This Function defines the Points where are the Buttons later on in the game

            Joint arrowJoint = new Joint();
            if (joint != null)
            {
                arrowJoint.Position.X = joint.Position.X;
                arrowJoint.Position.Y = joint.Position.Y;
            }
            return arrowJoint;
        }
        private short buttonHit(Joint handJoint)
        {
            short buttonNumber = -1;
            try
            {
                if (handJoint != null)
                {
                    if (calDistance(handJoint, arrowUp) < buttonSize)
                    {
                        buttonNumber = 1;
                        Console.WriteLine("Pfeil hoch");
                    }
                    if (calDistance(handJoint, arrowDown) < buttonSize)
                    {
                        buttonNumber = 2;
                        Console.WriteLine("Pfeil runter");
                    }
                    if (calDistance(handJoint, arrowLeft) < buttonSize)
                    {
                        buttonNumber = 3;
                        Console.WriteLine("Pfeil links");
                    }
                    if (calDistance(handJoint, arrowRight) < buttonSize)
                    {
                        buttonNumber = 4;
                        Console.WriteLine("Pfeil rechts");
                    }
                }
                return buttonNumber;

            }
            catch (NullReferenceException)
            {
                return buttonNumber;
            }
        }
        private double calDistance(Joint hand, Joint button)
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
            double distance = -1.0;
            double d = (Math.Pow(xHelp,2.0) + (Math.Pow(yHelp,2.0)));
            distance = Math.Sqrt(d);
            return distance;
        }
    }
}
