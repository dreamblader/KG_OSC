using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ventuz.OSC;

namespace Kinect_Gesture_to_OSC
{
    class OSC_Messages
    {
        //class for creating OSC default messages to send to other apps.
        private int Id_track;
        private int Id_fx;
        private float Fx_value;
        private float Velocity;
        private string osc_address;

        // Load an OSC DLL and set it Objects -- VentuzOSC
        private static string ip_address = "127.0.0.1";
        private static int port_num = 9001;
        private OscBundle osc_pack = new OscBundle(); // Bundle Object -- used to hold all OSC INFO (OscElements) to be transported using the UDPWriter.
        UdpWriter udp_obj = new UdpWriter(ip_address, port_num); // Object used to Write/Send OSC Packages using UDP protocol.

        //  -- Default Package Info ---
        // Separated in 3 Types of address and constructors overloads << unused for now
        // INT - generate the addres string for OSC type using right now: \type1, \type2 n \type3 (based on the movement type section) // INT - for ID of Track that will be played // INT - for ID of FX that will be used // FLOAT - for value that will be applied in the FX // FLOAT - for value 8applied in the Velocity factor of the song //
        public OSC_Messages(int type, int id_track = -1, int id_fx = -1, float fx_value = -1, float velocity = -1) //constructor for Type 1 Command - Only Affect Tracks - << unused for now
        {
            Id_track = id_track;
            Id_fx = id_fx;
            Fx_value = fx_value;
            Velocity = velocity;
            osc_address = @"/type" + type; //OSC Address
            osc_pack.AddElement(Pack_OSC()); // Call Pack_OSC function creating the OSCElement to be added to the OSCBunble
        }

        // BETTER LET 1 CONSRUCTOR WITH THE ADDRES ON IT AND DEFAULT VALUES AT -1
        /*
        public OSC_Messages(int id_fx, float fx_value) // contructor for Type 2 Command - Only Affect FXs and they Values -
        {
            Id_fx = id_fx;
            Fx_value = fx_value;
            osc_address = "\type2"; //Address - /type2
            Pack_OSC();
        }

        public OSC_Messages(float velocity) // contructor for Type 3 Command - Only Affect Velocity -
        {
            Velocity = velocity;
            osc_address = "\type3"; //Address - /type3
            Pack_OSC();
        }
        */

        private OscElement Pack_OSC() //used to pack OSC Elements independent of the constructor (multiple constructors not used, but function still implemented for future changes)
        {
            OscElement osc_obj = new OscElement(osc_address, null);
            
            if (osc_address == @"/type1")
            {
                 osc_obj = new OscElement(osc_address, Id_track);

            }
            else
                if (osc_address == @"/type2")
            {
                 osc_obj = new OscElement(osc_address, Id_fx, Fx_value);
            }
            else
                if(osc_address == @"/type3")
            {
                 osc_obj = new OscElement(osc_address, Id_track, Velocity);
            }
            else // no real type assigned: Return Error X and kill the application
            {
                Console.WriteLine("ERROR X: No real type assigned (only supporting types: 1, 2 or 3)");
                Console.WriteLine("FOR MORE INFO READ THE GUIDE.TXT IN THIS APP ROOT DIRECTORY");
                Console.Read();// just a "pause" for reading printed stuff
                Environment.Exit(-1);
            }
            

            return osc_obj;

        }

        // Default Package is -1. This value implies that this feature will be disabled at the end software wich receive this OSC Message

        //Prototype: (messages, UDP_port) >>>> maybe more?
        public void Send_OSC() //sending function 
        {
            
            Console.WriteLine("Connection IP: " + ip_address + "  Port:" + port_num);
            Console.WriteLine("OSC Address: " + osc_address);
            Console.WriteLine("This OBJ Variables: " + Id_track + " " + Id_fx + " " + Fx_value + " " + Velocity);
            Console.WriteLine("Number of Elements in the Package: " + osc_pack.Elements.Count);
            Console.Write("\n");
            
            //troubleshoot UDP SUCESS??? (is this possible?)
            udp_obj.Send(osc_pack);
        }

        ~OSC_Messages() //destructor
        {
            udp_obj.Dispose();
        }

    }

}
