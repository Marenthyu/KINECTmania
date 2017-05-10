using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Kinect;

namespace KINECTmania
{
    public class kinectDataInput
    {
        public static void Main(String [] args) {
            kinectDataInput kdi = new kinectDataInput();
            while (true) {
                refreshData();
                //throws events when theres a reason
            }
        }
        private KinectSensor kSensor = null;
        private BodyFrameReader bodyFrameReader = null;
        private Body[] bodies = null;
        private Joint arrowUp, arrowDown, arrowLeft, arrowRight = new Joint();
        private double buttonSize = 50.0;
        public kinectDataInput()
        {
            arrowUp.Position.X = 960;
            arrowUp.Position.Y = 50;
            arrowDown.Position.X = 960;
            arrowDown.Position.Y = 1030;
            arrowLeft.Position.X = 50;
            arrowLeft.Position.Y = 540;
            arrowRight.Position.X = 1870;
            arrowRight.Position.Y = 540;

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
            Console.WriteLine("Kinect open");
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
        private int buttonHit(Joint handJoint)
        {
            int buttonNumber = -1;
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
            catch (NullReferenceException e)
            {
                Console.WriteLine("At least one Button wasn`t defined!" + e.Message);
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
        public static void refreshData()
        {
            //this function will refresh everything in our Lifes

        }
    }

}
