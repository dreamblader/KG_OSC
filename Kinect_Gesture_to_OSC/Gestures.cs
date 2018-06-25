using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Gesture_to_OSC
{
    class Gestures
    {
        // Class for loading gestures and creating objects from them

        private static List<Gestures> Gesture_Library = new List<Gestures>(); //gesture library (list of all created gestures)

        // Prototype: (gesture_name) >>> maybe use it to invoke a loader to get it by it's name (?)
        public Gestures() //contructor
        {
            //var gesture = Gesture_Loader(gesture_name);
        }

        // Prototype: (gesture_name) >>> see above [PS: change void type to the Gesture type to return the gesture]
        public void Gesture_Loader()
        {
            //return gesture;
        }

        // Prototype: (gesture_name)
        public void Gesture_Reader()// search for a gesture in gesture library compatible to the function caller
        {

        }
    }
}
