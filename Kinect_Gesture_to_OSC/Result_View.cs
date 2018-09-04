using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Gesture_to_OSC
{
    /// <summary>
    /// Class for printing results easily in the command console
    /// </summary>
    public class Result_View //Class that compiles all the useful information to show to the user.
    {
        List<string> Gestures; //list of all gestures inside the gesture lib
        bool type_2_state;
        bool type_3_state;
    }
}
