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
    class Program
    {
        static DungeonClass myDungeon = new DungeonClass();

        // Dictionary to store all players in. Uses Socket as key
        static Dictionary<Socket, Player> playerDictionary = new Dictionary<Socket, Player>();

        static Dictionary<String, Socket> clientDictionary = new Dictionary<String, Socket>();
        static int clientID = 1;

        static void SendClientName(Socket s, String clientName)
        {
            ClientNameMsg nameMsg = new ClientNameMsg();
            nameMsg.name = clientName;

            // Update the Player instance with the name
            playerDictionary[s].SetName(clientName);

            MemoryStream outStream = nameMsg.WriteData();

            s.Send(outStream.GetBuffer() );
        }

        static void SendClientList()
        {
            ClientListMsg clientListMsg = new ClientListMsg();

            lock (clientDictionary)
            {
                foreach (KeyValuePair<String, Socket> s in clientDictionary)
                {
                    clientListMsg.clientList.Add(s.Key);
                }

                MemoryStream outStream = clientListMsg.WriteData();

                foreach (KeyValuePair<String, Socket> s in clientDictionary)
                {
                    s.Value.Send(outStream.GetBuffer());
                }
            }
        }

        static void SendChatMessage(String msg)
        {
            PublicChatMsg chatMsg = new PublicChatMsg();

            chatMsg.msg = msg;

            MemoryStream outStream = chatMsg.WriteData();

            lock (clientDictionary)
            {            
                foreach (KeyValuePair<String,Socket> s in clientDictionary)
                {
                    try
                    {
                        s.Value.Send(outStream.GetBuffer());
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
            lock (clientDictionary)
            {
                return clientDictionary[name];
            }
        }

        static String GetNameFromSocket(Socket s)
        {
            lock (clientDictionary)
            {
                foreach (KeyValuePair<String, Socket> o in clientDictionary)
                {
                    if (o.Value == s)
                    {
                        return o.Key;
                    }
                }
            }

            return null;
        }

        static void RemoveClientBySocket(Socket socket)
        {
            string name = GetNameFromSocket(socket);

            if (name != null)
            {
                lock (clientDictionary)
                {
                    clientDictionary.Remove(name);
                }
                lock (playerDictionary)
                {
                    playerDictionary.Remove(socket);
                }
            }
        }


        static void receiveClientProcess(Object o)
        {
            bool bQuit = false;

            Socket chatClient = (Socket)o;

            Console.WriteLine("client receive thread for " + GetNameFromSocket(chatClient));
            Player newPlayer = new Player();
            newPlayer.SetRoom(myDungeon.GetStartRoom());
            if (playerDictionary.ContainsKey(chatClient))
            {
                playerDictionary[chatClient] = newPlayer;
            }
            else
            {
                playerDictionary.Add(chatClient, newPlayer);
            }

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

                        Msg m = Msg.DecodeStream(read);

                        if (m != null)
                        {
                            Console.Write("Got a message: ");
                            switch (m.mID)
                            {
                                case PublicChatMsg.ID:
                                    {
                                        PublicChatMsg publicMsg = (PublicChatMsg)m;

                                        String formattedMsg = "<" + GetNameFromSocket(chatClient)+"> " + publicMsg.msg;

                                        Console.WriteLine("public chat - " + formattedMsg);

                                        SendChatMessage(formattedMsg);
                                    }
                                    break;

                                case PrivateChatMsg.ID:
                                    {
                                        PrivateChatMsg privateMsg = (PrivateChatMsg)m;

                                        String formattedMsg = "PRIVATE <" + GetNameFromSocket(chatClient) + "> " + privateMsg.msg;

                                        Console.WriteLine("private chat - " + formattedMsg + "to " + privateMsg.destination);

                                        SendPrivateMessage(GetSocketFromName(privateMsg.destination), GetNameFromSocket(chatClient), formattedMsg);

                                        formattedMsg = "<" + GetNameFromSocket(chatClient) + "> --> <" +privateMsg.destination+"> " + privateMsg.msg;
                                        SendPrivateMessage(chatClient, "", formattedMsg);
                                    }
                                    break;

                                case GameMsg.ID:
                                    {
                                        GameMsg gameMessage = (GameMsg)m;

                                        String formattedMsg = gameMessage.msg;

                                        Room tempCurrentRoom = playerDictionary[chatClient].GetRoom();

                                        // Looks up player from dictionary by the socket, then gets reference to current room
                                        String sendMsg = myDungeon.Process(ref playerDictionary[chatClient].GetRoom(), formattedMsg);
                                        sendMsg += "\r\n" + myDungeon.DescribeRoom(tempCurrentRoom);

                                        // Update room with new player presence
                                        if (playerDictionary[chatClient].GetRoom() != tempCurrentRoom)
                                        {
                                            Player p;
                                            playerDictionary[chatClient].GetRoom().RemovePlayer(ref playerDictionary[chatClient]);
                                            tempCurrentRoom.AddPlayer(ref playerDictionary[chatClient]);
                                        }

                                        playerDictionary[chatClient].SetRoom(tempCurrentRoom);

                                        Console.WriteLine(sendMsg);
                                        SendGameMessage(chatClient, "", sendMsg);

                                        
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }                   
                }
                catch (Exception)
                {
                    bQuit = true;

                    String output = "Lost client: " + GetNameFromSocket(chatClient);
                    Console.WriteLine(output);
                    SendChatMessage(output);

                    RemoveClientBySocket(chatClient);

                    SendClientList();
                }
            }
        }

        static void Main(string[] args)
        {
            // Initialise the static DungeonClass instance
            myDungeon.Init();

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500));
            serverSocket.Listen(32);

            bool bQuit = false;

            Console.WriteLine("Server");

            while (!bQuit)
            {
                Socket serverClient = serverSocket.Accept();

                Thread myThread = new Thread(receiveClientProcess);
                myThread.Start(serverClient);

                lock (clientDictionary)
                {
                    // Create a new player instance
                    Player newPlayer = new Player();

                    // Store in dictionary with the socket as the key
                    if (playerDictionary.ContainsKey(serverClient))
                        playerDictionary[serverClient] = newPlayer;
                    else
                        playerDictionary.Add(serverClient, newPlayer);

                    // Add newly created Player to the members of the first room of the dungeon
                    if (!myDungeon.GetStartRoom().GetPlayersInRoom().Contains(newPlayer))
                        myDungeon.GetStartRoom().AddPlayer(ref newPlayer);


                    String clientName = "client" + clientID;
                    clientDictionary.Add(clientName, serverClient);

                    SendClientName(serverClient, clientName);
                    Thread.Sleep(500);
                    SendClientList();

                    clientID++;
                }
                
            }
        }
    }
}
