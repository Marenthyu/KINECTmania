using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;

public class kinectDataInput
{
    KinectSensor kSensor = null;
    BodyFrameReader bodyFrameReader = null;
    Body[] bodies = null;
	public kinectDataInput()
	{
        initialiseKinect();

	}

    public void initialiseKinect(){
        kSensor = KinectSensor.GetDefault();
        if(kSensor!=null){
            //starts the Kinect
            kSensor.Open();
        }
        Console.WriteLine("Kinect open");
        bodyFrameReader = kSensor.BodyFrameSource.OpenReader();

        if(bodyFrameReader != null){
            bodyFrameReader.FrameArrived += Reader_FrameArrived;
        }
        
    }
    private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e){
        bool dataReceived = false;
        using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame()){
            if(bodyFrame != null){
                if(bodies == null){
                    bodies = new Body[bodyFrame.BodyCount];
                }
                
            }
            bodyFrame.GetAndRefreshBodyData(bodies);
            dataReceived = true;
        }
        if(dataReceived){
            foreach(Body body in bodies){
                if(body.IsTracked){
                    IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
                    Dictionary<JointType, Point> jointPoints =new Dictionary<JointType, Point>();
                    Joint rightHandJoint = joints[JointType.HandRight];
                    Joint leftHandJoint = joints[JointType.HandLeft];
                    Console.WriteLine(rightHandJoint.Position.X + rightHandJoint.Position.Y);
                    Console.WriteLine(leftHandJoint.Position.X + leftHandJoint.Position.Y);
                }
            }
        }
    }
/*    public static void Main(String[] args){
        kinectDataInput kdi = new kinectDataInput();
        while (true)
        {
            
        }
    }*/
}
