using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using RemotingInterfaces;
using System.Runtime.Remoting;
using System.Diagnostics;

namespace pacman
{
    class PuppetMaster
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        
        private static Dictionary<string, int> dpcs = new Dictionary<string, int>();
        private static Dictionary<string, string> pidToUrl = new Dictionary<string, string>();
        private static List<string> pcsList = new List<string>();
        private static Dictionary<string, IPCS> pcs = new Dictionary<string, IPCS>();
        private static List<string> activeServer = new List<string>();
        private static List<string> activeClient = new List<string>();
        static IPCS remote;


        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new MainWindow());

            Console.WriteLine("Cheguei");
            while (true) { readConsole(); };
            readConsole();
            //Console.ReadLine();


        }
        //PCS pcs = new PCS();


        //Copy from old PupperMaster Console App
        public static string ext_pid;
        static Thread th;
        static ThreadStart ths;
        //static void Main()
        //{
        //    System.Console.WriteLine("Cheguei");
        //    while (true) { readConsole(); };
        //    //readConsole();
        //    //System.Console.ReadLine();
        //}

        private static void testStuff()
        {
            Console.WriteLine("Alive");
        }

        private static void readConsole()
        {
            string input = Console.ReadLine();
            char[] delimiterChars = { ' ','\t' };

            string[] words = input.Split(delimiterChars);

            switch (words[0])
            {
                case "StartClient":
                    cmdStartClient(words[1], words[2], words[3], Int32.Parse(words[4]), Int32.Parse(words[5]));
                    break;
                case "StartServer":
                    cmdStartServer(words[1], words[2], words[3], Int32.Parse(words[4]), Int32.Parse(words[5]));
                    break;
                case "GlobalStatus":
                    cmdGlobalStatus();
                    break;
                case "Crash":
                    cmdCrash(words[1]);
                    break;
                case "Freeze":
                    cmdFreeze(words[1]);
                    break;
                case "Unfreeze":
                    cmdUnfreeze(words[1]);
                    break;
                case "InjectDelay":
                    cmdInjectDelay(words[1], words[2]);
                    break;
                case "LocalState":
                    cmdLocalState(words[1], Int32.Parse(words[2]));
                    break;
                case "Wait":
                    cmdWait(Int32.Parse(words[1]));
                    break;
                default:
                    Console.WriteLine("Command not found!");
                    break;
            }

        }

        private static void cmdStartClient(string pid, string pcs_url, string client_url, int msec_per_round, int num_players)
        {
            Console.WriteLine("Starting Client" + client_url);

            pidToUrl.Add(pid, client_url);
            activeClient.Add(client_url);

            //Console.WriteLine("Checking for pcs");
            //checkPCS(pcs_url);
            //Console.WriteLine(pcs_url);

            Console.WriteLine("Checking for pcs");
            remote = checkPCS(pcs_url);
            Console.WriteLine(pcs_url);

            remote.createReplica(pid, pcs_url, client_url, msec_per_round, num_players, 1);

        }

        private static void cmdStartServer(string pid, string pcs_url, string server_url, int msec_per_round, int num_players)
        {
            Console.WriteLine("Starting Server" + server_url);

            pidToUrl.Add(pid, server_url);
            activeServer.Add(server_url);

            Console.WriteLine("Checking for pcs");
            remote = checkPCS(pcs_url);
            Console.WriteLine(pcs_url);
           
            remote.createReplica(pid, pcs_url, server_url, msec_per_round, num_players, 0);

        }

        static private void cmdGlobalStatus()
        {
            Console.WriteLine("Global Status");
            //Console.WriteLine(th.ThreadState);

            Replica r = new Replica();

            int s = 1;
            int c = 1;
            string active = "";
            string inactive = "";
            foreach (var s_url in activeServer)
            {
                if (r.CheckServer(s_url)==1) { active += " S" + s; }
                else { inactive += " S" + s; }
                s += 1;
            }
            foreach (var c_url in activeClient)
            {
                if (r.CheckClient(c_url) == 1) { active += " C" + c; }
                else { inactive += " C" + c; }
                c += 1;
            }

            Console.WriteLine("Everything okay with: " + active);
            Console.WriteLine("Might be down: " + inactive);
        }

        private static void cmdCrash(string pid)
        {
            Console.WriteLine("Crashing");
            //TODO: Check if Server or client to properly kill

            string stringCutted = pidToUrl[pid].Split('/').Last();
            Console.WriteLine(stringCutted);

            char[] delimiterChars = { ':', '/' };
            string[] words = pidToUrl[pid].Split(delimiterChars);

            //Setup game settings
            int port = Int32.Parse(words[4]);

            if (stringCutted.Equals("Server")) {
                //IServer remote = RemotingServices.Connect(typeof(IServer),
                //pidToUrl[pid]) as IServer;

                IServer remote = RemotingServices.Connect(typeof(IServer),
                "tcp://localhost:"+port+"/Server") as IServer;
                try
                {
                    remote.Crash();
                    pidToUrl.Remove(pid);
                }
                catch (Exception ex) { };
            }
            else
            {
                IClient remote = RemotingServices.Connect(typeof(IClient),
                "tcp://localhost:" + port + "/Client") as IClient;
                try
                {
                    Console.WriteLine(pidToUrl[pid]);
                    remote.Crash();
                    pidToUrl.Remove(pid);
                }
                catch (Exception ex) { };
            //IClient client = (IClient)Activator.GetObject(typeof(IClient), pidToUrl[pid]);
            //client.Crash();
            }
        }

        static private void cmdFreeze(string pid)
        {
            Console.WriteLine("Freezing");
            //th.Suspend();

        }

        static private void cmdUnfreeze(string pid)
        {
            Console.WriteLine("Starting CLient");
            th.Resume();

        }

        static private void cmdInjectDelay(string src_pid, string dst_pid)
        {
            Console.WriteLine("Injecting Delay");

        }

        static private void cmdLocalState(string pid, int round_id)
        {
            Console.WriteLine("LocalState");

        }

        static private void cmdWait(int x_ms)
        {
            Console.WriteLine("Sleeping...");

            Console.WriteLine(x_ms);

            System.Threading.Thread.Sleep(x_ms);

        }

        static private IPCS checkPCS(string pcs_url)
        {
            if (pcsList.Any(item => item.Equals(pcs_url))){
                return pcs[pcs_url];
            }
            else
            {
                ProcessStartInfo info = new ProcessStartInfo(PCS.exe_path(), pcs_url);
                info.CreateNoWindow = false;

                remote = RemotingServices.Connect(typeof(IPCS),
                "tcp://localhost:11000/PCS") as IPCS;

                pcsList.Add(pcs_url);
                pcs.Add(pcs_url, remote);


                Process.Start(info);

                return pcs[pcs_url];
            }

        }
    }
}
