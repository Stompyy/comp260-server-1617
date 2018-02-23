using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon
{
    public class Room
    {
        public Room(String name, String desc) //, Player[] m_PlayersInRoom)
        {
            this.desc = desc;
            this.name = name;
            m_PlayersInRoom = new List<Player>();
        }

        public List<Player> GetPlayersInRoom()
        {
             return m_PlayersInRoom;
        }

        public void RemovePlayer(ref Player player)//ref Player player)
        {
            //var tempPlayer = (Player)player;
            // Sanity check
            if (this.m_PlayersInRoom.Contains(player))//tempPlayer))
                this.m_PlayersInRoom.Remove(player);
        }

        public void AddPlayer(ref Player player)
        {
            //var tempPlayer = (Player)player;
            // Sanity check
            if (!this.m_PlayersInRoom.Contains(player))
                this.m_PlayersInRoom.Add(player);
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


        public String name = "";
        public String desc = "";
        private List<Player> m_PlayersInRoom;
        public String[] exits = new String[4];
        public static String[] exitNames = { "NORTH", "SOUTH", "EAST", "WEST" };
    }

}


