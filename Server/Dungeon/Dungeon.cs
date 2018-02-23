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

        public void Init()
        {
            roomMap = new Dictionary<string, Room>();
            {
                var room = new Room("Room 0", "You are standing in the entrance hall\r\nAll adventures start here\n");
                room.north = "Room 1";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 1", "You are in room 1\r\n");
                room.south = "Room 0";
                room.west = "Room 3";
                room.east = "Room 2";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 2", "You are in room 2\r\n");
                room.north = "Room 4";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 3", "You are in room 3\r\n");
                room.east = "Room 1";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 4", "You are in room 4\r\n");
                room.south = "Room 2";
                room.west = "Room 5";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room("Room 5", "You are in room 5\r\n");
                room.south = "Room 1";
                room.east = "Room 4";
                roomMap.Add(room.name, room);
            }
            
            // Initialise the start room for all Player instances
            m_StartRoom = roomMap["Room 0"];
        }

        public Room GetStartRoom() { return m_StartRoom; }

        // Returns the description member of the Room instance, and a list of the exits
        public string DescribeRoom(Room currentRoom)
        {
            //String firstMessage = "";

            String message = currentRoom.desc + "\r\n";
            message += "Exits\r\n";
            for (var i = 0; i < currentRoom.exits.Length; i++)
            {
                if (currentRoom.exits[i] != null)
                {
                    message += Room.exitNames[i] + " ";
                }
            }
            return message;
        }
        public string Process(ref Room currentRoom, String key)
        {
            String[] input = key.Split(' ');

            String message = "";

            switch (input[0].ToLower())
            {
                case "help":
                    message = "Commands are ....\r\n";
                    message += "help - for this screen\r\n";
                    message += "look - to look around\r\n";
                    message += "go [north | south | east | west]  - to travel between locations\r\n";
                    
                    break;

                case "look":
                    message = "The players in this room are: ";
                    foreach (Player player in currentRoom.GetPlayersInRoom())
                        message += player.GetName() + ", ";
                    break;

                case "say":
                    message = "You say ";
                    for (var i = 1; i < input.Length; i++)
                    {
                        message += input[i] + " ";
                    }
                    break;

                case "whereami":
                    message = DescribeRoom(currentRoom);
                    break;

                case "go":
                    // is arg[1] sensible?
                    if ((input[1].ToLower() == "north") && (currentRoom.north != null))
                    {
                        currentRoom = roomMap[currentRoom.north];
                    }
                    else
                    {
                        if ((input[1].ToLower() == "south") && (currentRoom.south != null))
                        {
                            currentRoom = roomMap[currentRoom.south];
                        }
                        else
                        {
                            if ((input[1].ToLower() == "east") && (currentRoom.east != null))
                            {
                                currentRoom = roomMap[currentRoom.east];
                            }
                            else
                            {
                                if ((input[1].ToLower() == "west") && (currentRoom.west != null))
                                {
                                    currentRoom = roomMap[currentRoom.west];
                                }
                                else
                                {
                                    //handle error
                                    message = "\r\nERROR";
                                    message += "\r\nCan not go " + input[1] + " from here";
                                    message += "\r\nPress any key to continue";
                                }
                            }
                        }
                    }
                    break;

                default:
                    //handle error
                    message = "\r\nERROR";
                    message += "\r\nCan not " + key;
                    break;
            }

            return message;
        }
    }
}
