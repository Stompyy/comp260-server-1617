using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dungeon
{
    // Base class item. Can also be used for unimportant objects with no specific functionality
    public class Item
    {
        public Item(String name, float weight, float value, String description)
        {
            m_Name = name;
            m_Weight = weight;
            m_Value = value;
            m_Description = description;
        }

        public virtual String Use(ref Player player)
        {
            return "You should probably do that later.";
        }

        private String m_Name;
        public String Name { get { return m_Name; } }// set { m_Name = value; } }

        private float m_Weight;
        public float Weight { get { return m_Weight; } }// set { m_Weight = value; } }

        private float m_Value;
        public float Value { get { return m_Value; } }// set { m_Value = value; } }

        private String m_Description;
        public String Description { get { return m_Description; } }
    }

    // Weapon class adds damage field to Item class
    public class Weapon : Item
    {
        public Weapon(String name, float weight, float value, String description, int damage) : base (name, weight, value, description)
        {
            m_Damage = damage;
        }

        private int m_Damage;
        public int Damage { get {return m_Damage; } set {m_Damage = value; } }
    }

    // Armour class adds defense field to Item class
    public class Armour : Item
    {
        public Armour(String name, float weight, float value, String description, int armourClassModifier) : base (name, weight, value, description)
        {
            m_ArmourClassModifier = armourClassModifier;
        }

        private int m_ArmourClassModifier;
        public int ArmourClassModifier { get { return m_ArmourClassModifier; } set { m_ArmourClassModifier = value; } }
    }

    // Health replenishing item
    public class HealthItem : Item
    {
        public HealthItem(String name, float weight, float value, String description) : base(name, weight, value, description)
        {

        }

        public override string Use(ref Player player)
        {
            // Refill health
            player.HitPoints = player.MaxHitPoints;

            // Remove from inventory
            player.Inventory.Remove(this);

            //throw new NotImplementedException();
            return "The item heals you.\r\n\r\nYour Health is now full at " + player.MaxHitPoints;
        }

    }
}
