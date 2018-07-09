using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;

namespace Kinect_Gesture_to_OSC
{
    class GestureDetector
    {
        // Class for loading gestures and creating objects from them

        //private static List<Gestures> Gesture_Library = new List<Gestures>(); //gesture library (list of all created gestures)
        private readonly string type1_database = @"Gestures\Type 1.gbd"; // Dummy Database
        //private readonly string type2_database = @"Gestures\Type 1_example.gbd"; << NOT ACTIVE YET
        //private readonly string type3_database = @"Gestures\Type 1_example.gbd"; << NOT ACTIVE YET

        Gesture gesture_history = null;
        int result_score = 0; //score to get enough time to trigger osc message (10 frames = 1 third of second)
        bool result_cooldown = true; //cooldown for result score timer

        /// <summary> Gesture frame source which should be tied to a body tracking ID </summary>
        private VisualGestureBuilderFrameSource vgbFrameSource = null;

        /// <summary> Gesture frame reader which will handle gesture events coming from the sensor </summary>
        private VisualGestureBuilderFrameReader vgbFrameReader = null;


        public GestureDetector(KinectSensor sensor) //contructor
        {
            //var gesture = Gesture_Loader(gesture_name);

            this.vgbFrameSource = new VisualGestureBuilderFrameSource(sensor, 0);
            this.vgbFrameReader = this.vgbFrameSource.OpenReader();
            if (this.vgbFrameReader != null)
            {
                IsPaused = true;
                this.vgbFrameReader.FrameArrived += this.Gesture_Reader;
            }
                

            // VGB Gestures Loader
            using (VisualGestureBuilderDatabase database = new VisualGestureBuilderDatabase(this.type1_database))
            {
                // we could load all available gestures in the database with a call to vgbFrameSource.AddGestures(database.AvailableGestures), 
                this.vgbFrameSource.AddGestures(database.AvailableGestures);
                
            }
        }

        /// <summary>
        /// Gets or sets the body tracking ID associated with the current detector
        /// The tracking ID can change whenever a body comes in/out of scope
        /// </summary>
        public ulong TrackingId
        {
            get
            {
                return this.vgbFrameSource.TrackingId;
            }

            set
            {
                if (this.vgbFrameSource.TrackingId != value)
                {
                    this.vgbFrameSource.TrackingId = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the detector is currently paused
        /// If the body tracking ID associated with the detector is not valid, then the detector should be paused
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return this.vgbFrameReader.IsPaused;
            }

            set
            {
                if (this.vgbFrameReader.IsPaused != value)
                {
                    this.vgbFrameReader.IsPaused = value;
                }
            }
        }


        private void Gesture_Reader(object sender, VisualGestureBuilderFrameArrivedEventArgs e)// search for a gesture in gesture library compatible to the frame arrived from the sensor
        {
            VisualGestureBuilderFrameReference frameReference = e.FrameReference;

            using (VisualGestureBuilderFrame frame = frameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    // get the discrete gesture results which arrived with the latest frame
                    IReadOnlyDictionary<Gesture, DiscreteGestureResult> discreteResults = frame.DiscreteGestureResults;

                    if (discreteResults != null)
                    {
                        // we only have one gesture in this source object, but you can get multiple gestures
                        foreach (Gesture gesture in this.vgbFrameSource.Gestures)
                        {
                            if (gesture.GestureType == GestureType.Discrete)
                            {
                                DiscreteGestureResult result = null;
                                discreteResults.TryGetValue(gesture, out result);

                                if (result.Detected && result.Confidence > 0.9) //got a discret result with a high confidence
                                {
                                    //Console.WriteLine(result_score); //DEBUG TEST SCORE

                                    //Create OSC Message HERE (?)
                                    if(gesture_history != gesture)
                                    {                      
                                        gesture_history = gesture;
                                        result_score = 1;
                                        result_cooldown = true;
                                    }
                                    else if(gesture_history == gesture)
                                    {
                                        result_score++;
                                    }

                                    if(result_score > 10 && result_cooldown)
                                    {
                                        Console.WriteLine("A pose: " + gesture.Name + " foi efetuada com sucesso.");
                                        Gesture_List_to_OSC(gesture);
                                        result_cooldown = false;
                                    }
                                    
                                }
                                else if (result.Confidence < 0.6 && gesture == gesture_history) //if a gesture saved in the history is not anymore being tracked >>> delete it, so you can track it again
                                {
                                    gesture_history = null;
                                    result_score = 0;
                                    result_cooldown = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Gesture_List_to_OSC(Gesture user_gesture) // function to compare which gesture got triggered and create a OSC Message based on it
        {

        }
    }
}
