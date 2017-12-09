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
using System.Diagnostics;

namespace pacman {
    public partial class Form1 : Form {

        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        private IServer server;
        private IClient client;
        private int port;
        private int gameID = 0;
        public static List<string> removedCoins = new List<string>();
        //Dicionario de todas as rondas key: ronda, value: Dicionario de um PacmanObject
        //Dicionario de uma ronda key:PacmanObject ID, value: tuple com atributos do PacmanObject
        //Tuplo dos atributos: Tag, Score (caso nao seja Pacman resultado 0), posicaoX, posicaoY  
        Dictionary<int, Dictionary<string, Tuple<string, int, int, int>>> allRounds =
            new Dictionary<int, Dictionary<string, Tuple<string, int, int, int>>>();
        List<bool> moves = new List<bool>(new bool[4]);

        public Form1(int port, Dictionary<int, List<bool>> inputFile, bool input) {

            //this.port = FreeTcpPort();
            this.port = port;
            ClientServices.form = this;
            TcpChannel chan = new TcpChannel(port);
            ChannelServices.RegisterChannel(chan, false);
            ClientServices.clientMoves = inputFile;
            ClientServices.inputFile = input;

            // Alternative 1 for service activation
            ClientServices servicos = new ClientServices();
            RemotingServices.Marshal(servicos, "Client",
                typeof(ClientServices));

            this.server = (IServer)Activator.GetObject(typeof(IServer), "tcp://localhost:20001/Server");
            //this.server = (IServer)Activator.GetObject(typeof(IServer), "tcp://1.2.3.4:20001/Server");
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
            if (ClientServices.processing)
            {
                if (e.KeyCode == Keys.Left)
                {
                    moves[0] = true;
                    //pacman.Image = Properties.Resources.Left;
                }
                if (e.KeyCode == Keys.Right)
                {
                    moves[1] = true;
                    //pacman.Image = Properties.Resources.Right;
                }
                if (e.KeyCode == Keys.Up)
                {
                    moves[2] = true;
                    //pacman.Image = Properties.Resources.Up;
                }
                if (e.KeyCode == Keys.Down)
                {
                    moves[3] = true;
                    //pacman.Image = Properties.Resources.down;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    tbMsg.Enabled = true; tbMsg.Focus();
                }
            }
        }

        private void keyisup(object sender, KeyEventArgs e) {
            if (ClientServices.processing)
            {
                if (e.KeyCode == Keys.Left)
                {
                    moves[0] = false;
                }
                if (e.KeyCode == Keys.Right)
                {
                    moves[1] = false;
                }
                if (e.KeyCode == Keys.Up)
                {
                    moves[2] = false;
                }
                if (e.KeyCode == Keys.Down)
                {
                    moves[3] = false;
                }
            }
        }

        private void sendMovesToServer(object sender, EventArgs myEventArgs)
        {
            
            List<bool> movesFromFile = new List<bool>(new bool[4]);
            //Check if has input file
            if (ClientServices.inputFile == false) {
                try
                {
                    this.server.AddMoves(gameID, moves);
                }
                catch (Exception e) { }
            }
            else
            {
                if (ClientServices.round <= ClientServices.clientMoves.Count)
                {
                    movesFromFile = ClientServices.clientMoves[ClientServices.round];
                    ClientServices.round++;
                    if (ClientServices.round==55) { ClientServices.inputFile = false; }
                    try
                    {
                        this.server.AddMoves(gameID, movesFromFile);
                    } catch (Exception e) { }
                }

            }
            //this.server.AddMoves(gameID, moves);

        }

        private PictureBox setGhostImage(PictureBox picture, string ghostName)
        {
            picture.SizeMode = PictureBoxSizeMode.Zoom;
            if (ghostName == "pinkGhost")
            {
                picture.Image = Properties.Resources.pink_guy;
            }
            else if (ghostName == "yellowGhost")
            {
                picture.Image = Properties.Resources.yellow_guy;
            }
            else if (ghostName == "redGhost")
            {
                picture.Image = Properties.Resources.red_guy;
            }
            return picture;
        }

        private PictureBox setPacmanImage(PictureBox picture, string pacmanName)
        {
            picture.SizeMode = PictureBoxSizeMode.Zoom;
            picture.Image = Properties.Resources.Left;
            return picture;
        }
        public void Winner()
        {
            label2.Visible = true;
            label2.Text = "Winner";
        }

        public void GameOver()
        {
            label2.Visible = true;
            label2.Text = "GAME OVER";
        }

        public void addDeletedCoins(string entry)
        {
            removedCoins.Add(entry);
        }

        public void setInitialGame(List<Tuple<string, string, int, int, int, int, int>> myList, int round)
        {
            //Dicionario de uma ronda key:PacmanObject ID, value: tuple com atributos do PacmanObject
            //Tuplo dos atributos: Tag, Score (caso nao seja Pacman resultado 0), posicaoX, posicaoY 
            Dictionary<string, Tuple<string, int, int, int>> roundDict =
                new Dictionary<string, Tuple<string, int, int, int>>();
            foreach (Tuple<string, string, int, int, int, int, int> pacmanObject in myList)
            {
                roundDict.Add(pacmanObject.Item2, new Tuple<string, int, int, int>(
                    pacmanObject.Item1, 0,
                    pacmanObject.Item4, pacmanObject.Item5));
                var picture = new PictureBox
                {
                    BackColor = System.Drawing.Color.Transparent,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Tag = pacmanObject.Item1,
                    Name = pacmanObject.Item2,
                    Location = new System.Drawing.Point(pacmanObject.Item4, pacmanObject.Item5),
                    Size = new System.Drawing.Size(pacmanObject.Item6, pacmanObject.Item7),

                };
                if(pacmanObject.Item1 == "pacman")
                {
                    picture = setPacmanImage(picture, pacmanObject.Item2);
                }
                else if (pacmanObject.Item1 == "ghost")
                {
                    picture = setGhostImage(picture, pacmanObject.Item2);
                }
                else if (pacmanObject.Item1 == "wall")
                {
                    picture.BackColor = System.Drawing.Color.MidnightBlue;
                }
                else if (pacmanObject.Item1 == "coin")
                {
                    picture.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox56.Image")));
                }
                this.Controls.Add(picture);
            }
            allRounds.Add(round, roundDict);
        }

        public void save_Round(Dictionary<string, Tuple<string, int, int, int>> whatToSend, int round)
        {
            Dictionary<string, Tuple<string, int, int, int>> roundDict =
                new Dictionary<string, Tuple<string, int, int, int>>();
            if (round != allRounds.Count)
            {
                roundDict = allRounds[allRounds.Count - 1];
                for (int i = allRounds.Count; i < round; i++)
                {
                    allRounds.Add(i, roundDict);
                }
            }
            //Dicionario de uma ronda key:PacmanObject ID, value: tuple com atributos do PacmanObject
            //Tuplo dos atributos: Tag, Score (caso nao seja Pacman resultado 0), posicaoX, posicaoY 
            roundDict = new Dictionary<string, Tuple<string, int, int, int>>();
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "coin" || (string)x.Tag == "ghost")
                {
                    roundDict.Add(x.Name, new Tuple<string, int, int, int>(
                    (string)x.Tag, 0, x.Location.X, x.Location.Y));
                }
                else if (x is PictureBox && (string)x.Tag == "pacman" && whatToSend.ContainsKey(x.Name))
                {
                    roundDict.Add(x.Name, new Tuple<string, int, int, int>(
                    (string)x.Tag, whatToSend[x.Name].Item2, x.Location.X, x.Location.Y));
                }
            }
            allRounds.Add(round, roundDict);
        }

        public void timer1_Tick(Dictionary<string, Tuple<string, int, int, int>> whatToSend)
        {
            foreach (Control x in this.Controls)
            {
                if (removedCoins.Contains(x.Name))
                {
                    this.Controls.Remove(x);
                }
                if (x is PictureBox && (string)x.Tag == "coin" && whatToSend.ContainsKey(x.Name))
                {
                    this.Controls.Remove(x);
                }
                else if (x is PictureBox && (string)x.Tag == "pacman" && whatToSend.ContainsKey(x.Name))
                {
                    x.Location = new System.Drawing.Point(whatToSend[x.Name].Item3, whatToSend[x.Name].Item4);
                }
                else if (x is PictureBox && (string)x.Tag == "ghost" && whatToSend.ContainsKey(x.Name))
                {
                    x.Location = new System.Drawing.Point(whatToSend[x.Name].Item3, whatToSend[x.Name].Item4);
                }
                if(x is PictureBox && (string)x.Tag == "pacman" && x.Name == this.gameID.ToString())
                {
                    label1.Text = "Score: " + whatToSend[x.Name].Item2;
                }
            }
        }

        public Dictionary<string, Tuple<string, int, int, int>> localState(int round)
        {
            return allRounds[round];
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
            ClientServices.isInitialize = true;

        }
        private void Form1_Closed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tbChat_TextChanged(object sender, EventArgs e)
        {

        }
    }

    delegate Dictionary<string, Tuple<string, int, int, int>> DelLocalState(int round);
    delegate void DelAddMsg(string mensagem);
    delegate void DelSetGameID(int gameID);
    delegate void DelWinner();
    delegate void DelGameOver();
    delegate void DelSaveRound(Dictionary<string, Tuple<string, int, int, int>> whatToSend, int round);
    delegate void DelDoRound(Dictionary<string, Tuple<string, int, int, int>> whatToSend);
    delegate void DelSetInitialGame(List<Tuple<string, string, int, int, int, int, int>> myList, int round);
    delegate void DelAddDeletedCoins(string entry);

    public class ClientServices : MarshalByRefObject, IClient
    {
        public static Form1 form;
        public static List<IClient> players;
        public static bool isInitialize = false;
        List<string> messages;
        public static Boolean processing = true;
        public static Boolean inputFile = false;
        List<bool> movesFile ;
        public static int round = 0;
        public static Dictionary<int, List<bool>> clientMoves;

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

        public Dictionary<string, Tuple<string, int, int, int>> localState(int round)
        {
            DelLocalState delLocalState = new DelLocalState(form.localState);
            return delLocalState(round);
        }

        public void PlayMoves(Dictionary<string, Tuple<string, int, int, int>> whatToSend, int round)
        {
            if (isInitialize)
            {
                if (ClientServices.processing)
                { 
                    form.Invoke(new DelDoRound(form.timer1_Tick), whatToSend);
                }
                else
                {
                    foreach(KeyValuePair<string, Tuple<string, int, int, int>> entry in whatToSend)
                    {
                        if (entry.Value.Item1.Equals("coin"))
                        {
                            form.Invoke(new DelAddDeletedCoins(form.addDeletedCoins), entry.Key);
                        }
                    }
                }
                form.Invoke(new DelSaveRound(form.save_Round), whatToSend, round);
            }
            
            //DelDoRound delDoRound = new DelDoRound(form.timer1_Tick);
            //delDoRound(whatToSend);
        }
        public void GameOver()
        {
            form.Invoke(new DelGameOver(form.GameOver));
        }
        public void Winner()
        {
            form.Invoke(new DelWinner(form.Winner));
        }

        public void setInitialGame(List<Tuple<string, string, int, int, int, int, int>> myList, int round)
        {
            if (isInitialize)
            {
                form.Invoke(new DelSetInitialGame(form.setInitialGame), myList, round);
            }
            else
            {
                Thread.Sleep(1000);
                setInitialGame(myList, round);
            }
            
            //DelSetInitialGame delSetInitialGame = new DelSetInitialGame(form.setInitialGame);
            //delSetInitialGame(myList);
        }

        public void Crash()
        {
            Console.WriteLine("CRASH");
            Process.GetCurrentProcess().Kill();
        }

        public int isAlive()
        {
            return 1;
        }

        public void Freeze()
        {
            ClientServices.processing = false;
        }

        public void Unfreeze()
        {
            ClientServices.processing = true;
        }

        public void addInputFile(Dictionary<int, List<bool>> clientMoves)
        {
            ClientServices.clientMoves = clientMoves;
            ClientServices.inputFile = true;
        }
    }
}
