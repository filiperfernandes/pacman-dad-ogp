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
    class PCS : MarshalByRefObject
    {
        public static void createReplica(string pid, string pcs_url, string cli_srv_url, int msec_per_round, int num_players, Boolean cli)
        {

            string args = pid + " " + pcs_url + " " + cli_srv_url + msec_per_round + num_players;
            string exe_path;

            if (cli)
            {
                exe_path = Client.exe_path();
            }
            else
            {
                exe_path = Server.exe_path();
            }
            

            ProcessStartInfo info = new ProcessStartInfo(exe_path, args);
            info.CreateNoWindow = false;

            Process.Start(info);
        }

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

        public static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("PCS");
            Console.WriteLine("listening on tcp://" + PCS.GetLocalIPAddress() + ":11000/pcs");

            TcpChannel channel = new TcpChannel(1100);
            ChannelServices.RegisterChannel(channel, false);

            PCS pcs = new PCS();
            RemotingServices.Marshal(pcs, "pcs");

            createReplica("", "", "", 10, 10, false);
            createReplica("","","",10,10, true);
            createReplica("", "", "", 10, 10, true);


            // Dont close console
            Console.ReadLine();

            // Close TcpChannel
            channel.StopListening(null);
            RemotingServices.Disconnect(pcs);
            ChannelServices.UnregisterChannel(channel);
            channel = null;
        }
    }
}