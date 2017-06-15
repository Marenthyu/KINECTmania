﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Kinect;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;

namespace KINECTmania.kinectProcessing
{
    public class kinectDataInput
    {
        public static void Main(String[] args) {
            kinectDataInput kdi = new kinectDataInput();
            kdi.Start();
        }

        private bool keepRunningArrowDetection = false;
        private bool keepRunningVideoRecording = false;
        private bool[] keepRunning;

        private KinectSensor kSensor = null;

        private BodyFrameReader bodyFrameReader = null;
        private Body[] bodies = null;
        private ColorFrameReader cfr = null;
        private byte[] colorData;
        private ColorImageFormat format;
        private WriteableBitmap wbmp;
        private BitmapSource bmpSource;
        private Stream ColorFrameOutput;


        private Joint arrowUp, arrowDown, arrowLeft, arrowRight = new Joint();
        private bool[] stillHittingLeft = new bool[4];
        private bool[] stillHittingRight = new bool[4];
        private float buttonSize = 0.3F;

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
            keepRunning[0] = keepRunningArrowDetection;
            keepRunning[1] = keepRunningVideoRecording;
            arrowUp.Position.X = 0.0F;
            arrowUp.Position.Y = 0.5F;
            arrowDown.Position.X = 0.0F;
            arrowDown.Position.Y = -0.5F;
            arrowLeft.Position.X = -0.5F;
            arrowLeft.Position.Y = 0.0F;
            arrowRight.Position.X = 0.5F;
            arrowRight.Position.Y = 0.0F;
        }

        public void Start() {
            this.keepRunningArrowDetection = true;
            while (this.keepRunningArrowDetection) {
                this.initialiseKinect();
            }
        }
        public void Stop() {
            this.keepRunningArrowDetection = false;
            if (!(keepRunning[1]))
            {
                kSensor.Close();
            }
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
      
            var fd = kSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);
            uint frameSize = fd.BytesPerPixel * fd.LengthInPixels;
            colorData = new byte[frameSize];
            format = ColorImageFormat.Bgra;
            cfr = kSensor.ColorFrameSource.OpenReader();
            cfr.FrameArrived += Cfr_FrameArrived;

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


        private bool defineArrows(short direction,Joint point)
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
                            arrowPub.SendEvent(-1);
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
                            arrowPub.SendEvent(-2);
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
                            arrowPub.SendEvent(-3);
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

        private void Cfr_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            if (e.FrameReference == null) return;

            using (ColorFrame cf = e.FrameReference.AcquireFrame())
            {
                if (cf == null) return;
                cf.CopyConvertedFrameDataToArray(colorData, format);
                var fd = cf.FrameDescription;
                
                var bytesPerPixel = (PixelFormats.Bgr32.BitsPerPixel) / 8;
                var stride = bytesPerPixel * cf.FrameDescription.Width;

                bmpSource = BitmapSource.Create(fd.Width, fd.Height, 96.0, 96.0, PixelFormats.Bgr32, null, colorData, stride);
                
                wbmp = new WriteableBitmap(bmpSource);
            }
        }
    }
}
