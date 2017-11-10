using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using System.Collections.Generic;
using RemotingInterfaces;

namespace Server
{
    class Server
    {
        public IServer server;
        
        [STAThread]
        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(ServerServices), "Server",
                WellKnownObjectMode.Singleton);
            System.Console.WriteLine("Press <enter> to terminate chat server...");
           // System.Console.ReadLine();
            while (true)
            {
                if (sw.ElapsedMilliseconds == 100)
                {
                    IServer server = (IServer)Activator.GetObject(typeof(IServer), "tcp://localhost:8086/Server");
                    server.CheckTime(true);
                    sw.Restart();

                }
                   
            }
        }
    }

    class ServerServices : MarshalByRefObject, IServer
    {
        List<IClient> clients;
        List<string> messages;

        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

        // Dictionary<IClient, List<string>> updates = new Dictionary<IClient, List<string>>();
        List<string> updates;
        
        ServerServices()
        {
            clients = new List<IClient>();
            messages = new List<string>();
            updates = new List<string>();
        }

        public void CheckTime(Boolean time)
        {
            if (time)
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    try
                    {
                        ((IClient)clients[i]).UpdateGame(updates);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed sending message to client. Removing client. " + e.Message);
                        clients.RemoveAt(i);
                    }
                }
                updates.Clear();

            } 
        }

        public void ReadPlay(String Moves)
        {
            //updates.Add(client, Moves);
            updates.Add(Moves);
            System.Console.WriteLine(Moves);
            
        }

        public List<IClient> RegisterClient(string NewClientName)
        {
            Console.WriteLine("New client listening at " + "tcp://localhost:" + NewClientName + "/Client");
            IClient newClient =
                (IClient)Activator.GetObject(
                       typeof(IClient), "tcp://localhost:" + NewClientName + "/Client");
            InformNewClientArrival(NewClientName);
            clients.Add(newClient);
            return clients;
        }

        public void InformNewClientArrival(string NewClientName)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                try
                {
                    ((IClient)clients[i]).AddNewPlayer(NewClientName);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to inform client. Removing client. " + e.Message);
                    clients.RemoveAt(i);
                }
            }
        }
        
        public void SendMsg(string mensagem)
        {
            messages.Add(mensagem);
            ThreadStart ts = new ThreadStart(this.BroadcastMessage);
            Thread t = new Thread(ts);
            t.Start();
        }
        private void BroadcastMessage()
        {
            string MsgToBcast;
            lock (this)
            {
                MsgToBcast = messages[messages.Count - 1];
            }
            
        }
    }
}
