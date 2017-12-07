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
        static MainWindow main;


        [STAThread]
        static void Main()
        {
            var th = new Thread(consoleApp);
            th.Start();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            main = new MainWindow();
            Application.Run(main);
            printPM("Cheguei",1);
        }

        private static void consoleApp()
        {
            string input;
            while (true) {
                input = Console.ReadLine();
                readConsole(input, 1); };
        }

        static public string[] splitInputBox (string input)
        {
            
            char[] delimiterChars = { ' ', '\t' };
            string[] words = input.Split(delimiterChars);
            return words;
        }

        public static void readConsole(string input, int src)
        {
            string[] words = splitInputBox(input);

            switch (words[0])
            {
                case "StartClient":
                    cmdStartClient(words[1], words[2], words[3], Int32.Parse(words[4]), Int32.Parse(words[5]), src);
                    break;
                case "StartServer":
                    cmdStartServer(words[1], words[2], words[3], Int32.Parse(words[4]), Int32.Parse(words[5]), src);
                    break;
                case "GlobalStatus":
                    cmdGlobalStatus(src);
                    break;
                case "Crash":
                    cmdCrash(words[1], src);
                    break;
                case "Freeze":
                    cmdFreeze(words[1], src);
                    break;
                case "Unfreeze":
                    cmdUnfreeze(words[1], src);
                    break;
                case "InjectDelay":
                    cmdInjectDelay(words[1], words[2], src);
                    break;
                case "LocalState":
                    cmdLocalState(words[1], Int32.Parse(words[2]),src);
                    break;
                case "Wait":
                    cmdWait(Int32.Parse(words[1]), src);
                    break;
                default:
                    Console.WriteLine("Command not found!");
                    break;
            }

        }

        public static void cmdStartClient(string pid, string pcs_url, string client_url, int msec_per_round, int num_players, int src)
        {
            pidToUrl.Add(pid, client_url);
            activeClient.Add(client_url);

            printPM("Starting Client", src);
            printPM("Checking for PCS", src);

            remote = checkPCS(pcs_url);
            //Console.WriteLine(pcs_url);

            remote.createReplica(pid, pcs_url, client_url, msec_per_round, num_players, 1);

        }

        public static void cmdStartServer(string pid, string pcs_url, string server_url, int msec_per_round, int num_players, int src)
        {

            pidToUrl.Add(pid, server_url);
            activeServer.Add(server_url);

            printPM("Starting Server", src);
            printPM("Checking for PCS", src);

            remote = checkPCS(pcs_url);
            //Console.WriteLine(pcs_url);

            remote.createReplica(pid, pcs_url, server_url, msec_per_round, num_players, 0);

        }

        static public void cmdGlobalStatus(int src)
        {
            printPM("Global Status", src);
            
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

            printPM("Everything okay with: " + active, src);
            printPM("Might be down: " + inactive, src);

        }

        public static void cmdCrash(string pid, int src)
        {
            printPM("Crashing" + pid, src);

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

                    pidToUrl.Remove(pid);
                    remote.Crash();
                }
                catch (Exception ex) { };
            }
            else
            {
                IClient remote = RemotingServices.Connect(typeof(IClient),
                "tcp://localhost:" + port + "/Client") as IClient;
                try
                {
                    pidToUrl.Remove(pid);
                    remote.Crash();          
                }
                catch (Exception ex) { };
            //IClient client = (IClient)Activator.GetObject(typeof(IClient), pidToUrl[pid]);
            //client.Crash();
            }
        }

        static public void cmdFreeze(string pid, int src)
        {
            printPM("Freezing" + pid, src);
        }

        static public void cmdUnfreeze(string pid, int src)
        {
            printPM("Unfreezing" + pid, src);
        }

        static public void cmdInjectDelay(string src_pid, string dst_pid, int src)
        {
            printPM("Injecting Delay from:" + src_pid + " to:" + dst_pid, src);
        }

        static public void cmdLocalState(string pid, int round_id, int src)
        {
            printPM("LocalState of:" + pid + " on round:" + round_id, src);
        }

        static public void cmdWait(int x_ms, int src)
        {

            printPM("Sleeping for " + x_ms.ToString(), src);
            
            System.Threading.Thread.Sleep(x_ms);

        }

        static public IPCS checkPCS(string pcs_url)
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

        static public void printPM(string text, int src)
        {

            if (src == 1)
            {
                Console.WriteLine(text);
            }
            else
            {
                main.output_box.AppendText(text + "\r\n");
            }
            //Console.WriteLine(text);
            //main.output_box.Text += text +  "\r\n";           
        }
    }
}
