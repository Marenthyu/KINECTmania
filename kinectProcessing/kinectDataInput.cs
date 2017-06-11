using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Kinect;
using System.Threading;
namespace KINECTmania.kinectDataInput
{
    public class kinectDataInput
    {
        public static void Main(String[] args) {
            kinectDataInput kdi = new kinectDataInput();
            bool keepRunning = true;
            //keepRunnung will change be events(to start and stop the game)
            while (keepRunning) {
                kdi.initialiseKinect();
            }
            //Cleaning the reserved RAM here

        }
        private KinectSensor kSensor = null;
        private BodyFrameReader bodyFrameReader = null;
        private Body[] bodies = null;
        private Joint arrowUp, arrowDown, arrowLeft, arrowRight = new Joint();
        private bool[] stillHittingLeft = new bool[4];
        private bool[] stillHittingRight = new bool[4];
        private float buttonSize = 0.3F;
        Publisher pub = new Publisher();
        public kinectDataInput()
        {

            //kinectEventHandler keh = new kinectEventHandler();
            //Thread eventSlave = new Thread(keh.throwEvent);
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
                    bodyFrame.GetAndRefreshBodyData(bodies);
                    dataReceived = true;
                }

            }
            if (dataReceived)
            {
                foreach (Body body in bodies)
                {
                    if (body.IsTracked)
                    {
                        IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
                        Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>(); 
                        //Console.WriteLine("RIGHT: " + joints[JointType.HandRight].Position.X.ToString() + " | " + joints[JointType.HandRight].Position.Y.ToString());
                        //Console.WriteLine("LEFT: " + joints[JointType.HandLeft].Position.X.ToString() + " | " + joints[JointType.HandLeft].Position.Y.ToString());
                        short rHit = buttonHit(joints[JointType.HandRight],stillHittingRight);
                        short lHit = buttonHit(joints[JointType.HandLeft],stillHittingLeft);
                        switch (rHit) {
                            case 1:
                                if (!(stillHittingRight[0]))
                                {
                                    pub.SendEvent("UP");
                                    stillHittingRight[0] = true;
                                }
                                break;
                            case 2:
                                if (!(stillHittingRight[1]))
                                {
                                    pub.SendEvent("DOWN");
                                    stillHittingRight[1] = true;
                                }
                                break;
                            case 3:
                                if (!(stillHittingRight[2]))
                                {
                                    pub.SendEvent("LEFT");
                                    stillHittingRight[2] = true;
                                }
                                break;
                            case 4:
                                if (!(stillHittingRight[3]))
                                {
                                    pub.SendEvent("RIGHT");
                                    stillHittingRight[3] = true;
                                }
                                break;
                        }
                        switch (lHit)
                        {
                            case 1:
                                if (!(stillHittingLeft[0]))
                                {
                                    pub.SendEvent("UP");
                                    stillHittingLeft[0] = true;
                                }
                                break;
                            case 2:
                                if (!(stillHittingLeft[1]))
                                {
                                    pub.SendEvent("DOWN");
                                    stillHittingLeft[1] = true;
                                }
                                break;
                            case 3:
                                if (!(stillHittingLeft[2]))
                                {
                                    pub.SendEvent("LEFT");
                                    stillHittingLeft[2] = true;
                                }
                                break;
                            case 4:
                                if (!(stillHittingLeft[3]))
                                {
                                    pub.SendEvent("RIGHT");
                                    stillHittingLeft[3] = true;
                                }
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
        private short buttonHit(Joint handJoint, bool[] stillHitting)
        {
            short buttonNumber = -1;
            try
            {
                if (handJoint != null)
                {
                    if (calDistance(handJoint, arrowUp) < buttonSize)
                    {
                        buttonNumber = 1;

                    }
                    else {
                        if (stillHitting[0] == true) {
                            //Here will be an Event which shows that the button will no langer be touched (for a long touch)
                        }
                        stillHitting[0] = false;
                    }
                    if (calDistance(handJoint, arrowDown) < buttonSize)
                    {
                        buttonNumber = 2;
                    }
                    else
                    {
                        if (stillHitting[1] == true)
                        {
                            //Here will be an Event which shows that the button will no langer be touched (for a long touch)
                        }
                        stillHitting[1] = false;
                    }
                    if (calDistance(handJoint, arrowLeft) < buttonSize)
                    {
                        buttonNumber = 3;
                    }
                    else
                    {
                        if (stillHitting[2] == true)
                        {
                            //Here will be an Event which shows that the button will no langer be touched (for a long touch)
                        }
                        stillHitting[2] = false;
                    }
                    if (calDistance(handJoint, arrowRight) < buttonSize)
                    {
                        buttonNumber = 4;
                    }
                    else
                    {
                        if (stillHitting[3] == true)
                        {
                            //Here will be an Event which shows that the button will no langer be touched (for a long touch)
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
            double distance = Math.Sqrt((Math.Pow(xHelp, 2.0) + (Math.Pow(yHelp, 2.0))));
            return distance;
        }
    }
}
