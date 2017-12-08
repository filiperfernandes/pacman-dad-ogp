using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using RemotingInterfaces;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization.Formatters;
using System.Net.Sockets;
using System.Net;


namespace pacman {
    public static class Client{
        public static int msec_per_round;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string [] args) {

            string url = args[0];
            msec_per_round = Int32.Parse(args[1]);
            //int num_players = Int32.Parse(args[2]);

            char[] delimiterChars = { ':', '/' };
            string[] words = url.Split(delimiterChars);

            //Setup game settings
            int port = Int32.Parse(words[4]);
            int num_players = Int32.Parse(args[2]);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(port));
        }
        public static string exe_path()
        {
            return @Environment.CurrentDirectory + "/pacman.exe";
        }
    }
}
