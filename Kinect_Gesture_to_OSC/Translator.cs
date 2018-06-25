using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace Kinect_Gesture_to_OSC
{
    class Translator  //class for capturing gestures and translate them into OSC messages
    {
        private KinectSensor sensor = null; // Sensor
        private Body[] bodies = null; // Multiple Bodies tracker
        private BodyFrameReader bodyFrameReader = null; // Reader for body frames
        private CoordinateMapper coodinatemapper = null; // Coordinate mapper to map one type of point to another       
        private int body_history = -1; // Maintain last number of bodies captured - used to not print the same message again if number of bodies didn't change
        private OSC_Messages osc_message = null; // final osc message -- will be created if a gesture is triggered.

        // HANDSTATES VARIABLES
        private HandState right_hand_consistency = new HandState(); // used to see if last right hand state didn't change
        private HandState left_hand_consistency= new HandState(); // used to see if last left hand state didn't change
        private int right_hand_score = 0; // this right hand "hold" time
        private int left_hand_score = 0; // this left hand "hold" time
        private bool right_cooldown = false; // cooldown so the user don't change states just by holding his hand longer
        private bool leftt_cooldown = false; // /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\ /\
        // ------------------------------------------------------------------------------------------------------------------


        public Translator() //constructor
        {    
            //setting translator Kinect variables
            this.sensor = KinectSensor.GetDefault();
            this.coodinatemapper = this.sensor.CoordinateMapper;
            this.bodyFrameReader = this.sensor.BodyFrameSource.OpenReader();

            this.sensor.IsAvailableChanged += Kinect_Status_Callback;

            bodyFrameReader.FrameArrived += this.Frame_Reader;

            sensor.Open();

            //Better use the Delegation of IsAvailableChanged function to maintain a realtime feedback of Kinect Status
            /* 
            if (sensor.IsAvailable) // kinect is plugged
            {
                Console.WriteLine("Kinect Device Connected");
                //sensor.Open();


            if (sensor.IsAvailable) // kinect is plugged
            {
                Console.WriteLine("Kinect Device Connected");
                sensor.Open();        

            }
            else
            {
                Console.WriteLine("Kinect Device NOT FOUND!");
                Console.Read();// just a "pause" for reading printed stuff
                Environment.Exit(-1);
            }

            */



        }

        /*
        public void Listener() // listen the kinect messages until it gets something.
        {
            if (!sensor.IsAvailable)
            {
                Console.WriteLine("Kinect Device Connection Dropped");
                Console.Read();// just a "pause" for reading printed stuff
                Environment.Exit(-1);
            }
            else
            {
                if (bodyFrameReader != null)
                {
                    //bodyFrameReader.FrameArrived += this.Frame_Reader;

                    bodyFrameReader.FrameArrived += this.Frame_Reader;
                }
            }
        }
        */ // DESCONTINUED

        private void Kinect_Status_Callback(object sender, IsAvailableChangedEventArgs args)
        {
            Console.WriteLine("Kinect Status: "+ args.IsAvailable.ToString());
        }


        private void Frame_Reader(object sender, BodyFrameArrivedEventArgs args)
        {
            bool datareceived = false; // check if any type of data has being received >>> Wait for a body appear on screen
            int bodies_on_screen = 0;
            Body user_body = null; // user body - only body that will be used to compare with Gesture Library. Will get Body.IsTracked from the Kinect if it is the only body on screen.

                using (BodyFrame bodyFrame = args.FrameReference.AcquireFrame())
            {
                if(bodyFrame != null)
                {

                   if(this.bodies == null)
                    {
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    bodyFrame.GetAndRefreshBodyData(this.bodies); // Get All Body Data from bodyFrame and allocate to the Body Vector
                    datareceived = true;
                }
            }

            if (datareceived) // if BodyFrame is received call this function to deal with the new data
            {
                foreach(Body body in this.bodies)
                {
                    if (body.IsTracked)
                    {
                        bodies_on_screen++;
                        user_body = body;
                    }
                }

                if(bodies_on_screen > 1 && bodies_on_screen != body_history)
                {
                    Console.WriteLine("There are "+ bodies_on_screen + " bodies captured");
                    Console.WriteLine("This application only supports 1.");
                    Console.WriteLine("Please Step Back.");
                    body_history = bodies_on_screen;
                }
                else if (bodies_on_screen == 1 && bodies_on_screen != body_history)
                {
                    Console.WriteLine("1 Body is Tracked");
                    Console.WriteLine("You are Ready to Go BABY!");
                    body_history = bodies_on_screen;
                    Gesture_Compare(user_body);
                }
            }
        }

        private void Gesture_Compare(Body user_body) // look for user_body and compare with hand states and gestures of the gesture library.
        {
            
            //osc_message = new OSC_Messages(3, 1, 2, 3, 4); <<< Osc Prototype to remember how to use it.

            if(user_body.HandRightState == right_hand_consistency)
            {
                right_hand_score++;

                if(right_hand_score > 500 && !right_cooldown) // 500 is a dummy number
                {
                    //Change Type HERE
                    Console.WriteLine("Type TRIGGER --- Just a TEST");
                    right_cooldown = true; // wait for a hand consistency drop to cooldown this movement
                }
            }
            else if (user_body.HandRightState != right_hand_consistency)
            {
                right_hand_consistency = user_body.HandRightState;
                right_hand_score = 0; //resets "hold" time
                right_cooldown = false; // restore cooldown
            }



            if(osc_message != null)
            {
                //osc_message.Send_OSC(); << Call in the end if osc message exist
            }
        }
    }
}
