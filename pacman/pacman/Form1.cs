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

        // direction player is moving in. Only one will be true
        bool goup;
        bool godown;
        bool goleft;
        bool goright;

        string move;


        int boardRight = 320;
        int boardBottom = 320;
        int boardLeft = 0;
        int boardTop = 40;
        //player speed
        int speed = 5;

        int score = 0; int total_coins = 61;

        //ghost speed for the one direction ghosts
        int ghost1 = 5;
        int ghost2 = 5;
        
        //x and y directions for the bi-direccional pink ghost
        int ghost3x = 5;
        int ghost3y = 5;            

        public Form1() {
            
            this.port = FreeTcpPort();
            ClientServices.form = this;
            TcpChannel chan = new TcpChannel(port);
            ChannelServices.RegisterChannel(chan, false);

            // Alternative 1 for service activation
            ClientServices servicos = new ClientServices();
            RemotingServices.Marshal(servicos, "Client",
                typeof(ClientServices));

            IServer server = (IServer)Activator.GetObject(typeof(IServer), "tcp://localhost:8086/Server");
            ClientServices.players = server.RegisterClient(port.ToString());
            this.server = server;
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

        public void AddMsg(string s)
        {
            this.tbChat.AppendText("\r\n" + s);
        }

        private void keyisdown(object sender, KeyEventArgs e) {
            List<string> Moves;

            string move = "";

            Moves = new List<string>();
            if (e.KeyCode == Keys.Left) {
                Moves.Add("goleft");
                move = "goleft";
              //  pacman.Image = Properties.Resources.Left;
            }
            if (e.KeyCode == Keys.Right) {
                Moves.Add("goright");
                move = "goright";
                //  pacman.Image = Properties.Resources.Right;
            }
            if (e.KeyCode == Keys.Up) {
                Moves.Add("goup");
                move = "goup";
                // pacman.Image = Properties.Resources.Up;
            }
            if (e.KeyCode == Keys.Down) {
                Moves.Add("godown");
                move = "godown";
                // pacman.Image = Properties.Resources.down;
            }
            if (e.KeyCode == Keys.Enter) {
                    tbMsg.Enabled = true; tbMsg.Focus();
               }
            server.ReadPlay(move);
        }

        private void keyisup(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Left) {
                goleft = false;
            }
            if (e.KeyCode == Keys.Right) {
                goright = false;
            }
            if (e.KeyCode == Keys.Up) {
                goup = false;
            }
            if (e.KeyCode == Keys.Down) {
                godown = false;
            }
        }

        public void Move_Pacman (string Moves)
        {
            
            //move player
            if (Moves == "goleft")
            {
                
                if (pacman.Left > (boardLeft))
                    pacman.Left -= speed;
            }
            if (Moves == "goright")
            {
                if (pacman.Left < (boardRight))
                    pacman.Left += speed;
            }
            if (Moves == "goup")
            {
                if (pacman.Top > (boardTop))
                    pacman.Top -= speed;
            }
            if (Moves == "godown")
            {
                
                if (pacman.Top < (boardBottom))
                    pacman.Top += speed;
            }

        }

        public void timer1_Tick(object sender, EventArgs e) {
            label1.Text = "Score: " + score;

            
            //move ghosts
            redGhost.Left += ghost1;
            yellowGhost.Left += ghost2;

            // if the red ghost hits the picture box 4 then wereverse the speed
            if (redGhost.Bounds.IntersectsWith(pictureBox1.Bounds))
                ghost1 = -ghost1;
            // if the red ghost hits the picture box 3 we reverse the speed
            else if (redGhost.Bounds.IntersectsWith(pictureBox2.Bounds))
                ghost1 = -ghost1;
            // if the yellow ghost hits the picture box 1 then wereverse the speed
            if (yellowGhost.Bounds.IntersectsWith(pictureBox3.Bounds))
                ghost2 = -ghost2;
            // if the yellow chost hits the picture box 2 then wereverse the speed
            else if (yellowGhost.Bounds.IntersectsWith(pictureBox4.Bounds))
                ghost2 = -ghost2;
            //moving ghosts and bumping with the walls end
            //for loop to check walls, ghosts and points
            foreach (Control x in this.Controls) {
                // checking if the player hits the wall or the ghost, then game is over
                if (x is PictureBox && x.Tag == "wall" || x.Tag == "ghost") {
                    if (((PictureBox)x).Bounds.IntersectsWith(pacman.Bounds)) {
                        pacman.Left = 0;
                        pacman.Top = 25;
                        label2.Text = "GAME OVER";
                        label2.Visible = true;
                        timer1.Stop();
                    }
                }
                if (x is PictureBox && x.Tag == "coin") {
                    if (((PictureBox)x).Bounds.IntersectsWith(pacman.Bounds)) {
                        this.Controls.Remove(x);
                        score++;
                        //TODO check if all coins where "eaten"
                        if (score == total_coins) {
                            //pacman.Left = 0;
                            //pacman.Top = 25;
                            label2.Text = "GAME WON!";
                            label2.Visible = true;
                            timer1.Stop();
                            }
                    }
                }
            }
                pinkGhost.Left += ghost3x;
                pinkGhost.Top += ghost3y;

                if (pinkGhost.Left < boardLeft ||
                    pinkGhost.Left > boardRight ||
                    (pinkGhost.Bounds.IntersectsWith(pictureBox1.Bounds)) ||
                    (pinkGhost.Bounds.IntersectsWith(pictureBox2.Bounds)) ||
                    (pinkGhost.Bounds.IntersectsWith(pictureBox3.Bounds)) ||
                    (pinkGhost.Bounds.IntersectsWith(pictureBox4.Bounds))) {
                    ghost3x = -ghost3x;
                }
                if (pinkGhost.Top < boardTop || pinkGhost.Top + pinkGhost.Height > boardBottom - 2) {
                    ghost3y = -ghost3y;
                }
        }

        private void tbMsg_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                IClient client = (IClient)Activator.GetObject(typeof(IClient), "tcp://localhost:"+this.port+"/Client");
                client.SendMsg("Player" + this.port +  " : " + this.tbMsg.Text);
                this.tbMsg.Text = null;
                tbMsg.Enabled = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

    delegate void DelAddMsg(string mensagem);
    delegate void DelUpdateGame(string moves);

    public class ClientServices : MarshalByRefObject, IClient
    {
        public static Form1 form;
        public static List<IClient> players;
        IServer server;
        List<string> messages;
        List<string> moves;

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

        public void SendMsg(string mensagem)
        {
            messages.Add(mensagem);
            ThreadStart ts = new ThreadStart(this.BroadcastMessage);
            Thread t = new Thread(ts);
            t.Start();
        }

       
        public void UpdateGame(List<String> moves)
        {
            for(int i = 0; i < moves.Count; i++)
                form.Invoke(new DelUpdateGame(form.Move_Pacman), moves[i]);

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
    }
}
