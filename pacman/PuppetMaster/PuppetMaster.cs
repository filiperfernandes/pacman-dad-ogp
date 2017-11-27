using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMaster
{
    class PuppetMaster
    {
        static void Main()
        {
            readConsole();
            System.Console.ReadLine();
        }

        static private void readConsole()
        {
            string input = Console.ReadLine();
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };

            string[] words = input.Split(delimiterChars);

            switch (words[0])
            {
                case "StartClient":
                    cmdStartClient(Int32.Parse(words[1]), words[2], words[3], Int32.Parse(words[4]), Int32.Parse(words[4]));
                    break;
                case "StartServer:":
                    cmdStartServer(Int32.Parse(words[1]), words[2], words[3], Int32.Parse(words[4]), Int32.Parse(words[4]));
                    break;
                case "GlobalStatus":
                    cmdGlobalStatus();
                    break;
                case "Crash":
                    cmdCrash(Int32.Parse(words[1]));
                    break;
                case "Freeze":
                    cmdFreeze(Int32.Parse(words[1]));
                    break;
                case "Unfreeze":
                    cmdUnfreeze(Int32.Parse(words[1]));
                    break;
                case "InjectDelay":
                    cmdInjectDelay(Int32.Parse(words[1]), Int32.Parse(words[2]));
                    break;
                case "LocalState":
                    cmdLocalState(Int32.Parse(words[1]), Int32.Parse(words[2]));
                    break;
                case "Wait":
                    cmdWait(Int32.Parse(words[1]));
                    break;
            }

        }

        static private void cmdStartClient(int pid, string pcs_url, string client_url, int msec_per_round, int num_players)
        {
            Console.WriteLine("Starting Client");

            //int pid = Int32.Parse(words[1]);
            //string pcs_url = words[2];
            //string client_url = args[2];
            //int msec_per_round = Int32.Parse(args[3]);
            //int num_players = Int32.Parse(args[4]);

        }

        static private void cmdStartServer(int pid, string pcs_url, string client_url, int msec_per_round, int num_players)
        {
            Console.WriteLine("Starting Server");

        }

        static private void cmdGlobalStatus()
        {
            Console.WriteLine("Global Status");
        }

        static private void cmdCrash(int pid)
        {
            Console.WriteLine("Crashing");

        }

        static private void cmdFreeze(int pid)
        {
            Console.WriteLine("Freezing");

        }

        static private void cmdUnfreeze(int pid)
        {
            Console.WriteLine("Starting CLient");

        }

        static private void cmdInjectDelay(int src_pid, int dst_pid)
        {
            Console.WriteLine("Injecting Delay");

        }

        static private void cmdLocalState(int pid, int round_pid)
        {
            Console.WriteLine("LocalState");

        }

        static private void cmdWait(int x_ms)
        {
            Console.WriteLine("Sleeping...");

            Console.WriteLine(x_ms);

            System.Threading.Thread.Sleep(x_ms);

            Console.WriteLine("olaola");
        }
    }
}
