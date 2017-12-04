using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pacman
{
    class PuppetMaster
    {

        public static string ext_pid;
        static Thread th;
        static ThreadStart ths;
        static void Main()
        {

            while (true) { readConsole(); };
            //readConsole();
            //System.Console.ReadLine();
        }

        private static void testStuff()
        {    
            Console.WriteLine("Alive");
        }

        static private void readConsole()
        {
            string input = Console.ReadLine();
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };

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

        static private void cmdStartClient(string pid, string pcs_url, string client_url, int msec_per_round, int num_players)
        {
            Console.WriteLine("Starting Client");
            ext_pid = pid;

            Console.WriteLine("EXT is: " + ext_pid);

            Stuff test = new Stuff();
            test.pid = "pid";
            ths = new ThreadStart(test.main);
            th = new Thread(ths);
            th.Start();
            Thread.Sleep(1000);
            Console.WriteLine("Main thread ({0}) exiting...",
                              Thread.CurrentThread.ManagedThreadId);


        }

        static private void cmdStartServer(string pid, string pcs_url, string client_url, int msec_per_round, int num_players)
        {
            Console.WriteLine("Starting Server");

        }

        static private void cmdGlobalStatus()
        {
            Console.WriteLine("Global Status");
            Console.WriteLine(th.ThreadState);
        }

        static private void cmdCrash(string pid)
        {
            Console.WriteLine("Crashing");

            try
            {
                //th.ResetAbort();
                th.Abort();
            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("Abort!");
            }
            

        }

        static private void cmdFreeze(string pid)
        {
            Console.WriteLine("Freezing");
            th.Suspend();
       
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
    }

    class Stuff
    {
        public string pid { get; set; }

        public void main()
        {
            Console.WriteLine(pid);
        }
    }
}
