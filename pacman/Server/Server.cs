using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Timers;
using System.Collections.Generic;

using RemotingInterfaces;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters;
using System.Collections;
using System.Net;

namespace pacman
{
    public class Server
    {
        private Object thisLock = new Object();
        private static Timer enoughPlayersTimer;
        private static Timer myTimer;
        // Dictionary key: player gameID, value: moves of the player
        private static Dictionary<int, List<bool>> roundMoves;
        private static Dictionary<int, IClient> clients;
        private static ServerPacman game;
        private static int num_players;
        private static int msec_per_round;

        public Server(string url, int port)
        {
            roundMoves = new Dictionary<int, List<bool>>();
            clients = new Dictionary<int, IClient>();
            ServerServices.server = this;
            TcpChannel channel = new TcpChannel(port);

            //BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            //provider.TypeFilterLevel = TypeFilterLevel.Full;
            //IDictionary props = new Hashtable();
            //props["name"] = "tcp";
            //props["bindTo"] = "127.0.0.1";
            //props["bindTo"] = IPAddress.Parse("1.2.3.4");
            //props["port"] = port;
            //TcpChannel channel = new TcpChannel(props, null, provider);



            Console.WriteLine(channel.ChannelName);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(ServerServices), "Server",
                WellKnownObjectMode.Singleton);
        }

        public static string exe_path()
        {
            return @Environment.CurrentDirectory + "/Server.exe";
        }

        [STAThread]
        static void Main(string[] args)
        {
            string url = args[0];
            msec_per_round = Int32.Parse(args[1]);
            //int num_players = Int32.Parse(args[2]);

            char[] delimiterChars = { ':', '/' };
            string[] words = url.Split(delimiterChars);

            //Setup game settings
            int port = Int32.Parse(words[4]);
            num_players = Int32.Parse(args[2]);

            game = new ServerPacman(num_players);
            enoughPlayersTimer = new Timer(msec_per_round);
            enoughPlayersTimer.Elapsed += checkIfEnoughPlayers;
            enoughPlayersTimer.AutoReset = true;
            enoughPlayersTimer.Enabled = true;
            new Server(url, port);
            Console.WriteLine("Listening on port " + port);
            Console.WriteLine("Press <enter> to terminate chat server...");
            Console.ReadLine();
        }

        private static void checkIfEnoughPlayers(object sender, ElapsedEventArgs e)
        {
            if (clients.Count >= num_players)
            {
                enoughPlayersTimer.Enabled = false;
                List<PacmanObject> positions = game.setLevel1();
                //List of tuples with tag, name, score, xPosition, yPosition, xSize and ySize
                List<Tuple<string, string, int, int, int, int, int>> myList =
                    new List<Tuple<string, string, int, int, int, int, int>>();
                foreach (PacmanObject pacmanObject in positions)
                {
                    myList.Add(new Tuple<string, string, int, int, int, int, int>(
                        pacmanObject.getTag(), pacmanObject.getName(), 0,
                        pacmanObject.getCurrentX(), pacmanObject.getCurrentY(),
                        pacmanObject.getObjectRectangle().Width, pacmanObject.getObjectRectangle().Height));
                }
                /*if (flag == 0)
                {
                    foreach (Tuple<string, string, int, int, int, int, int> x in myList)
                    {
                        Console.WriteLine("List: {0} {1} {2} {3} {4} {5} {6}", x.Item1, x.Item2, x.Item3, x.Item4, x.Item5, x.Item6, x.Item7);
                    }
                }
                flag = 1;*/
                foreach (var key in clients.Keys)
                {
                    ((IClient)clients[key]).setInitialGame(myList);
                }
                //System.Threading.Thread.Sleep(1000);
                myTimer = new Timer(msec_per_round);
                myTimer.Elapsed += AtualizaJogo;
                myTimer.AutoReset = true;
                myTimer.Enabled = true;
            }
        }

        private static void AtualizaJogo(object sender, ElapsedEventArgs e)
        {
            if (clients.Count > 0)
            {
                //List of tuples with tag, name, score, xPosition and yPosition
                Dictionary<string, Tuple<string, int, int, int>> whatToSend =
                    new Dictionary<string, Tuple<string, int, int, int>>();
                List<PacmanObject> positions = game.updateGame(roundMoves);
                foreach (PacmanObject pacmanObject in positions)
                {
                    whatToSend.Add(pacmanObject.getName(), new Tuple<string, int, int, int>(
                        pacmanObject.getTag(),
                        pacmanObject.getScore(),
                        pacmanObject.getCurrentX(),
                        pacmanObject.getCurrentY()));
                }
                foreach (var key in clients.Keys)
                {
                    ((IClient)clients[key]).PlayMoves(whatToSend);
                }
                roundMoves = new Dictionary<int, List<bool>>();
            }
        }

        public void AddMoves(int gameID, List<bool> moves)
        {
            lock (thisLock)
            {
                if (roundMoves.ContainsKey(gameID))
                {
                    roundMoves[gameID] = moves;
                    return;
                }
                else
                {
                    roundMoves.Add(gameID, moves);
                }
            }
            
        }

        public void RegisterClient(int gameID, string NewClientName)
        {
            IClient newClient =
                (IClient)Activator.GetObject(
                       typeof(IClient), "tcp://localhost:" + NewClientName + "/Client");
            clients.Add(gameID, newClient);
        }

        public void RemoveClient(int gameID)
        {
            clients.Remove(gameID);
        }
    }

    delegate void DelAddMoves(int gameID, List<bool> moves);
    delegate void DelRegisterClient(int gameID, string NewClientName);
    delegate void DelRemoveClient(int gameID);

    class ServerServices : MarshalByRefObject, IServer
    {
        public static Server server;
        Dictionary<int, IClient> clients;

        ServerServices()
        {
            clients = new Dictionary<int, IClient>();
        }

        public List<IClient> RegisterClient(string NewClientName)
        {
            Console.WriteLine("New client listening at " + "tcp://localhost:" + NewClientName + "/Client");
            int gameID = 0;
            IClient newClient =
                (IClient)Activator.GetObject(
                       typeof(IClient), "tcp://localhost:" + NewClientName + "/Client");
            for (int i = 1; i <= clients.Count + 1; i++)
            {
                if (!clients.ContainsKey(i))
                {
                    gameID = i;
                    break;
                }
            }
            InformNewClientArrival(NewClientName);
            clients.Add(gameID, newClient);
            ServerRegisterClient(gameID, NewClientName);
            newClient.SetGameID(gameID);
            return new List<IClient>(clients.Values);
        }
        
        private void InformNewClientArrival(string NewClientName)
        {
            foreach (KeyValuePair<int, IClient> entry in clients)
            {
                try
                {
                    ((IClient)clients[entry.Key]).AddNewPlayer(NewClientName);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to inform client. Removing client. " + e.Message);
                    clients.Remove(entry.Key);
                    ServerRemoveClient(entry.Key);
                }
            }
        }

        private void ServerRegisterClient(int gameID, string NewClientName)
        {
            DelRegisterClient delRegisterClient = new DelRegisterClient(server.RegisterClient);
            delRegisterClient(gameID, NewClientName);
        }

        private void ServerRemoveClient(int gameID)
        {
            DelRemoveClient delRemoveClient = new DelRemoveClient(server.RemoveClient);
            delRemoveClient(gameID);
        }

        public void AddMoves(int gameID, List<bool> moves)
        {
            DelAddMoves delAddMoves = new DelAddMoves(server.AddMoves);
            delAddMoves(gameID, moves);
        }

        public void Crash()
        {
            Console.WriteLine("Aqui");
            Process.GetCurrentProcess().Kill();
        }

        int IServer.isAlive()
        {
            return 1;
        }
    }

    [Serializable]
    class PacmanObject
    {
        private string tag;
        private string name = "";
        private int score = 0;
        private int xSpeed = 0;
        private int ySpeed = 0;
        private int currentX = 0;
        private int currentY = 0;
        private bool isAlive = true;
        private System.Drawing.Point initialPosition;
        private System.Drawing.Rectangle objectRectangle;

        public PacmanObject()
        {
            
        }

        //Used to initialize pacmans
        public PacmanObject(string tag, string name, int xSpeed, int ySpeed, int position, int xSize, int ySize)
        {
            this.tag = tag;
            this.name = name;
            this.xSpeed = xSpeed;
            this.ySpeed = ySpeed;
            this.currentX = (position % 9) * 40 + 8;
            this.currentY = ((position / 9) + 1) * 40;
            this.initialPosition = new System.Drawing.Point(this.currentX, this.currentY);
            System.Drawing.Size size = new System.Drawing.Size(xSize, ySize);
            this.objectRectangle = new System.Drawing.Rectangle(initialPosition, size);
        }

        //Used to initialize ghosts
        public PacmanObject(string tag, string name, int xSpeed, int ySpeed, int xPosition, int yPosition, int xSize, int ySize)
        {
            this.tag = tag;
            this.name = name;
            this.xSpeed = xSpeed;
            this.ySpeed = ySpeed;
            this.currentX = xPosition;
            this.currentY = yPosition;
            System.Drawing.Point pointPosition = new System.Drawing.Point(xPosition, yPosition);
            System.Drawing.Size size = new System.Drawing.Size(xSize, ySize);
            this.objectRectangle = new System.Drawing.Rectangle(pointPosition, size);
        }

        public void setTag(string tag)
        {
            this.tag = tag;
        }

        public string getTag()
        {
            return this.tag;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public string getName()
        {
            return this.name;
        }

        public void setScore(int score)
        {
            this.score = score;
        }

        public int getScore()
        {
            return this.score;
        }

        public void setXSpeed(int xSpeed)
        {
            this.xSpeed = xSpeed;
        }

        public int getXSpeed()
        {
            return this.xSpeed;
        }

        public void setYSpeed(int ySpeed)
        {
            this.ySpeed = ySpeed;
        }

        public int getYSpeed()
        {
            return this.ySpeed;
        }

        public void setCurrentX(int currentX)
        {
            this.currentX = currentX;
        }

        public int getCurrentX()
        {
            return this.currentX;
        }

        public void setCurrentY(int currentY)
        {
            this.currentY = currentY;
        }

        public int getCurrentY()
        {
            return this.currentY;
        }

        public void setIsAlive(bool isAlive)
        {
            this.isAlive = isAlive;
        }

        public bool getIsAlive()
        {
            return this.isAlive;
        }

        public System.Drawing.Point getInitialPosition()
        {
            return this.initialPosition;
        }

        private System.Drawing.Point setPosition(int position)
        {
            this.currentX = (position % 9) * 40 + 8;
            this.currentY = ((position / 9) + 1) * 40;
            return new System.Drawing.Point(this.currentX, this.currentY);
        }

        private System.Drawing.Size setSize(int xSize, int ySize)
        {
            return new System.Drawing.Size((xSize - 1) * 40 + 15, (ySize - 1) * 40 + 15);
        }

        public void setObjectRectangle(int position, int xSize, int ySize)
        {
            System.Drawing.Point pointPosition = setPosition(position);
            System.Drawing.Size size = setSize(xSize, ySize);
            this.objectRectangle = new System.Drawing.Rectangle(pointPosition, size);
        }

        public System.Drawing.Rectangle getObjectRectangle()
        {
            return this.objectRectangle;
        }

        public void objectRectangleChangeXBySpeed(int speed)
        {
            this.currentX += speed;
            this.objectRectangle.X += speed;
        }

        public void objectRectangleChangeYBySpeed(int speed)
        {
            this.currentY += speed;
            this.objectRectangle.Y += speed;
        }

        public void setInitialPosition()
        {
            this.currentX = this.initialPosition.X;
            this.currentY = this.initialPosition.Y;
            this.objectRectangle.X = this.initialPosition.X;
            this.objectRectangle.Y = this.initialPosition.Y;
        }
    }

    class ServerPacman 
    {
        private List<PacmanObject> objects;
        private List<PacmanObject> whatToSend;
        private int numPlayers;
        private int boardRight;
        private int boardBottom;
        private int boardLeft;
        private int boardTop;

        public ServerPacman(int numPlayers)
        {
            this.objects = new List<PacmanObject>();
            this.numPlayers = numPlayers;
        }

        /*
        tabuleiro posicoes:
         00 | 01 | 02 | 03 | 04 | 05 | 06 | 07 | 08 
        ----+----+----+----+----+----+----+----+----
         09 | 10 | 11 | 12 | 13 | 14 | 15 | 16 | 17 
        ----+----+----+----+----+----+----+----+----
         18 | 19 | 20 | 21 | 22 | 23 | 24 | 25 | 26  
        ----+----+----+----+----+----+----+----+----
         27 | 28 | 29 | 30 | 31 | 32 | 33 | 34 | 35 
        ----+----+----+----+----+----+----+----+----
         36 | 37 | 38 | 39 | 40 | 41 | 42 | 43 | 44  
        ----+----+----+----+----+----+----+----+----
         45 | 46 | 47 | 48 | 49 | 50 | 51 | 52 | 53 
        ----+----+----+----+----+----+----+----+----
         54 | 55 | 56 | 57 | 58 | 59 | 60 | 61 | 62 
        ----+----+----+----+----+----+----+----+----
         63 | 64 | 65 | 66 | 67 | 68 | 69 | 70 | 71 
        */
        public List<PacmanObject> setLevel1()
        {
            setBoardsLimits(320, 320, 0, 40);
            //List with coins positions for level 1
            List<int> coins = new List<int>() {0, 1, 3, 4, 5, 7, 8,
                                                9, 10, 12, 13, 14, 16, 17,
                                                18, 19, 21, 22, 23, 25, 26,
                                                27, 28, 29, 30, 31, 32, 33, 34, 35,
                                                36, 37, 38, 39, 40, 41, 42, 43, 44,
                                                45, 46, 47, 49, 50, 51, 53,
                                                54, 55, 56, 58, 59, 60, 62,
                                                63, 64, 65, 67, 68, 69, 71};
            //List with wall positions, xSizes and ySizes for level 1 
            List<int[]> walls = new List<int[]>() {new int[] {2, 1, 3}, new int[] {6, 1, 3},
                                                    new int[] {48, 1, 3}, new int[] {52, 1, 3}};
            foreach (int position in coins)
            {
                PacmanObject coin = new PacmanObject();
                coin.setTag("coin");
                coin.setName("coin" + position.ToString());
                coin.setObjectRectangle(position, 1, 1);
                this.objects.Add(coin);
            }
            foreach (int[] wallData in walls)
            {
                PacmanObject wall = new PacmanObject();
                wall.setTag("wall");
                wall.setName("wall" + wallData[0].ToString());
                wall.setObjectRectangle(wallData[0], wallData[1], wallData[2]);
                this.objects.Add(wall);
            }
            //Add pinkGhost to list of pacmanObjects
            setGhosts("pinkGhost", 5, 5, 301, 72);
            //Add yellowGhost to list of pacmanObjects
            setGhosts("yellowGhost", 5, 0, 221, 273);
            //Add redGhost to list of pacmanObjects
            setGhosts("redGhost", 5, 0, 180, 73);

            setPacmans(this.numPlayers);
            return this.objects;
        }

        private void setBoardsLimits(int boardRight, int boardBottom, int boardLeft, int boardTop)
        {
            this.boardRight = boardRight;
            this.boardBottom = boardBottom;
            this.boardLeft = boardLeft;
            this.boardTop = boardTop;
        }
        
        private void setGhosts(string name, int xSpeed, int ySpeed, int xPosition, int yPosition)
        {
            PacmanObject ghost = new PacmanObject("ghost", name, xSpeed, ySpeed, xPosition, yPosition, 30, 30);
            this.objects.Add(ghost);
        }

        private void setPacmans(int numPlayers)
        {
            for (int i = 0; i < numPlayers; i++)
            {
                PacmanObject pacman = new PacmanObject("pacman", (i+1).ToString(), 5, 5, i*9, 25, 25);
                this.objects.Add(pacman);
            }
        }

        public List<PacmanObject> updateGame(Dictionary<int, List<bool>> roundMoves)
        {
            this.whatToSend = new List<PacmanObject>();
            foreach (PacmanObject pacmanObject in this.objects)
            {
                //move the ghosts in the correct direction
                if (pacmanObject is PacmanObject && pacmanObject.getTag() == "ghost")
                {
                    pacmanObject.objectRectangleChangeXBySpeed(pacmanObject.getXSpeed());
                    pacmanObject.objectRectangleChangeYBySpeed(pacmanObject.getYSpeed());
                    foreach (PacmanObject x in this.objects)
                    {
                        if (x is PacmanObject && x.getTag() == "wall")
                        {
                            if ((pacmanObject.getObjectRectangle().IntersectsWith(x.getObjectRectangle())))
                            {
                                pacmanObject.setXSpeed(-pacmanObject.getXSpeed());
                            }
                        }
                    }
                    if (pacmanObject.getObjectRectangle().Left < this.boardLeft ||
                        pacmanObject.getObjectRectangle().Left > this.boardRight)
                    {
                        pacmanObject.setXSpeed(-pacmanObject.getXSpeed());
                    }
                    if (pacmanObject.getObjectRectangle().Top < this.boardTop ||
                        pacmanObject.getObjectRectangle().Top + pacmanObject.getObjectRectangle().Height > this.boardBottom - 2)
                    {
                        pacmanObject.setYSpeed(-pacmanObject.getYSpeed());
                    }
                    this.whatToSend.Add(pacmanObject);
                }
                //check the collisions with the alive pacmans
                else if (pacmanObject is PacmanObject && pacmanObject.getTag() == "pacman" && pacmanObject.getIsAlive())
                {
                    int gameID;
                    bool isNumber = Int32.TryParse(pacmanObject.getName(), out gameID);
                    if (isNumber && roundMoves.ContainsKey(gameID))
                    {
                        List<bool> moves = roundMoves[gameID];
                        //check goLeft
                        if (moves[0])
                        {
                            if (pacmanObject.getObjectRectangle().Left > this.boardLeft)
                            {
                                pacmanObject.objectRectangleChangeXBySpeed(-pacmanObject.getXSpeed());
                            }
                        }
                        //check goRight
                        if (moves[1])
                        {
                            if (pacmanObject.getObjectRectangle().Left < this.boardRight)
                            {
                                pacmanObject.objectRectangleChangeXBySpeed(pacmanObject.getXSpeed());
                            }
                        }
                        //check goUp
                        if (moves[2])
                        {
                            if (pacmanObject.getObjectRectangle().Top > this.boardTop)
                            {
                                pacmanObject.objectRectangleChangeYBySpeed(-pacmanObject.getYSpeed());
                            }
                        }
                        //check goDown
                        if (moves[3])
                        {
                            if (pacmanObject.getObjectRectangle().Top < this.boardBottom)
                            {
                                pacmanObject.objectRectangleChangeYBySpeed(pacmanObject.getYSpeed());
                            }
                        }
                    }
                    foreach (PacmanObject x in this.objects)
                    {
                        if (x is PacmanObject && x.getTag() == "wall" || x.getTag() == "ghost")
                        {
                            if (x.getObjectRectangle().IntersectsWith(pacmanObject.getObjectRectangle()))
                            {
                                //pacman dies
                                pacmanObject.setInitialPosition();
                                pacmanObject.setIsAlive(false);
                            }
                        }
                        if (x is PacmanObject && x.getTag() == "coin" && x.getIsAlive())
                        {
                            if (x.getObjectRectangle().IntersectsWith(pacmanObject.getObjectRectangle()))
                            {
                                //removes coin
                                this.whatToSend.Add(x);
                                x.setIsAlive(false);
                                pacmanObject.setScore(pacmanObject.getScore()+1);
                            }
                        }
                    }
                    this.whatToSend.Add(pacmanObject);
                }
            }
           
            return this.whatToSend;
        }
    }
}
