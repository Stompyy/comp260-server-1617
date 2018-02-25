using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Dungeon
{
    public class DungeonClass
    {
        Dictionary<String, Room> roomMap;

        // The room in which all Player instances will start the game in
        private Room m_StartRoom;
        public Room StartRoom { get { return m_StartRoom; } set { m_StartRoom = value; } }

        Item grog = new Item("grog", 0.5f, 4.0f, "Ah sweet grog. I love you grog.");
        Weapon club = new Weapon("club", 2.0f, 1.0f, "This is a large stick. Or a small branch.", 10.0f);
        Weapon sword = new Weapon("sword", 5.0f, 100.0f, "This is a large sword. It will kill easily. Have fun.", 85.0f);
        Armour shield = new Armour("shield", 5.0f, 20.0f, "It really is a shield.", 50.0f);

        Friendly chicken = new Friendly("chicken", new List<Item>(), "The chicken looks at you.", "The chicken says nothing. It looks angry.");

        public void Init()
        {
            roomMap = new Dictionary<string, Room>();
            {
                var room = new Room(
                    "Room 0", 
                    "You are standing in the entrance hall\r\nAll adventures start here\n", 
                    new List<Item> { grog, club, shield }, 
                    new List<NPC>()
                    );
                room.north = "Room 1";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room(
                    "Room 1", 
                    "You are in room 1\r\n", 
                    new List<Item> { sword },
                    new List<NPC>()
                    );
                room.south = "Room 0";
                room.west = "Room 3";
                room.east = "Room 2";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room(
                    "Room 2", 
                    "You are in room 2\r\n", 
                    new List<Item> { shield },
                    new List<NPC>()
                    );
                room.north = "Room 4";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room(
                    "Room 3", 
                    "You are in room 3\r\n", 
                    new List<Item>(),
                    new List<NPC> { chicken }
                    );
                room.east = "Room 1";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room(
                    "Room 4", 
                    "You are in room 4\r\n", 
                    new List<Item>(),
                    new List<NPC>()
                    );
                room.south = "Room 2";
                room.west = "Room 5";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room(
                    "Room 5", 
                    "You are in room 5\r\n", 
                    new List<Item>(),
                    new List<NPC>()
                    );
                room.south = "Room 1";
                room.east = "Room 4";
                roomMap.Add(room.name, room);
            }
            
            // Initialise the start room for all Player instances
            m_StartRoom = roomMap["Room 0"];
        }

        // Returns the description member of the Room instance, and a list of the exits
        public string DescribeRoom(Room currentRoom)
        {
            String message = "\r\n" + currentRoom.description;
            message += "\r\n\r\nExits:\r\n";
            for (var i = 0; i < currentRoom.exits.Length; i++)
            {
                if (currentRoom.exits[i] != null)
                {
                    message += Room.exitNames[i] + ", ";
                }
            }

            if (currentRoom.ItemList.Count > 0)
            {
                message += "\r\n\r\nIn the room you see the following items: ";
                for (int i = 0; i < currentRoom.ItemList.Count; i++)
                {
                    if (currentRoom.ItemList[i] != null)
                    {
                        message += currentRoom.ItemList[i].Name + ", ";
                    }
                }
            }

            if (currentRoom.NPCList.Count > 0)
            {
                message += "\r\n\r\nIn the room are the following NPCs: ";
                for (int i = 0; i < currentRoom.NPCList.Count; i++)
                {
                    if (currentRoom.NPCList[i] != null)
                    {
                        message += currentRoom.NPCList[i].Name + ", ";
                    }
                }
            }
            return message;
        }

        // Updates the current room value for the player. Also Updates the old and new room occupents list
        private void UpdateRoom(ref Room currentRoom, String playerName, String direction)
        {
            lock (roomMap)
            {
                currentRoom.RemovePlayer(playerName);
                currentRoom = roomMap[direction];
                currentRoom.AddPlayer(playerName);
            }
        }

        public string Update(ref Room currentRoom, ref Player player, String currentPlayerName, String inputMessage)//
        {
            String[] input = inputMessage.Split(' ');

            String outputMessage = "";

            switch (input[0].ToLower())
            {
                case "help":
                    outputMessage = "\r\nCommands are ....\r\n";
                    outputMessage += "help - for this screen\r\n";
                    outputMessage += "look around - to look around\r\n";
                    outputMessage += "look at inventory - to look at inventory\r\n";
                    outputMessage += "look at ... - to inspect an item in your inventory\r\n";
                    outputMessage += "pick up ... - to add an item in the room to your inventory\r\n";
                    outputMessage += "drop ... - to drop an item from your inventory into the room\r\n";
                    outputMessage += "say ... - to message all players in your current room\r\n";
                    outputMessage += "talk to ... - to talk to an NPC in the room\r\n";
                    outputMessage += "go [north | south | east | west]  - to travel between locations";
                    
                    break;

                case "look":
                    try
                    {
                        if (input[1].ToLower() == "around")
                        {
                            // "Look Around" will return the room description
                            outputMessage = DescribeRoom(currentRoom);

                            // And the current players in the room
                            if (currentRoom.PlayersInRoom.Count > 1)
                            {
                                outputMessage += "\r\nThe other players in this room are: ";

                                foreach (String playerNameInRoom in currentRoom.PlayersInRoom)
                                {
                                    if (playerNameInRoom != currentPlayerName)
                                        outputMessage += playerNameInRoom + ", ";
                                }
                                return outputMessage;
                            }
                            else
                            {
                                outputMessage += "\r\n\r\nThere are no other players in the room.";
                                return outputMessage;
                            }
                        }
                        // "Look At ..." 
                        else if (input[1].ToLower() == "at")
                        {
                            // List items in player's inventory
                            if ((input[2].ToLower() == "inventory"))
                            {
                                if (player.Inventory.Count > 0)
                                {
                                    outputMessage = "\r\nInventory:";
                                    foreach (Item item in player.Inventory)
                                    {
                                        outputMessage += "\r\n" + item.Name;
                                    }
                                    return outputMessage;
                                }
                                else
                                {
                                    return "\r\nYou are not carrying anything.";
                                }
                            }
                            // Will try and return a description of the item in the players inventory with ... name
                            for (int i = 0; i < player.Inventory.Count; i++)
                            {
                                if (player.Inventory[i].Name == (input[2].ToLower()))
                                {
                                    outputMessage += "\r\n" + player.Inventory[i].Description;
                                    return outputMessage;
                                }
                            }
                            outputMessage += "You do not possess a " + input[2].ToLower() + " in your inventory.";
                            return outputMessage;
                        }
                        else
                        {
                            outputMessage = "\r\nYou don't really want to look there do you";
                            return outputMessage;
                        }
                    }
                    catch (Exception)
                    {
                        //handle error
                        return "\r\nWhere would you like to look?";
                    }

                case "say":
                    outputMessage = "Room chat: <" + currentPlayerName + "> ";

                    for (var i = 1; i < input.Length; i++)
                    {
                        outputMessage += input[i] + " ";
                    }
                    break;

                case "talk":
                    if (currentRoom.NPCList.Count == 0)
                    {
                        // The room has no NPCs in. Also checked in the TalkToNPC() below
                        return "There are no NPCs in this room to talk to.";
                    }
                    try
                    {
                        if (input[1].ToLower() == "to")
                        {
                            return currentRoom.TalkToNPC(input[2].ToLower());
                        }
                        else
                        {
                            return "Try to 'talk to ...' NPCs instead.";
                        }
                    }
                    catch(Exception)
                    {
                        return "Try to 'talk to ...' NPCs instead.";
                    }

                case "whereami":
                    return "\r\n" + DescribeRoom(currentRoom);

                case "drop":
                    try
                    {
                        if (player.DropItem(input[1].ToLower(), ref currentRoom))
                        {
                            return "You drop the " + input[1].ToLower() + " on the floor.";
                        }
                        else
                        {
                            return "You need to have a " + input[1].ToLower() + " if you want to drop it.";
                        }
                    }
                    catch(Exception)
                    {
                        return "What would you like to drop?";
                    }

                case "pick":
                    try
                    {
                        if (input[1].ToLower() == "up")
                        {
                            if (currentRoom.PickUp(ref player, input[2].ToLower()))
                                return "\r\nYou pick up the " + input[2].ToLower() + " and add it to your inventory.";
                            else
                                return "\r\nThere is no " + input[2].ToLower() + " in the room.";
                        }
                        else
                        {
                            return "\r\nThat does not need picking right now thank you";
                        }
                    }
                    catch (Exception)
                    {
                        //handle error
                        outputMessage = "\r\nWhat would you like to pick?";
                    }
                    break;

                case "inventory":
                    {
                        if (player.Inventory.Count > 0)
                        {
                            outputMessage = "\r\nCurrent inventory:";
                            foreach (Item item in player.Inventory)
                                outputMessage += "\r\n" + item.Name;
                        }
                        else
                            return "\r\nYou are not carrying anything.";
                    }
                    break;

                case "go":
                    // is arg[1] sensible?
                    if ((input[1].ToLower() == "north") && (currentRoom.north != null))
                    {
                        UpdateRoom(ref currentRoom, currentPlayerName, currentRoom.north);
                        outputMessage += DescribeRoom(currentRoom);
                    }
                    else
                    {
                        if ((input[1].ToLower() == "south") && (currentRoom.south != null))
                        {
                            UpdateRoom(ref currentRoom, currentPlayerName, currentRoom.south);
                            outputMessage += DescribeRoom(currentRoom);
                        }
                        else
                        {
                            if ((input[1].ToLower() == "east") && (currentRoom.east != null))
                            {
                                UpdateRoom(ref currentRoom, currentPlayerName, currentRoom.east);
                                outputMessage += DescribeRoom(currentRoom);
                            }
                            else
                            {
                                if ((input[1].ToLower() == "west") && (currentRoom.west != null))
                                {
                                    UpdateRoom(ref currentRoom, currentPlayerName, currentRoom.west);
                                    outputMessage += DescribeRoom(currentRoom);
                                }
                                else
                                {
                                    //handle error
                                    outputMessage = "\r\nERROR";
                                    outputMessage += "\r\nCan not go " + input[1] + " from here.";
                                }
                            }
                        }
                    }
                    break;

                default:
                    //handle error
                    outputMessage = "\r\nERROR";
                    outputMessage += "\r\nCan not " + inputMessage + ". It's probably not a good idea.";
                    break;
            }

            return outputMessage;
        }
    }
}
