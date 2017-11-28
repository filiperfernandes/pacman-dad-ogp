using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Timers;
using System.Collections.Generic;

using RemotingInterfaces;

namespace Server
{
    class Server
    {
        private static Timer myTimer;
        // Dictionary key: round, value: players round moves
        private static Dictionary<int, Dictionary<int, List<bool>>> allRoundsMoves;
        // Dictionary key: player gameID, value: moves of the player
        private static Dictionary<int, List<bool>> roundMoves;
        private static Dictionary<int, IClient> clients;
        private static ServerPacman game;

        public Server(int port)
        {
            allRoundsMoves = new Dictionary<int, Dictionary<int, List<bool>>>();
            roundMoves = new Dictionary<int, List<bool>>();
            clients = new Dictionary<int, IClient>();
            ServerServices.server = this;
            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(ServerServices), "Server",
                WellKnownObjectMode.Singleton);
        }

        [STAThread]
        static void Main(string[] args)
        {
            game = new ServerPacman(1);
            game.setLevel1();
            myTimer = new Timer(20);
            myTimer.Elapsed += AtualizaJogo;
            myTimer.AutoReset = true;
            myTimer.Enabled = true;
            new Server(8086);
            Console.WriteLine("Press <enter> to terminate chat server...");
            Console.ReadLine();
        }

        private static void AtualizaJogo(object sender, ElapsedEventArgs e)
        {
            if (clients.Count > 0)
            {
                allRoundsMoves.Add(allRoundsMoves.Count + 1, roundMoves);
                //List of tuples with tag, name, score, xPosition and yPosition
                List<Tuple<string, string, int, int, int>> mylist = new List<Tuple<string, string, int, int, int>>();
                List<PacmanObject> positions = game.updateGame(roundMoves);
                foreach (PacmanObject pacmanObject in positions)
                {
                    mylist.Add(new Tuple<string, string, int, int, int>(pacmanObject.getTag(),
                        pacmanObject.getName(),
                        pacmanObject.getScore(),
                        pacmanObject.getObjectRectangle().X, 
                        pacmanObject.getObjectRectangle().Y));
                }
                foreach (var key in clients.Keys)
                {
                    ((IClient)clients[key]).PlayMoves(mylist);
                }
                roundMoves = new Dictionary<int, List<bool>>();
            }
            /*if(allRoundsMoves.Count == 10)
            {
                foreach (KeyValuePair<int, Dictionary<int, List<bool>>> round in allRoundsMoves)
                {
                    Console.WriteLine("Round = {0}", round.Key);
                    foreach (KeyValuePair<int, List<bool>> player in round.Value)
                    {
                        Console.WriteLine("GameID = {0}, moves = [{1}, {2}, {3}, {4}]", player.Key,
                            player.Value[0].ToString(), player.Value[1].ToString(),
                            player.Value[2].ToString(), player.Value[3].ToString());
                    }
                }
            }*/
        }

        public void AddMoves(int gameID, List<bool> moves)
        {
            for (int i = 0; i < roundMoves.Count; i++)
            {
                if (roundMoves.ContainsKey(gameID))
                {
                    roundMoves[gameID] = moves;
                    return;
                }
            }
            roundMoves.Add(gameID, moves);
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
    }

    class PacmanObject
    {
        private string tag;
        private string name = "";
        private int score = 0;
        private int xSpeed = 0;
        private int ySpeed = 0;
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
            this.initialPosition = new System.Drawing.Point((position % 9) * 40 + 8, ((position / 9) + 1) * 40);
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

        private System.Drawing.Point setPosition(int position)
        {
            return new System.Drawing.Point((position % 9) * 40 + 8, ((position / 9) +1) * 40);
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
            this.objectRectangle.X += speed;
        }

        public void objectRectangleChangeYBySpeed(int speed)
        {
            this.objectRectangle.Y += speed;
        }

        public void setInitialPosition()
        {
            this.objectRectangle.X = this.initialPosition.X;
            this.objectRectangle.Y = this.initialPosition.Y;
        }
    }

    class ServerPacman 
    {
        private List<PacmanObject> objects;
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
        public void setLevel1()
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
                }
                //check the collisions with the alive pacmans
                else if (pacmanObject is PacmanObject && pacmanObject.getTag() == "pacman")
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
                            }
                        }
                        if (x is PacmanObject && x.getTag() == "coin")
                        {
                            if (x.getObjectRectangle().IntersectsWith(pacmanObject.getObjectRectangle()))
                            {
                                this.objects.Remove(x);
                                pacmanObject.setScore(pacmanObject.getScore()+1);
                            }
                        }
                    }
                }
            }
            return this.objects;
        }
    }
}
