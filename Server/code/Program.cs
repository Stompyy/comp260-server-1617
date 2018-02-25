using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

using MessageTypes;
using Dungeon;


namespace Server
{
    // Struct binds a player instance to its socket for easy access from the playerDictionary
    struct PlayerInfo
    {
        public Player player;
        public Socket socket;

        // Constructor
        public PlayerInfo(Player newPlayer, Socket newSocket)
        {
            player = newPlayer;
            socket = newSocket;
        }
    }

    class Program
    {
        // Create a single instance of the dungeon for all players to use
        static DungeonClass myDungeon = new DungeonClass();

        // Dictionary stores all players and their appropriate socket in the PlayerInfo struct, with a string key
        static Dictionary<String, PlayerInfo> playerDictionary = new Dictionary<String, PlayerInfo>();

        // Initialise a client identifying integer
        static int clientID = 1;

        // Reverse look up for the string name key of the playerDictionary using the Player field of the PlayerInfo struct
        static String GetNameFromPlayer(Player player)
        {
            // Always lock when accessing!
            lock (playerDictionary)
            {
                foreach (KeyValuePair<String, PlayerInfo> pair in playerDictionary)
                {
                    if (pair.Value.player == player)
                    {
                        return pair.Key;
                    }
                }
                return "";
            }
        }

        // Send clientName to socket
        static void SendClientName(Socket socket, String clientName)
        {
            ClientNameMsg nameMsg = new ClientNameMsg();
            nameMsg.name = clientName;

            MemoryStream outStream = nameMsg.WriteData();

            socket.Send(outStream.GetBuffer() );
        }

        // Send a message to every socket in the playerDictionary's PlayerInfo struct values, containing a current list of all client names from the keys of the dictionary
        static void SendClientList()
        {
            ClientListMsg clientListMsg = new ClientListMsg();
            
            // What did I say about locking!
            lock (playerDictionary)
            {
                // Fill the clientListMsg with all the string dictionary keys
                foreach (KeyValuePair<String, PlayerInfo> pair in playerDictionary)
                {
                    clientListMsg.clientList.Add(pair.Key);
                }

                MemoryStream outStream = clientListMsg.WriteData();

                // Send it to each socket
                foreach (KeyValuePair<String, PlayerInfo> pair in playerDictionary)
                {
                    pair.Value.socket.Send(outStream.GetBuffer());
                }
            }
        }

        static void SendChatMessage(String msg)
        {
            PublicChatMsg chatMsg = new PublicChatMsg();

            chatMsg.msg = msg;

            MemoryStream outStream = chatMsg.WriteData();
            
            lock (playerDictionary)
            {
                foreach (KeyValuePair<String, PlayerInfo> pair in playerDictionary)
                {
                    try
                    {
                        pair.Value.socket.Send(outStream.GetBuffer());
                    }
                    catch (System.Exception)
                    {

                    }
                }
            }
        }

        static void SendPrivateMessage(Socket s, String from, String msg)
        {
            PrivateChatMsg chatMsg = new PrivateChatMsg();
            chatMsg.msg = msg;
            chatMsg.destination = from;
            MemoryStream outStream = chatMsg.WriteData();

            try
            {
                s.Send(outStream.GetBuffer());
            }
            catch (System.Exception)
            {

            }
        }

        static void SendGameMessage(Socket s, String from, String msg)
        {
            GameMsg gameMessage = new GameMsg();
            gameMessage.msg = msg;
            gameMessage.destination = from;
            MemoryStream outStream = gameMessage.WriteData();

            try
            {
                s.Send(outStream.GetBuffer());
            }
            catch (System.Exception)
            {

            }
        }

        static Socket GetSocketFromName(String name)
        {
            lock (playerDictionary)
            {
                return playerDictionary[name].socket;
            }
        }

        static String GetNameFromSocket(Socket socket)
        {
            lock (playerDictionary)
            {
                foreach (KeyValuePair<String, PlayerInfo> pair in playerDictionary)
                {
                    if (pair.Value.socket == socket)
                    {
                        return pair.Key;
                    }
                }
            }

            return null;
        }
        
        static void ReceiveClientProcess(Object socket)
        {
            bool bQuit = false;

            // Cast object argument back into a socket
            Socket chatClient = (Socket)socket;

            // server console debug message
            Console.WriteLine("client receive thread for " + GetNameFromSocket(chatClient));

            // Create a new Player class instance
            Player newPlayer = new Player(GetNameFromSocket(chatClient), new List<Item>());

            // Initialise the start location
            newPlayer.CurrentRoom = myDungeon.StartRoom;

            // Locking on heavens door
            lock (playerDictionary)
            {
                // Sanity check, then add new Player and appropriate socket as a PlayerInfo struct to the playerDictionary with the clientName as the dictionary's key
                if (playerDictionary.ContainsKey(GetNameFromSocket(chatClient)))
                    playerDictionary[GetNameFromSocket(chatClient)] = new PlayerInfo(newPlayer, chatClient);
                else
                    playerDictionary.Add(GetNameFromSocket(chatClient), new PlayerInfo(newPlayer, chatClient));
            }

            // On new player instantiation, update all players with the new client list
            SendClientList();


            while (bQuit == false)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int result;

                    result = chatClient.Receive(buffer);

                    if (result > 0)
                    {
                        MemoryStream stream = new MemoryStream(buffer);
                        BinaryReader read = new BinaryReader(stream);

                        Msg message = Msg.DecodeStream(read);

                        if (message != null)
                        {
                            Console.Write("Got a message: ");
                            switch (message.mID)
                            {
                                case PublicChatMsg.ID:
                                    {
                                        PublicChatMsg publicMsg = (PublicChatMsg)message;

                                        String formattedMsg = "<" + GetNameFromSocket(chatClient)+"> " + publicMsg.msg;

                                        Console.WriteLine("public chat - " + formattedMsg);

                                        SendChatMessage(formattedMsg);
                                    }
                                    break;

                                case PrivateChatMsg.ID:
                                    {
                                        PrivateChatMsg privateMsg = (PrivateChatMsg)message;

                                        String formattedMsg = "PRIVATE <" + GetNameFromSocket(chatClient) + "> " + privateMsg.msg;

                                        Console.WriteLine("private chat - " + formattedMsg + "to " + privateMsg.destination);

                                        SendPrivateMessage(GetSocketFromName(privateMsg.destination), GetNameFromSocket(chatClient), formattedMsg);

                                        formattedMsg = "<" + GetNameFromSocket(chatClient) + "> --> <" +privateMsg.destination+"> " + privateMsg.msg;
                                        SendPrivateMessage(chatClient, "", formattedMsg);
                                    }
                                    break;

                                case GameMsg.ID:
                                    {
                                        // Cast back to GameMsg class
                                        GameMsg gameMessage = (GameMsg)message;

                                        // Get the string from the GameMsg class
                                        String formattedMsg = gameMessage.msg;

                                        // Get the sending client name, to be used as a dictionary key next, and to pass the String value
                                        String clientName = GetNameFromSocket(chatClient);

                                        // Lock it before changing it
                                        lock (playerDictionary)
                                        {
                                            // Temporary variable to alter outside of dictionary
                                            Player tempPlayer = playerDictionary[clientName].player;

                                            // Update function of the DungeonClass instance. If moved, Updates the current room of the player instance, and the currentRoom occupency of the dungeon room
                                            String sendMsg = myDungeon.Update(ref playerDictionary[clientName].player.GetCurrentRoomRef, ref tempPlayer, clientName, formattedMsg);

                                            // Console debug message
                                            Console.WriteLine(sendMsg);

                                            // "Say" command will always return a sendMsg beginning with "Room chat:". If true then send message to all players in room
                                            if (sendMsg.Substring(0, 10) == "Room chat:")
                                            {
                                                foreach (String playerName in tempPlayer.GetCurrentRoomRef.PlayersInRoom)
                                                {
                                                    SendGameMessage(playerDictionary[playerName].socket, "", sendMsg);
                                                }
                                            }
                                            else
                                            {
                                                // Update dictionary variable
                                                playerDictionary[clientName] = new PlayerInfo(tempPlayer, playerDictionary[clientName].socket);

                                                // Send the complete message to the player socket
                                                SendGameMessage(chatClient, "", sendMsg);
                                            }
                                        }
                                    }
                                    break;

                                case PlayerInitMsg.ID:
                                    {
                                        // Cast back to GameMsg class
                                        PlayerInitMsg playerInitMessage = (PlayerInitMsg)message;

                                        // Get the string from the PlayerInitMsg class
                                        String characterSheetString = playerInitMessage.msg;

                                        String[] processedCharacterSheet = characterSheetString.Split(' ');

                                        // Get the sending client name, to be used as a dictionary key next, and to pass the String value
                                        String clientName = GetNameFromSocket(chatClient);

                                        // Lock it like it's hot
                                        lock (playerDictionary)
                                        {
                                            Player tempPlayer = playerDictionary[clientName].player;
                                            tempPlayer.Init(processedCharacterSheet);
                                            // Update dictionary variable
                                            playerDictionary[clientName] = new PlayerInfo(tempPlayer, playerDictionary[clientName].socket);

                                        }
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }                   
                }

                // Remove player from game
                catch (Exception)
                {
                    bQuit = true;

                    // Lost the player message
                    String output = "Lost client: " + GetNameFromSocket(chatClient);

                    // Debug message to server console
                    Console.WriteLine(output);

                    // Inform the other players
                    SendChatMessage(output);

                    // Always lock the list!!!
                    lock (playerDictionary)
                    {
                        // Sanity check, then remove player name from the current rooms list of currently occupying players names
                        if (playerDictionary[GetNameFromSocket(chatClient)].player.GetCurrentRoomRef.PlayersInRoom.Contains(GetNameFromSocket(chatClient)))
                            playerDictionary[GetNameFromSocket(chatClient)].player.GetCurrentRoomRef.RemovePlayer(GetNameFromSocket(chatClient));

                        // Sanity check, then remove player information from playerDictionary
                        if (playerDictionary.ContainsKey(GetNameFromSocket(chatClient)))
                            playerDictionary.Remove(GetNameFromSocket(chatClient));
                    }

                    // Update all other players current client list
                    SendClientList();
                }
            }
        }

        static void Main(string[] args)
        {
            // Initialise the static DungeonClass instance
            myDungeon.Init();

            // Listening socket initialisation
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Local address for developing purposes!
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500));

            // Listen for new clients
            serverSocket.Listen(32);

            bool bQuit = false;

            // Console message to boast about being a server!
            Console.WriteLine("This is the server!");

            while (!bQuit)
            {
                // When there is a new connection, create a new socket
                Socket serverClient = serverSocket.Accept();

                // Start a new thread assigned to this socket
                Thread myThread = new Thread(ReceiveClientProcess);
                myThread.Start(serverClient);

                // Lock the kazbah
                lock (playerDictionary)
                {
                    // clientID increments after every new player is added, ensures a different new next clientName
                    String clientName = "client" + clientID;

                    // Gives the client its own clientName as a reference
                    SendClientName(serverClient, clientName);

                    // Create a new player instance
                    Player newPlayer = new Player(GetNameFromSocket(serverClient), new List<Item>());
                    playerDictionary.Add(clientName, new PlayerInfo(newPlayer, serverClient));

                    // Sanity check, then adds newly created Player to the members of the first room of the dungeon
                    if (!myDungeon.StartRoom.PlayersInRoom.Contains(clientName))
                        myDungeon.StartRoom.AddPlayer(clientName);
                }

                Thread.Sleep(500);

                // Update all players with up to date client list
                SendClientList();

                // Increment variable to ensure next clientName is different
                clientID++;

                // Initial room description message for client
                String initialMessage = "\r\n\r\n" + myDungeon.DescribeRoom(myDungeon.StartRoom);
                SendGameMessage(serverClient, "", initialMessage);
            }
        }
    }
}
