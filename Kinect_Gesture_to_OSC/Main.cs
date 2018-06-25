using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_Gesture_to_OSC
{
    class Main_App
    {

        static void Main()
        {
            //main software runner
            bool running = true;

            Translator main_translate = new Translator();

            while(running) 
            {
                if (Console.KeyAvailable) //get the key from a stream stack, dont block the app waiting for a key press
                {
                    ConsoleKeyInfo exit_command = Console.ReadKey(true); // key command listener

                    if (exit_command.Key == ConsoleKey.Enter)
                    {
                        running = false;
                        //Console.WriteLine("KILLER"); << DEBUG PRINT
                    }
                }               

                // run listener (transformer function) 
            }


            //OSC CLASS TEST RUNNER//

            OSC_Messages test1 = new OSC_Messages(1, 1 , 2, 3, 4);
            test1.Send_OSC(); //function call tester type 1

            OSC_Messages test2 = new OSC_Messages(2, 1, 2, 3, 4);
            test2.Send_OSC(); //function call tester type 2

            OSC_Messages test3 = new OSC_Messages(3, 1, 2, 3, 4);
            test3.Send_OSC(); //function call tester type 3

            //BAD ADRESS TEST
            /*
            OSC_Messages testf = new OSC_Messages(@"/wrong_stuff", 1, 2, 3, 4);
            test3.Send_OSC(); //function call tester type 3
            */

            Console.Read();// just a "pause" for reading printed stuff

            //END TEST RUNNER //
        }


    }
}
