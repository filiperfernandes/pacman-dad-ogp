using RemotingInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Threading;

namespace pacman
{
    public class PCS : MarshalByRefObject, IPCS
    {
        public static string pcs_url = "localhost";
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "localhost";
        }

        public static string getUrl()
        {
            return pcs_url;
        }

        public PCS(string url)
        {
            pcs_url = url;
        }
        public PCS() : base() { }

        public static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("PCS");
            Console.WriteLine(args[0]);

            TcpChannel channel = new TcpChannel(11000);
            ChannelServices.RegisterChannel(channel, false);

            PCS pcs = new PCS();
            RemotingServices.Marshal(pcs, "pcs", typeof(IPCS));

            // Dont close console
            Console.ReadLine();

            // Close TcpChannel
            channel.StopListening(null);
            RemotingServices.Disconnect(pcs);
            ChannelServices.UnregisterChannel(channel);
            channel = null;
        }

        void IPCS.createReplica(string pid, string pcs_url, string cli_srv_url, int msec_per_round, int num_players, int cli)
        {
            string args = cli_srv_url+" "+msec_per_round+ " "+ num_players + " "+cli;

            string exe_path;

            exe_path = Replica.exe_path();
            ProcessStartInfo info = new ProcessStartInfo(exe_path, args);
            info.CreateNoWindow = false;

            Process.Start(info);
        }

        public static string exe_path()
        {
            return @Environment.CurrentDirectory + "/PCS.exe";
        }
    }
}