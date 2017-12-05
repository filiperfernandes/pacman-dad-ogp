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
        public static void Main(string[] args)
        {
            string exe_path;
            int cli = Int32.Parse(args[0]);

            //string repl_url = args[0];
            //int msec_per_round = Int32.Parse(args[1]);
            //int num_players = Int32.Parse(args[2]);
            //Boolean cli = Boolean.Parse(args[3]);

            //Debug.WriteLine(repl_url + msec_per_round + num_players + cli);


            if (cli==1)
            {
                exe_path = Client.exe_path();
            }
            else
            {
                exe_path = Server.exe_path();
            }

            string args2 = "";
            //exe_path = Server.exe_path();
            ProcessStartInfo info = new ProcessStartInfo(exe_path, args2);
            info.CreateNoWindow = false;

            Process p = Process.Start(info);

        }


        //public Replica(string repl_url, Boolean cli)
        //{
        //    string exe_path;

        //    if (cli)
        //    {
        //        exe_path = Client.exe_path();
        //    }
        //    else
        //    {
        //        exe_path = Server.exe_path();
        //    }

        //    string args = "";
        //    ProcessStartInfo info = new ProcessStartInfo(exe_path, args);
        //    info.CreateNoWindow = false;

        //    Process p = Process.Start(info);

        //    //return p.Id;

        //}

        public static string exe_path()
        {
            return @Environment.CurrentDirectory + "/Replica.exe";
        }

        public void Crash()
        {
            //IServer root = RemotingServices.Connect(typeof(IServer),
            //   url) as IServer;
            //root.Crash();
            Console.WriteLine("CRASH");
            Process.GetCurrentProcess().Kill();
        }

        public void Freeze()
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
    }
}
