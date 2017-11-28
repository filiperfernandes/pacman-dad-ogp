using System;
using System.Collections.Generic;
using System.Windows.Forms;
using RemotingInterfaces;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace pacman {
    public partial class Form1 : Form {

        private IServer server;
        private IClient client;
        private int port;
        private int gameID = 0;
        List<bool> moves = new List<bool>(new bool[4]);

        int total_coins = 60;
     

        public Form1() {
            
            this.port = FreeTcpPort();
            ClientServices.form = this;
            TcpChannel chan = new TcpChannel(port);
            ChannelServices.RegisterChannel(chan, false);

            // Alternative 1 for service activation
            ClientServices servicos = new ClientServices();
            RemotingServices.Marshal(servicos, "Client",
                typeof(ClientServices));

            this.server = (IServer)Activator.GetObject(typeof(IServer), "tcp://localhost:8086/Server");
            this.client = (IClient)Activator.GetObject(typeof(IClient), "tcp://localhost:" + this.port + "/Client");
            ClientServices.players = server.RegisterClient(port.ToString());
            InitializeComponent();
            label2.Visible = false;
        }

        private int FreeTcpPort()
        {
            TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            return port;
        }

        private void keyisdown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Left) {
                moves[0] = true;
                pacman.Image = Properties.Resources.Left;
            }
            if (e.KeyCode == Keys.Right) {
                moves[1] = true;
                pacman.Image = Properties.Resources.Right;
            }
            if (e.KeyCode == Keys.Up) {
                moves[2] = true;
                pacman.Image = Properties.Resources.Up;
            }
            if (e.KeyCode == Keys.Down) {
                moves[3] = true;
                pacman.Image = Properties.Resources.down;
            }
            if (e.KeyCode == Keys.Enter) {
                tbMsg.Enabled = true; tbMsg.Focus();
            }
        }

        private void keyisup(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Left) {
                moves[0] = false;
            }
            if (e.KeyCode == Keys.Right) {
                moves[1] = false;
            }
            if (e.KeyCode == Keys.Up) {
                moves[2] = false;
            }
            if (e.KeyCode == Keys.Down) {
                moves[3] = false;
            }
        }

        private void sendMovesToServer(object sender, EventArgs myEventArgs)
        {
            this.server.AddMoves(gameID, moves);
        }
        
        public void timer1_Tick(List<Tuple<string, string, int, int, int>> myList)
        {
            foreach (Tuple<string, string, int, int, int> pacmanObject in myList)
            {
                if (pacmanObject.Item1 == "pacman")
                {
                    if(pacmanObject.Item2 == this.gameID.ToString())
                    {
                        label1.Text = "Score: " + pacmanObject.Item3;
                        /*if (pacmanObject.Item3 == total_coins)
                        {
                            label2.Text = "GAME WON!";
                            label2.Visible = true;
                            timer1.Stop();
                        }*/
                        pacman.Location = new System.Drawing.Point(pacmanObject.Item4, pacmanObject.Item5);
                    }
                }

                else if (pacmanObject.Item1 == "ghost")
                {
                    if (pacmanObject.Item2 == "pinkGhost")
                    {
                        pinkGhost.Location = new System.Drawing.Point(pacmanObject.Item4, pacmanObject.Item5);
                    }
                    else if (pacmanObject.Item2 == "redGhost")
                    {
                        redGhost.Location = new System.Drawing.Point(pacmanObject.Item4, pacmanObject.Item5);
                    }
                    else if (pacmanObject.Item2 == "yellowGhost")
                    {
                        yellowGhost.Location = new System.Drawing.Point(pacmanObject.Item4, pacmanObject.Item5);
                    }
                }
            }
            //moving ghosts and bumping with the walls end
            //for loop to check walls, ghosts and points
            foreach (Control x in this.Controls) {
                // checking if the player hits the wall or the ghost, then game is over
                if (x is PictureBox && x.Tag == "wall" || x.Tag == "ghost") {
                    if (((PictureBox)x).Bounds.IntersectsWith(pacman.Bounds)) {
                        label2.Text = "GAME OVER";
                        label2.Visible = true;
                        timer1.Stop();
                    }
                }
                if (x is PictureBox && x.Tag == "coin") {
                    if (((PictureBox)x).Bounds.IntersectsWith(pacman.Bounds)) {
                        this.Controls.Remove(x);
                        //TODO check if all coins where "eaten"
                        /*if (score == total_coins) {
                            label2.Text = "GAME WON!";
                            label2.Visible = true;
                            timer1.Stop();
                        }*/
                    }
                }
            }
        }

        public void AddMsg(string s)
        {
            this.tbChat.AppendText("\r\n" + s);
        }

        public void SetGameID(int gameID)
        {
            this.gameID = gameID;
        }

        private void tbMsg_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                client.SendMsg("Player" + this.gameID +  ": " + this.tbMsg.Text);
                this.tbMsg.Text = null;
                tbMsg.Enabled = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    delegate void DelAddMsg(string mensagem);
    delegate void DelSetGameID(int gameID);
    delegate void DelDoRound(List<Tuple<string, string, int, int, int>> myList);


    public class ClientServices : MarshalByRefObject, IClient
    {
        public static Form1 form;
        public static List<IClient> players;
        List<string> messages;

        public ClientServices()
        {
            players = new List<IClient>();
            messages = new List<string>();
        }

        public void MsgToClient(string mensagem)
        {
            // thread-safe access to form
            form.Invoke(new DelAddMsg(form.AddMsg), mensagem);
        }

        public void AddNewPlayer(string NewClientName)
        {
            IClient newPlayer =
                (IClient)Activator.GetObject(
                       typeof(IClient), "tcp://localhost:" + NewClientName + "/Client");
            players.Add(newPlayer);
        }

        public void SetGameID(int gameID)
        {
            DelSetGameID delGameID = new DelSetGameID(form.SetGameID);
            delGameID(gameID);
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
            for (int i = 0; i < players.Count; i++)
            {
                try
                {
                    ((IClient)players[i]).MsgToClient(MsgToBcast);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed sending message to player. Removing client. " + e.Message);
                    players.RemoveAt(i);
                }
            }
        }

        public void PlayMoves(List<Tuple<string, string, int, int, int>> myList)
        {
            DelDoRound delDoRound = new DelDoRound(form.timer1_Tick);
            delDoRound(myList);
        }
    }
}
