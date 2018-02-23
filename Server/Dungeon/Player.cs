using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dungeon
{
    public class Player
    {
        // Running count of number of Player instances created
        static int numberOfPlayers;

        // This Player instances unique identifier
        private int m_PlayerIDNumber;

        // String identifier for this player instance
        private String m_Name;

        // Standard health float
        private float m_Health = 1.0f;

        // Starting room number
        private Room m_CurrentRoom;

        // Constructor
        public Player()
        {
            // This player's 0 based identifier can be the previous current number of players. i.e. Creating a second player when one already exists gives that second player ID = 1
            this.m_PlayerIDNumber = numberOfPlayers;

            // Increment static number of Players 
            numberOfPlayers++;
        }

        public int GetPlayerID() { return m_PlayerIDNumber; }
        public String GetName() { return m_Name; }
        public void SetName(String name) { m_Name = name; }

        public float GetHealth() { return m_Health; }
        public void ApplyDamage( /*const*/ float damage) { m_Health -= damage; }

        public ref Room GetRoom() { return ref m_CurrentRoom; }
        public void SetRoom(Room currentRoom) { m_CurrentRoom = currentRoom; }

        public int GetNumberOfPlayers()
        {
            return numberOfPlayers;
        }
    }
    
    class Weapon
    {
        private float m_AttackPower;

        // Constructor
        public Weapon(float attackPower)
        {
            this.m_AttackPower = attackPower;
        }
    }
}
