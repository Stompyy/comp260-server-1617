using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dungeon
{
    public class Character
    {
        // Constructor
        public Character(String name, List<Item> startItems)
        {
            m_Name = name;
            m_Inventory = startItems;
        }

        // Standard health float
        private float m_Health = 1.0f;
        public float Health { get { return m_Health; } set { m_Health = value; } }
        public void ApplyDamage(float damage) { m_Health -= damage; }

        // Name
        private String m_Name;
        public String Name { get { return m_Name; } }

        // Inventory to hold items
        private List<Item> m_Inventory;
        public ref List<Item> Inventory { get { return ref m_Inventory; } }

        public void AddItem(Item item) { m_Inventory.Add(item); }
        public bool DropItem(String itemName, ref Room currentRoom)
        {
            foreach (Item item in m_Inventory)
            {
                if (item.Name == itemName)
                {
                    m_Inventory.Remove(item);
                    currentRoom.ItemList.Add(item);
                    return true;
                }
            }
            return false;
        }
    }

    // Player class adds current room and player functionality to Character class
    public class Player : Character
    {
        public Player(String name, List<Item> startItems) : base(name, startItems)
        {
            
        }

        // No point in a for loop here as would have to do the same amount of work assigning the Player member variables to an array to loop through first
        public void Init(String[] characterSheet)
        {
            m_Strength = Convert.ToInt32(characterSheet[0]);
            m_Dexterity = Convert.ToInt32(characterSheet[1]);
            m_Constitution = Convert.ToInt32(characterSheet[2]);
            m_Intelligence = Convert.ToInt32(characterSheet[3]);
            m_Wisdom = Convert.ToInt32(characterSheet[4]);
            m_Charisma = Convert.ToInt32(characterSheet[5]);
        }

        // Currently occupied Room
        private Room m_CurrentRoom;
        public ref Room GetCurrentRoomRef { get { return ref m_CurrentRoom; } }
        public Room CurrentRoom { set { m_CurrentRoom = value; } }

        // D&D style character sheet
        private int
            m_Strength,
            m_Dexterity,
            m_Constitution,
            m_Intelligence,
            m_Wisdom,
            m_Charisma;

        public int Strength { get { return m_Strength; } }
        public int Dexterity { get { return m_Dexterity; } }
        public int Constitution { get { return m_Constitution; } }
        public int Intelligence { get { return m_Intelligence; } }
        public int Wisdom { get { return m_Wisdom; } }
        public int Charisma { get { return m_Charisma; } }
    }

    // NPC
    public class NPC : Character
    {
        public NPC(String name, List<Item> startItems, String description, String speech) : base(name, startItems)
        {
            m_Description = description;
            m_Speech = speech;
        }

        private String m_Description;
        public String Description { get { return m_Description; } }

        private String m_Speech;
        public String Speech { get { return m_Speech; } }
    }

    // Friendly
    public class Friendly : NPC
    {
        public Friendly(String name, List<Item> startItems, String description, String speech) : base(name, startItems, description, speech) { }
    }
}
