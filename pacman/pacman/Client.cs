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

            char[] delimiterChars = { ':', '/' };
            string[] words = url.Split(delimiterChars);

            //Setup game settings
            int port = Int32.Parse(words[4]);
            int num_players = Int32.Parse(args[2]);
            Dictionary<int, List<bool>> inputFile;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length>3)
            {
                inputFile = parseInputFile(args[3]);
                Application.Run(new Form1(port, inputFile, true));
            }
            else {
                inputFile = new Dictionary<int, List<bool>>() ;
                Application.Run(new Form1(port, inputFile, false));
            }


            
            
        }
        public static string exe_path()
        {
            return @Environment.CurrentDirectory + "/pacman.exe";
        }

        static public Dictionary<int, List<bool>> parseInputFile(string path)
        {
            Dictionary<int, List<bool>> movesPerRound = new Dictionary<int, List<bool>>();
            List<bool> moves = new List<bool>(new bool[4]);

            string[] fileInput = System.IO.File.ReadAllLines(path);
            char[] delimiterChars = { ',' };
            string[] words;
            int i = 0;
            foreach (string line in fileInput)
            {
                moves = new List<bool>(new bool[4]);
                Console.WriteLine(line);
                words = line.Split(delimiterChars);

                moves[0] = false;
                moves[1] = false;
                moves[2] = false;
                moves[3] = false;
                if (words[1].Equals("LEFT")) { moves[0] = true; }
                else if (words[1].Equals("RIGHT")) { moves[1] = true; }
                else if (words[1].Equals("UP")) { moves[2] = true; }
                else if (words[1].Equals("DOWN")) { moves[3] = true; }

                //Console.WriteLine("This is round: " + words[0] + "And the move is: " + words[1]);
                //Console.WriteLine("0:" + moves[0] + " 1:" + moves[1] + " 2:" + moves[2] + "3:" + moves[3]);

                movesPerRound.Add(Int32.Parse(words[0]), moves);

            }
            //foreach (var item in movesPerRound)
            //{
            //    Console.WriteLine(item.Key + " " + item.Value[0]+" "+ item.Value[1] + " "+ item.Value[2] + " " + item.Value[3]);
            //}

            return movesPerRound;
        }
    }
}
