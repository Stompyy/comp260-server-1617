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

        // Item instantiation
        Item grog = new Item("grog", 0.5f, 4.0f, "Ah sweet grog. I love you grog.");

        static HealthItem potion = new HealthItem("potion", 1.0f, 20.0f, "This potion will replenish your health.");
        static HealthItem sweetRoll = new HealthItem("sweetroll", 2.0f, 3.0f, "Wait what game is this?");
        static HealthItem egg = new HealthItem("egg", 0.1f, 5.0f, "There is an angry chicken somewhere. The egg looks tasty.");

        Weapon club = new Weapon("club", 2.0f, 1.0f, "This is a large stick. Or a small branch.", 5);
        Weapon sword = new Weapon("sword", 5.0f, 100.0f, "This is a large sword. It will kill easily. Have fun.", 8);
        Weapon masterSword = new Weapon("mastersword", 3.0f, 120.0f, "There is an image of the triforce on the hilt.", 8);
        static Weapon busterSword = new Weapon("bustersword", 7.0f, 150.0f, "This sword is much larger than you. It can hold three materia.", 12);

        Armour shield = new Armour("shield", 5.0f, 20.0f, "It is wildly over powered.", 4);
        Armour fullPlate = new Armour("fullplate", 50.0f, 100.0f, "It is so bulky you don't think you can even hold a shield at the same time.", 6);

        Friendly guard = new Friendly("guard", new List<Item> { busterSword, potion }, "The guard looks friendly.", "<guard> Beware of the monsters ahead. The chicken is alright though. You will want a sword soon though.");
        Friendly chicken = new Friendly("chicken", new List<Item> { egg }, "The chicken looks back at you.", "The chicken says nothing. It looks angry.");

        public void Init()
        {
            roomMap = new Dictionary<string, Room>();
            {
                var room = new Room(
                    "Room 0",
                    "You are standing in the entrance hall\r\nAll adventures start here\n",
                    new List<Item> { grog, club, shield },
                    new List<NPC> { guard },
                    false
                    );
                room.north = "Room 1";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room(
                    "Room 1", 
                    "You are in room 1\r\n", 
                    new List<Item> { sword },
                    new List<NPC>(),
                    false
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
                    new List<NPC>(),
                    false
                    );
                room.north = "Room 4";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room(
                    "Room 3", 
                    "You are in room 3\r\n", 
                    new List<Item>(),
                    new List<NPC> { chicken },
                    false
                    );
                room.east = "Room 1";
                roomMap.Add(room.name, room);
            }

            {
                var room = new Room(
                    "Room 4", 
                    "You are in room 4\r\n", 
                    new List<Item> { potion },
                    new List<NPC> { },
                    false
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
                    new List<NPC>(),
                    false
                    );
                room.south = "Room 1";
                room.east = "Room 4";
                roomMap.Add(room.name, room);
            }

            {
                var endRoom = new Room(
                    "End Room!",
                    "The final room, congratulations!",
                    new List<Item>(),
                    new List<NPC>(),
                    false
                    );
                //...
                //...
                //...
            }
            
            // Initialise the start room for all Player instances
            m_StartRoom = roomMap["Room 0"];
        }

        // The room in which all Player instances will start the game in
        private Room m_StartRoom;
        public Room StartRoom { get { return m_StartRoom; } set { m_StartRoom = value; } }

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
        public void UpdateRoom(ref Room currentRoom, String playerName, String direction)
        {
            lock (roomMap)
            {
                currentRoom.RemovePlayer(playerName);
                currentRoom = roomMap[direction];
                currentRoom.AddPlayer(playerName);
            }
        }
    }
}
