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

            string repl_url = args[0];
            int msec_per_round = Int32.Parse(args[1]);
            int num_players = Int32.Parse(args[2]);
            int cli = Int32.Parse(args[3]);


            if (cli==1)
            {
                exe_path = Client.exe_path();
            }
            else
            {
                exe_path = Server.exe_path();
            }

            string args2 = repl_url + ' ' + msec_per_round + ' ' + num_players;
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
    }
}
