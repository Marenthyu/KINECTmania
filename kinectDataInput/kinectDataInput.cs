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
                Console.In.Read;
            }
        }
        private KinectSensor kSensor = null;
        private BodyFrameReader bodyFrameReader = null;
        private Body[] bodies = null;
        private Joint arrowUp, arrowDown, arrowLeft, arrowRight = new Joint();
        private double buttonSize = 50.0;
        public kinectDataInput()
        {
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
                    }
                    if (calDistance(handJoint, arrowDown) < buttonSize)
                    {
                        buttonNumber = 2;
                    }
                    if (calDistance(handJoint, arrowLeft) < buttonSize)
                    {
                        buttonNumber = 3;
                    }
                    if (calDistance(handJoint, arrowRight) < buttonSize)
                    {
                        buttonNumber = 4;
                    }
                }
                return buttonNumber;

            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("At least one Button wasn`t defined!" + e.Message);
                return hit;
            }
        }
        private double calDistance(Joint hand, Joint button)
        {
            double distance = -1.0;
            //TODO calculation missing

            return distance;
        }
    }
}
