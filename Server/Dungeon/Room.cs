using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon
{
    public class Room
    {
        public Room(String name, String desc, List<Item> itemsInRoom, List<NPC> NPCsInRoom, bool isLocked)
        {
            this.description = desc;
            this.name = name;
            m_ItemList = itemsInRoom;
            m_NPCList = NPCsInRoom;
            m_NamesOfPlayersInRoom = new List<String>();
            PlayersInRoom = new List<String>();
            this.IsLocked = isLocked;
        }

        public void RemovePlayer(String playerName)
        {
            // Sanity check - DOING THIS ALL TWICE!!!?
            if (this.m_NamesOfPlayersInRoom.Contains(playerName))
                this.m_NamesOfPlayersInRoom.Remove(playerName);
        }

        public void AddPlayer(String playerName)
        {
            // Sanity check
            if (!this.m_NamesOfPlayersInRoom.Contains(playerName))
                this.m_NamesOfPlayersInRoom.Add(playerName);
        }

        public String north
        {
            get { return exits[0]; }
            set { exits[0] = value; }
        }

        public String south
        {
            get { return exits[1]; }
            set { exits[1] = value; }
        }

        public String east
        {
            get { return exits[2]; }
            set { exits[2] = value; }
        }
        public String west
        {
            get { return exits[3]; }
            set { exits[3] = value; }
        }

        // Room name
        public String name = "";

        // Room description
        public String description = "";

        // List of players' names currently occupying room. Only need String name here as can use a dictionary lookup in the Program.cs to retrieve the Player class instance. No point in storing every player/ref twice.
        private List<String> m_NamesOfPlayersInRoom;
        public List<String> PlayersInRoom { get { return m_NamesOfPlayersInRoom; } set { m_NamesOfPlayersInRoom = value; } }

        public String[] exits = new String[4];
        public static String[] exitNames = { "NORTH", "SOUTH", "EAST", "WEST" };

        // Items to be found in the room
        private List<Item> m_ItemList;
        public List<Item> ItemList { get { return m_ItemList; } set { m_ItemList = value; } }
        public bool PickUp(ref Player player, String itemName)
        {
            if (m_ItemList.Count > 0)
            {
                foreach (Item item in m_ItemList)
                {
                    if (item.Name == itemName)
                    {
                        m_ItemList.Remove(item);
                        player.AddItem(item);
                        return true;
                    }
                }
            }
            Console.WriteLine(itemName + " not in room.");
            return false;
        }

        // Non playable characters in this room
        private List<NPC> m_NPCList;
        public List<NPC> NPCList { get { return m_NPCList; } set { m_NPCList = value; } }
        public bool IsLocked { get; set; }
        public String TalkToNPC(String npcName)
        {
            if (m_NPCList.Count > 0)
            {
                foreach (NPC npc in m_NPCList)
                {
                    if (npc.Name == npcName)
                    {
                        return npc.Speech;
                    }
                }
                return "There is not a " + npcName + " in the room to talk to.";
            }
            return "There are no NPCs in the room.";
        }
    }

}


