using RemotingInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace pacman
{
    public class Replica : MarshalByRefObject, IReplica
    {
        private static Dictionary<string, int> urlToPid = new Dictionary<string, int>();

        public static void Main(string[] args)
        {
            string exe_path;
            string input_file;

            string repl_url = args[0];
            int msec_per_round = Int32.Parse(args[1]);
            int num_players = Int32.Parse(args[2]);
            int cli = Int32.Parse(args[3]);

            string args2;


            if (cli==1)
            {
                if (args.Length > 4)
                {
                    input_file = args[4];
                    args2 = repl_url + ' ' + msec_per_round + ' ' + num_players + ' ' + input_file;
                }
                else { args2 = repl_url + ' ' + msec_per_round + ' ' + num_players; }               
                exe_path = Client.exe_path();               
            }
            else
            {
                exe_path = Server.exe_path();
                args2 = repl_url + ' ' + msec_per_round + ' ' + num_players;
            }

            
            ProcessStartInfo info = new ProcessStartInfo(exe_path, args2);
            info.CreateNoWindow = false;

            Process p = Process.Start(info);

            urlToPid.Add(repl_url,p.Id);
            
        }


        public static string exe_path()
        {
            return @Environment.CurrentDirectory + "/Replica.exe";
        }

        public void Crash(string url)
        {
            Console.WriteLine("CRASH");
            try
            {
                Process.GetProcessById(urlToPid[url]).Kill();
            }
            catch (Exception ex) { };
        }

        public void Freeze(string url)
        {
            throw new NotImplementedException();
        }

        public string GlobalStatus()
        {
            throw new NotImplementedException();
        }

        public void Unfreeze()
        {
            throw new NotImplementedException();
        }

        public void Freeze()
        {
            throw new NotImplementedException();
        }

        public int CheckServer(string url)
        {
            char[] delimiterChars = { ':', '/' };
            string[] words = url.Split(delimiterChars);

            //Setup game settings
            int port = Int32.Parse(words[4]);

            IServer remote = RemotingServices.Connect(typeof(IServer),
                "tcp://localhost:" + port + "/Server") as IServer;

            try
            {
                int response = remote.isAlive();
                if (response == 1) { return 1; }
                else { return 0; }
            }
            catch (Exception ex) { };
            return 0;
        }

        public int CheckClient(string url)
        {
            char[] delimiterChars = { ':', '/' };
            string[] words = url.Split(delimiterChars);

            //Setup game settings
            int port = Int32.Parse(words[4]);

            IClient remote = RemotingServices.Connect(typeof(IClient),
                "tcp://localhost:" + port + "/Client") as IClient;

            try
            {
                int response = remote.isAlive();
                if (response == 1) { return 1; }
                else { return 0; }
            }
            catch (Exception ex) { };
            return 0;
        }

    }
}
