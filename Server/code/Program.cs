using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

using MessageTypes;



namespace Server
{
    class Program
    {
        // Database table references
        static DungeonTable dungeonTable;
        static LoginTable loginsTable;
        static PlayersTable playersTable;
        static ItemsTable itemsTable;
        static NPCsTable npcsTable;
        static IdTable IDTable;

        // The default starting room for new players
        static String startRoom = "Mountain road";

        // The database that will store all of the tables
        static SQLDatabase database;

        // Controls instance to parse client strings
        static Controls controls;

        // Dictionary stores all currently connected playerNames and their associated socket for this session
        static Dictionary<String, Socket> playerSocketDictionary = new Dictionary<String, Socket>();

        /*
         * Send a message to every socket in the playerDictionary, containing a current list of all client names from the keys of the dictionary
         */
        static void SendClientList()
        {
            ClientListMsg clientListMsg = new ClientListMsg();

            // Always lock dictionarys
            lock (playerSocketDictionary)
            {
                // Fill the clientListMsg with all the string dictionary keys
                foreach (KeyValuePair<String, Socket> pair in playerSocketDictionary)
                {
                    clientListMsg.clientList.Add(pair.Key);
                }

                MemoryStream outStream = clientListMsg.WriteData();

                // Send it to each socket
                foreach (KeyValuePair<String, Socket> pair in playerSocketDictionary)
                {
                    try
                    {
                        pair.Value.Send(outStream.GetBuffer());
                    }catch { }
                }
            }
        }

        /*
         * Send a chat message to every socket in the playerDictionary
         */
        static void SendChatMessage(String msg)
        {
            PublicChatMsg chatMsg = new PublicChatMsg();

            chatMsg.msg = msg;

            MemoryStream outStream = chatMsg.WriteData();

            lock (playerSocketDictionary)
            {
                foreach (KeyValuePair<String, Socket> pair in playerSocketDictionary)
                {
                    try
                    {
                        pair.Value.Send(outStream.GetBuffer());
                    }
                    catch (System.Exception) { }
                }
            }
        }

        /*
         * Send a message to a specific socket, including the sender name String
         */
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
            catch (System.Exception) { }
        }

        /*
         * Send a game message to a client
         */
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
            catch (System.Exception) { }
        }

        /*
         * Send a player death message to a player that has died
         */
        static void SendDeathMessage(Socket s, String msg)
        {
            PlayerDeadMsg gameMessage = new PlayerDeadMsg();
            gameMessage.msg = msg;
            MemoryStream outStream = gameMessage.WriteData();

            try
            {
                s.Send(outStream.GetBuffer());
            }
            catch (System.Exception) { }
        }

        /*
         * Send a message that contains new user creation information when the client is authenticating a new user request
         */
        static void SendNewUserMessage(Socket s, String msg)
        {
            CreateNewUserMsg newUserMessage = new CreateNewUserMsg();
            newUserMessage.msg = msg;
            MemoryStream outStream = newUserMessage.WriteData();

            try
            {
                s.Send(outStream.GetBuffer());
            }
            catch (System.Exception) { }
        }

        /*
         * Send a message that contains login authentication information
         */
        static void SendLoginMessage(Socket s, String msg)
        {
            LoginMsg loginMessage = new LoginMsg();
            loginMessage.msg = msg;
            MemoryStream outStream = loginMessage.WriteData();

            try
            {
                s.Send(outStream.GetBuffer());
            }
            catch (System.Exception) { }
        }

        /*
         * Dictionary look up that locks the dictionary whilst looking
         */
        static Socket GetSocketFromName(String name)
        {
            lock (playerSocketDictionary)
            {
                return playerSocketDictionary[name];
            }
        }

        /*
         * Reverse dictionary look up that locks the dictionary whilst looking
         */
        static String GetNameFromSocket(Socket socket)
        {
            lock (playerSocketDictionary)
            {
                foreach (KeyValuePair<String, Socket> pair in playerSocketDictionary)
                {
                    if (pair.Value == socket)
                    {
                        return pair.Key;
                    }
                }
            }
            // else
            return null;
        }

        /*
         * Outbound string parsing from the controls class sometimes requires that a message is sent to a targeted other player as well as the player that has called the action
         */
        static void SendSplitMessage(Socket thisPlayerSocket, Socket secondPlayerSocket, String message)
        {
            // '@' symbool is used to split the output String between thisPlayer destined message and secondPlayer destined message
            String[] splitOutputMessage = message.Split('@');

            // Send the first part of the message to the this player socket - Split function requires a 1 based array accessor here. Odd but it places a "" at [0]
            SendGameMessage(thisPlayerSocket, "", splitOutputMessage[1]);

            // Send second part as a private message to the second player's socket
            SendPrivateMessage(secondPlayerSocket, "", splitOutputMessage[2]);
        }

        /*
         * For receiving player messages
         */
        static void ReceiveClientProcess(Object socket)
        {
            bool bQuit = false;

            // Cast object argument back into a socket
            Socket clientSocket = (Socket)socket;

            // Server console debug message
            Console.WriteLine("client receive new player thread");

            // Initialise string that will recieve the message from the server
            String sendMsg = "";

            while (bQuit == false)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int result;

                    result = clientSocket.Receive(buffer);

                    if (result > 0)
                    {
                        MemoryStream stream = new MemoryStream(buffer);
                        BinaryReader read = new BinaryReader(stream);

                        Msg message = Msg.DecodeStream(read);

                        if (message != null)
                        {
                            Console.Write("Client action: " + clientSocket);
                            switch (message.mID)
                            {
                                case PublicChatMsg.ID:
                                    {
                                        PublicChatMsg publicMsg = (PublicChatMsg)message;

                                        String formattedMsg = "<" + GetNameFromSocket(clientSocket) + "> " + publicMsg.msg;

                                        Console.WriteLine("public chat - " + formattedMsg);

                                        SendChatMessage(formattedMsg);
                                    }
                                    break;

                                case PrivateChatMsg.ID:
                                    {
                                        PrivateChatMsg privateMsg = (PrivateChatMsg)message;

                                        String formattedMsg = "PRIVATE <" + GetNameFromSocket(clientSocket) + "> " + privateMsg.msg;

                                        Console.WriteLine("private chat - " + formattedMsg + "to " + privateMsg.destination);

                                        SendPrivateMessage(GetSocketFromName(privateMsg.destination), GetNameFromSocket(clientSocket), formattedMsg);

                                        formattedMsg = "<" + GetNameFromSocket(clientSocket) + "> --> <" + privateMsg.destination + "> " + privateMsg.msg;
                                        SendPrivateMessage(clientSocket, "", formattedMsg);
                                    }
                                    break;

                                case GameMsg.ID:
                                    {
                                        // Cast back to GameMsg class
                                        GameMsg gameMessage = (GameMsg)message;

                                        // Get the string from the GameMsg class
                                        String formattedMsg = gameMessage.msg;

                                        // Filter the incoming message for a reference to another player and store
                                        String targetedOtherPlayerName = null;
                                        String[] FormattedStringWords = formattedMsg.Split(' ');
                                        foreach (String possibleName in FormattedStringWords)
                                        {
                                            if (playerSocketDictionary.ContainsKey(possibleName))
                                            {
                                                // Once found, break
                                                targetedOtherPlayerName = possibleName;
                                                break;
                                            }
                                        }

                                        // Get the sending client name, to be used as a dictionary key next, and to pass the String value
                                        String clientName = GetNameFromSocket(clientSocket);

                                        // Lock it before changing it
                                        lock (playerSocketDictionary)
                                        {
                                            String thisPlayerName = GetNameFromSocket(clientSocket);

                                            // Update function of the DungeonClass instance. If moved, Updates the current room of the player instance, and the currentRoom occupency of the dungeon room
                                            sendMsg = controls.Update(thisPlayerName, targetedOtherPlayerName, formattedMsg);

                                            // Console debug message
                                            Console.WriteLine(sendMsg);

                                            // Parse the returned String from the controls for any server specific commands. 

                                            // Try/catch is needed to avoid crashing due to mismatched string sizes. i.e. comparing the first 14 characters of a 10 length string.

                                            // "Say" command will always return a sendMsg beginning with "Room chat:". If true then send private message to all players in room
                                            try
                                            {
                                                if (sendMsg.Substring(0, 6) == "<Room>")
                                                {
                                                    String currentRoom = playersTable.getStringFieldFromName(thisPlayerName, "currentRoom");

                                                    foreach (String playerName in playersTable.getNamesFromField("currentRoom", currentRoom))
                                                    {
                                                        SendPrivateMessage(playerSocketDictionary[playerName], "", sendMsg);
                                                    }
                                                    break;
                                                }
                                            }
                                            catch { }

                                            // if "give (Item) to (Player)" is parsed correctly then the returned sendMsg string will start ""@<Gift>"". This will then inform the recipient player
                                            try
                                            {
                                                if (sendMsg.Substring(0, 7) == "@<Gift>")
                                                {
                                                    // Splits the message and sends each part to the corresponding socket
                                                    SendSplitMessage(clientSocket, GetSocketFromName(targetedOtherPlayerName), sendMsg);
                                                    break;
                                                }
                                            }
                                            catch (Exception) { }

                                            // If "pickpocket" attempt has been correctly executed but not successfully performed then inform the targetedPlayer
                                            try
                                            {
                                                if (sendMsg.Substring(0, 14) == "@<PickPocket> ")
                                                {
                                                    // Splits the message and sends each part to the corresponding socket
                                                    SendSplitMessage(clientSocket, GetSocketFromName(targetedOtherPlayerName), sendMsg);
                                                    break;
                                                }
                                            }
                                            catch (Exception) { }

                                            // If you enjoy battling on screens, please feel free to try out 'BattleScreens: Multiplayer Shooter' on all iOS devices that it doesn't not sometimes work on.
                                            // Do not refund
                                            try
                                            {
                                                if (sendMsg.Substring(0, 8) == "<Battle>")
                                                {
                                                    // Send the complete message to both player's sockets
                                                    SendGameMessage(clientSocket, "", sendMsg);
                                                    SendGameMessage(GetSocketFromName(targetedOtherPlayerName), "", sendMsg);
                                                    break;
                                                }
                                            }
                                            catch (Exception) { }

                                            // a player was killed!
                                            try
                                            {
                                                if (sendMsg.Substring(0, 11) == "You killed ")
                                                {
                                                    // Send the complete message to the player socket
                                                    SendGameMessage(clientSocket, "", sendMsg);

                                                    // Tell killed player client to disconnect
                                                    SendDeathMessage(GetSocketFromName(targetedOtherPlayerName), "Wasted.");

                                                    // Lost the player message
                                                    String output = targetedOtherPlayerName + " has been killed.";

                                                    // Debug message to server console
                                                    Console.WriteLine(output);

                                                    // Inform the other players
                                                    SendChatMessage(output);

                                                    playersTable.setFieldFromName(targetedOtherPlayerName, "null", "currentRoom");

                                                    // playerDictionary is already locked
                                                    // Sanity check, then remove player information from playerDictionary
                                                    if (playerSocketDictionary.ContainsKey(targetedOtherPlayerName))
                                                        playerSocketDictionary.Remove(targetedOtherPlayerName);

                                                    // Update all other players current client list
                                                    SendClientList();
                                                    break;
                                                }
                                            }
                                            catch (Exception) { }

                                            // WIN STATE!!!Now it's a game
                                            try
                                            {
                                                if (sendMsg.Substring(0, 5) == "<Win>")
                                                {
                                                    // Send the complete message to the player socket
                                                    SendGameMessage(clientSocket, "", sendMsg);

                                                    // Lost the player message
                                                    String output = clientName + " has reached the end!";

                                                    // Debug message to server console
                                                    Console.WriteLine(output);

                                                    // Inform the other players
                                                    SendChatMessage(output);

                                                    // Update all other players current client list
                                                    SendClientList();

                                                    break;
                                                }
                                            }
                                            catch (Exception) { }

                                            // If sendMsg conditions have not returned true for any of the above specific cases, then send the message to the player socket
                                            SendGameMessage(clientSocket, "", sendMsg);
                                        }
                                    }
                                    break;

                                case CreateNewUserMsg.ID:
                                    {
                                        CreateNewUserMsg createNewUserMessage = (CreateNewUserMsg)message;
                                        String newLoginInfo = createNewUserMessage.msg;

                                        // Safe to split with this as no spaces allowed in username or password
                                        String[] processedLoginInfo = newLoginInfo.Split(' ');

                                        // If length == 1 then the client is performing a username availability check on the database
                                        if (processedLoginInfo.Length == 1)
                                        {
                                            // Here we not only check if the name has been taken by another user, but also if an NPC or item has that name.
                                            // The game would be unpredictable if names were replicated from a controls standpoint, even if unique IDs were used.
                                            // For instance the control: "pickpocket guard" would be ambiguous were a player called 'guard' and an npc called 'guard' were both in the same room.
                                            if (loginsTable.queryExists(processedLoginInfo[0], "name") || npcsTable.queryExists(processedLoginInfo[0], "name") || itemsTable.queryExists(processedLoginInfo[0], "name"))
                                            {
                                                SendNewUserMessage(clientSocket, "NameTaken");
                                            }
                                            else
                                            {
                                                SendNewUserMessage(clientSocket, "NameAvailable");
                                            }
                                        }
                                        else
                                        {
                                            // The position of the user name in the message
                                            String userName = processedLoginInfo[6];

                                            // Someone else may have taken the userName since it was checked last so do one more quick check
                                            if (loginsTable.queryExists(userName, "name"))
                                            {
                                                SendNewUserMessage(clientSocket, "NameTaken");
                                            }
                                            else
                                            {
                                                // Tells the client to close it's 'registerNewUser' window
                                                SendNewUserMessage(clientSocket, "Success");

                                                // Get the unique id first so as to use the same ID in the player table and the login table to link them together
                                                String uniqueID = GetNextUniqueID().ToString();

                                                // Add the new user details to the logins table
                                                loginsTable.AddEntry(new string[]
                                                {
                                                    // Starting values
                                                    userName,                       // name
                                                    processedLoginInfo[7],          // password
                                                    processedLoginInfo[8],          // salt
                                                    "false",                        // is logged in
                                                    uniqueID                        // Id (not currently used as each name is unique)
                                                });

                                                // Add the new player details to the players table
                                                playersTable.AddEntry(new string[]
                                                {
                                                    userName,                       // name
                                                    uniqueID,                       // Id (the same one as used in the login table)
                                                    startRoom,                      // currentRoom
                                                    processedLoginInfo[0],          // strength
                                                    processedLoginInfo[1],          // dexterity
                                                    processedLoginInfo[2],          // constitution
                                                    processedLoginInfo[3],          // intelligence
                                                    processedLoginInfo[4],          // wisdom
                                                    processedLoginInfo[5],          // charisma
                                                    Character.getStartingHitPoints(processedLoginInfo[2]).ToString(),       // hitPoints (adjusted from constitution)
                                                    Character.getStartingHitPoints(processedLoginInfo[2]).ToString(),       // maxHitPoints (adjusted from constitution)
                                                    Character.getStartingAttackModifier(processedLoginInfo[0]).ToString(),  // attackModifier (adjusted from strength)
                                                    Character.UnarmedAttackDamage,                                          // attackDamage (unarmed at start)
                                                    Character.getStartingArmourClass(processedLoginInfo[1]).ToString()      // armourClass (adjusted from dexterity)
                                                });
                                            }
                                        }
                                    }
                                    break;

                                case LoginMsg.ID:
                                    // Parse the message
                                    LoginMsg loginAttempt = (LoginMsg)message;
                                    String loginAttemptMessage = loginAttempt.msg;
                                    String[] processedLoginMessage = loginAttemptMessage.Split(' ');

                                    // The position of the user name in the message
                                    String submittedUserName = processedLoginMessage[0];

                                    // First check is whether the logins table contains a record of this username
                                    if (!loginsTable.queryExists(submittedUserName, "name"))
                                    {
                                        // If no data present then send failure message
                                        SendLoginMessage(clientSocket, "LoginFailed");
                                    }
                                    // Else if client is requesting salt
                                    else if (processedLoginMessage[1] == "RequestSalt")
                                    {
                                        // Check if the user is already logged in elsewhere
                                        if (loginsTable.getStringFieldFromName(submittedUserName, "isLoggedIn") == "false")
                                        {
                                            // Get the stored salt associated with that userName and send it to the client
                                            String salt = loginsTable.getStringFieldFromName(submittedUserName, "passwordSalt");
                                            SendLoginMessage(clientSocket, salt);
                                        }
                                        else
                                        {
                                            // The user is already logged in. Only one login per user
                                            SendLoginMessage(clientSocket, "UserAlreadyLoggedIn");
                                        }
                                    }
                                    else
                                    {
                                        // Get the database copy of the hashed salted password
                                        String databaseHash = loginsTable.getStringFieldFromName(submittedUserName, "passwordHash");
                                        
                                        // Compare against the hashed salted password sent in the login message
                                        if (processedLoginMessage[1] == databaseHash)
                                        {
                                            // If identical then the player has successfully authenticated themselves
                                            SendLoginMessage(clientSocket, "LoginAccepted");

                                            // Add to the socket dictionary for easy socket look up
                                            playerSocketDictionary[submittedUserName] = clientSocket;

                                            // Set user as logged in so that no other users can log in using this information
                                            loginsTable.setFieldFromName(submittedUserName, "true", "isLoggedIn");

                                            // Send an updated client list to every socket in the playerSocket Dictionary
                                            SendClientList();

                                            // Get the current room from the playerTable to start from in this session.
                                            // This will either be the last room the player was in before logging out, or if a first time user, the dungeon start room
                                            String userStartRoom = playersTable.getStringFieldFromName(submittedUserName, "currentRoom");

                                            // Initial room description message for client
                                            SendGameMessage(clientSocket, "", controls.describeRoom(userStartRoom, submittedUserName, loginsTable));
                                        }
                                        else
                                        {
                                            SendLoginMessage(clientSocket, "LoginFailed");
                                        }
                                    }
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
                    String output = "Lost client: " + GetNameFromSocket(clientSocket);

                    // Debug message to server console
                    Console.WriteLine(output);

                    // Inform the other players
                    SendChatMessage(output);

                    // Try to adjust the isLoggedIn field as the client may disconnect before they have logged in successfully
                    try
                    {
                        // Allow the client to log back in if disconnected
                        loginsTable.setFieldFromName(GetNameFromSocket(clientSocket), "false", "isLoggedIn");
                    }
                    catch { }

                    // Always lock the list!!!
                    lock (playerSocketDictionary)
                    {
                        try
                        {
                            // Sanity check, then remove player information from playerDictionary
                            if (playerSocketDictionary.ContainsKey(GetNameFromSocket(clientSocket)))
                                playerSocketDictionary.Remove(GetNameFromSocket(clientSocket));
                        }
                        catch (Exception)
                        { }
                    }

                    // Update all other players current client list
                    SendClientList();
                }
            }
        }

        /*
         * The main entry point for the program
         */
        static void Main(string[] args)
        {
            // Create the database
            database = new SQLDatabase("database");

            // Good to keep the following column names strings in this scope as they are critical to the program.
            // Initially I thought to create a seperate static class to hold this information or to give it to
            // the database or table class to initialise, but all tables are initialised from commands in this
            // scope, new players, IDs, and logins are created and added in this Program.cs, so it makes sense to 
            // keep this information here also.

            // Create the login table
            loginsTable = database.addLoginTable("logins",
                "name varchar(20) NOT NULL, " +
                "passwordHash varchar(512) NOT NULL, " +
                "passwordSalt varchar(512) NOT NULL, " +
                "isLoggedIn varchar(8), " +
                "id int NOT NULL");

            // Create the dungeon table 
            dungeonTable = database.addDungeonTable("dungeon",
                "name varchar(24) NOT NULL, " +
                "id int NOT NULL, " +
                "north varchar(24), " +
                "south varchar(24), " +
                "east varchar(24), " +
                "west varchar(24), " +
                "description varchar(256) NOT NULL, " +
                "isLocked varchar(8) NOT NULL");
            
            // Create the players table
            playersTable = database.addPlayersTable("players",
                "name varchar(24) NOT NULL, " +
                "id int NOT NULL, " +
                "currentRoom varchar(24) NOT NULL, " +
                "strength int NOT NULL, " +
                "dexterity int NOT NULL, " +
                "constitution int NOT NULL, " +
                "intelligence int NOT NULL, " +
                "wisdom int NOT NULL, " +
                "charisma int NOT NULL," +
                "hitpoints int NOT NULL, " +
                "maxHitPoints int NOT NULL, " +
                "attackModifier int NOT NULL, " +
                "attackDamage int NOT NULL, " +
                "armourClass int NOT NULL");

            // Create the items table
            itemsTable = database.addItemsTable("items",
                "name varchar(24) NOT NULL, " +
                "id int NOT NULL, " +
                "room varchar(24), " +
                "owner varchar(24), " +
                "description varchar(256), " +
                "isEquipped varchar(8), " +
                "useMessage varchar(256), " +
                "healAmount int, " +
                "damage int, " +
                "armourClass int, " +
                "roomUnlocks varchar(128)");

            // This ID table will simply stores the next available ID to use
            IDTable = database.addIDTable("ID",
                "name varchar(24) NOT NULL, " +
                "nextID int");

            // Create the NPC table
            npcsTable = database.addNPCTable("npcs",
                "name varchar(20) NOT NULL, " +
                "id int NOT NULL, " +
                "room varchar(24), " +
                "speech varchar(512), " +
                "description varchar(512), " +
                "strength int NOT NULL, " +
                "dexterity int NOT NULL, " +
                "constitution int NOT NULL, " +
                "intelligence int NOT NULL, " +
                "wisdom int NOT NULL, " +
                "charisma int NOT NULL");

            // Initialise the ID table first as will be used to assign incrementing ids to all the other database table items
            IDTable.AddIDEntry("next", 0 );
            
            // Initialise tables with preset values found in the static classes
            ForestCastleDungeon.Init(dungeonTable);
            NPCs.Init(npcsTable);
            Items.Init(itemsTable);

            // Initialise the Controls class and pass in references to all needed database tables
            controls = new Controls(dungeonTable, playersTable, itemsTable, npcsTable, loginsTable);

            // Listening socket initialisation
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Local address for developing purposes!
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500)); // 192.168.1.224

            // Listen for new clients
            serverSocket.Listen(32);

            bool bQuit = false;

            // Console message to boast about being a server!
            Console.WriteLine("This is the server!");

            while (!bQuit)
            {
                // When there is a new connection, create a new socket reference
                Socket serverClient = serverSocket.Accept();

                // Start a new thread assigned to this socket
                Thread myThread = new Thread(ReceiveClientProcess);
                myThread.Start(serverClient);

                Thread.Sleep(500);

                // Update all players with up to date client list
                SendClientList();
            }
        }

        /*
         * Returns an integer value from the id table. Also increments that value and sets it in the table ready for the next retrieval
         */
        static public int GetNextUniqueID()
        {
            // get the current unused value from the ID table, field nextID
            int i = IDTable.getIntFieldFromName("next", "nextID");

            // Set the field of the nextID with an unused incremented value
            IDTable.setFieldFromName("next", i+1, "nextID");

            // return the first get database value
            return i;
        }
    }
}
