using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Dungeon
{
    public abstract class Character
    {
        // D&D style ability modifier
        private void AdjustAbilityModifier(int abilityScore, ref int abilityModifier)
        {
            switch (abilityScore)
            {
                case 1: abilityModifier += -5; break;
                case 2: abilityModifier += -4; break;
                case 3: abilityModifier += -4; break;
                case 4: abilityModifier += -3; break;
                case 5: abilityModifier += -3; break;
                case 6: abilityModifier += -2; break;
                case 7: abilityModifier += -2; break;
                case 8: abilityModifier += -1; break;
                case 9: abilityModifier += -1; break;
                case 10: abilityModifier += 0; break;
                case 11: abilityModifier += 0; break;
                case 12: abilityModifier += 1; break;
                case 13: abilityModifier += 1; break;
                case 14: abilityModifier += 2; break;
                case 15: abilityModifier += 2; break;
                case 16: abilityModifier += 3; break;
                case 17: abilityModifier += 3; break;
                case 18: abilityModifier += 4; break;
                default: break;
            }
        }

        // Constructor
        public Character(String name, List<Item> startItems)
        {
            m_Name = name;
            m_Inventory = startItems;
            m_HitPoints = m_MaxHitPoints;
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

            // D&D style set Attack modifer (Strength), Armour class (Dexterity), and Hit points (Constitution)
            AdjustAbilityModifier(m_Strength, ref m_AttackModifier);
            AdjustAbilityModifier(m_Dexterity, ref m_ArmourClass);
            AdjustAbilityModifier(m_Constitution, ref m_MaxHitPoints);
        }

        // D&D style character sheet
        // Default values for NPCs. Can use Init to set as different values
        private int
            m_Strength = 10,
            m_Dexterity = 10,
            m_Constitution = 10,
            m_Intelligence = 10,
            m_Wisdom = 10,
            m_Charisma = 10;

        public int Strength { get { return m_Strength; } }
        public int Dexterity { get { return m_Dexterity; } set { m_Dexterity = value; } }
        public int Constitution { get { return m_Constitution; } }
        public int Intelligence { get { return m_Intelligence; } }
        public int Wisdom { get { return m_Wisdom; } }
        public int Charisma { get { return m_Charisma; } }

        public String GetStats()
        {
            String outputMessage = Name + ": character sheet:\r\n\r\n";
            outputMessage += "Strength: " + m_Strength.ToString() + "\r\n";
            outputMessage += "Dexterity: " + m_Dexterity.ToString() + "\r\n";
            outputMessage += "Constitution: " + m_Constitution.ToString() + "\r\n";
            outputMessage += "Intelligence: " + m_Intelligence.ToString() + "\r\n";
            outputMessage += "Wisdom: " + m_Wisdom.ToString() + "\r\n";
            outputMessage += "Charisma: " + m_Charisma.ToString() + "\r\n\r\n";
            outputMessage += "Hitpoints: " + m_HitPoints + "\r\n";
            outputMessage += "Armour class: " + m_ArmourClass + "\r\n";
            outputMessage += "AttackModifier: " + m_AttackModifier + "\r\n";
            outputMessage += "Attack damage: 1d" + AttackDamage;
            return outputMessage;
        }

        // Standard health floats
        private int m_MaxHitPoints = 20;
        public int MaxHitPoints { get; set; } = 20;
        private int m_HitPoints;
        public int HitPoints { get { return m_HitPoints; } set { m_HitPoints = value; } }
        public int ApplyDamage(int damage) { m_HitPoints -= damage; return m_HitPoints; }

        // D&D style attack modifier
        private int m_AttackModifier;
        public int AttackModifier { get { return m_AttackModifier; } }

        // Damage. Default damage 1d3 for unarmed combat
        private int m_UnarmedAttackDamage = 3;
        private int m_AttackDamage = 3;
        public int AttackDamage { get { return m_AttackDamage; } set { m_AttackDamage = value; } }

        // Base Armour class (AC) rating for unarmoured play
        private int m_ArmourClass = 10;
        public int ArmourClass { get { return m_ArmourClass; } set { m_ArmourClass = value; } }

        // Name
        private String m_Name;
        public String Name { get { return m_Name; } }

        // Inventory to hold items
        private List<Item> m_Inventory;
        public ref List<Item> Inventory { get { return ref m_Inventory; } }

        // Inventory control
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

        // Give item to another player
        public bool GiveItem(String itemName, ref Player recipient)
        {
            foreach (Item item in m_Inventory)
            {
                if (item.Name == itemName)
                {
                    m_Inventory.Remove(item);
                    recipient.Inventory.Add(item);
                    return true;
                }
            }
            return false;
        }

        // Keeps track of equipped weapons and armour
        private int m_EquippedItemsCount = 0;
        public int EquippedItemsCount { get { return m_EquippedItemsCount; } set { m_EquippedItemsCount = value; } }
        private List<Item> m_EquippedItemList = new List<Item>();
        public List<Item> EquippedItems { get { return m_EquippedItemList; } }

        // Equip a weapon and update the attack damage
        public void EquipWeapon(Weapon weapon)
        {
            EquippedItemsCount += 1;
            m_EquippedItemList.Add(weapon);
            AttackDamage = weapon.Damage;
        }

        // Equip armour
        public void EquipArmour(Armour armour)
        {
            EquippedItemsCount += 1;
            m_EquippedItemList.Add(armour);

            // Have to reset the AC first in case already have it modified. 1 piece of armour at a time here.
            AdjustAbilityModifier(m_Dexterity, ref m_ArmourClass);
            m_ArmourClass += armour.ArmourClassModifier;
        }

        // Unequip weapon
        public void UnequipWeapon(Weapon weapon)
        {
            EquippedItemsCount -= 1;
            m_EquippedItemList.Remove(weapon);

            // Default unarmed attack damage
            AttackDamage = m_UnarmedAttackDamage;
        }

        // Unequip armour
        public void UnequipArmour(Armour armour)
        {
            EquippedItemsCount -= 1;
            m_EquippedItemList.Remove(armour);

            // Have to reset the AC. 1 piece of armour at a time here.
            AdjustAbilityModifier(m_Dexterity, ref m_ArmourClass);
        }
    }

    // Player class adds current room and player functionality to Character class
    [Serializable()]
    public class Player : Character
    {
        public Player(String name, List<Item> startItems) : base(name, startItems)
        {
            
        }

        // Currently occupied Room
        private Room m_CurrentRoom;
        public ref Room GetCurrentRoomRef { get { return ref m_CurrentRoom; } }
        public Room CurrentRoom { set { m_CurrentRoom = value; } }

        // Pickpocket another player function
        public String PickPocketPlayer(ref Player targetPlayer, Random rand)
        {
            // Check target player has something to steal
            if (targetPlayer.Inventory.Count - targetPlayer.EquippedItemsCount == 0)
            {
                return targetPlayer.Name + " has nothing unequipped in their inventory.";
            }
            // Not D&D method but quite good substitute. D&D seems to rely more upon perception and situational bonuses given by DM
            // Rolls a dice with minimum 1, and maximum m_Dexterityy.
            int playerDexRoll = rand.Next(1, Dexterity);
            int targetSavingRoll = rand.Next(1, targetPlayer.Dexterity);

            // If Player roll is highest then success
            if (playerDexRoll > targetSavingRoll)
            {
                Item stolenItem = null;

                // Choose a random item in the targets inventory that is not equipped. Found a use for a do while loop!
                do { stolenItem = targetPlayer.Inventory[rand.Next(0, targetPlayer.Inventory.Count - 1)]; }
                while (targetPlayer.EquippedItems.Contains(stolenItem));

                // Update each player's inventory
                Inventory.Add(stolenItem);
                targetPlayer.Inventory.Remove(stolenItem);
                return "Success! You pickpocket " + targetPlayer.Name + " and steal a " + stolenItem.Name + ".\r\n\r\nRoll: " + playerDexRoll + "\r\nSave: "+ targetSavingRoll;
            }
            // Server will pick up on this message when sending the message back to the player client, and inform the targeted player
            return "@<PickPocket> attempt failed. " + targetPlayer.Name + " slowly turns round and looks at you. They know what you have just tried to do.\r\n\r\nRoll: " + playerDexRoll + "\r\nSave: " + targetSavingRoll + "@<PickPocket> " + Name + " has just tried to pickpocket you! They failed.";
        }

        // Pickpocket an NPC function. Slightly different to pickPocketPlayer function in return message and static instance altering. i.e. if you steal an item from an NPC, then it will still be available to steal by other players
        public String PickpocketNPC(Character targetNPC, Random rand)
        {
            // Check target NPC has something to steal
            if (targetNPC.Inventory.Count - targetNPC.EquippedItemsCount == 0)
            {
                return targetNPC.Name + " has nothing unequipped in their inventory.";
            }
            // Not D&D method but quite good substitute. D&D seems to rely more upon perception and situational bonuses given by DM
            // Rolls a dice with minimum 1, and maximum m_Dexterityy.
            int playerDexRoll = rand.Next(1, Dexterity);
            int targetSavingRoll = rand.Next(1, targetNPC.Dexterity);

            // If Player roll is highest then success
            if (playerDexRoll > targetSavingRoll)
            {
                Item stolenItem = null;

                // Choose a random item in the targets inventory that is not equipped. Found a use for a do while loop!
                do { stolenItem = targetNPC.Inventory[rand.Next(0, targetNPC.Inventory.Count - 1)]; }
                while (targetNPC.EquippedItems.Contains(stolenItem));

                // Update player's inventory
                Inventory.Add(stolenItem);
                return "Success! You pickpocket " + targetNPC.Name + " and steal a " + stolenItem.Name + ".\r\n\r\nRoll: " + playerDexRoll + "\r\nSave: " + targetSavingRoll;
            }
            return "<PickPocket> attempt failed. " + targetNPC.Name + " slowly turns round and looks at you. They know what you have just tried to do.\r\n\r\nRoll: " + playerDexRoll + "\r\nSave: " + targetSavingRoll;
        }

        // This is quite fun
        public String FightPlayer(ref Player attackedPlayer, Random rand)
        {
            // Roll attack throw against attacked player's armour class
            int playerAttackRoll = rand.Next(1, 20);
            int thisAttackDamageRoll;

            if (playerAttackRoll == 1)
            {
                // Critical Miss
                return "<Battle> Roll: 1 - Critical miss!\r\n\r\n" + Name + " fights like a dairy farmer.";
            }
            else if (playerAttackRoll == 20)
            {
                // Critical Hit. Two damage rolls!
                thisAttackDamageRoll = rand.Next(1, AttackDamage) + rand.Next(1, AttackDamage);
                if (attackedPlayer.ApplyDamage(thisAttackDamageRoll + AttackModifier) <= 0)
                {
                    attackedPlayer.HitPoints = 0;
                    return "You killed " + attackedPlayer.Name + "!\r\n<Battle> Roll: 20 - Critical hit!\r\n\r\n" + Name + " deals " + (thisAttackDamageRoll + AttackModifier).ToString() + " (Roll: " + thisAttackDamageRoll + " + attack modifier: " + AttackModifier + ") damage to " + attackedPlayer.Name + "!";
                }
                else
                {
                    return "<Battle> Roll: 20 - Critical hit!\r\n\r\n" + Name + " deals " + (thisAttackDamageRoll + AttackModifier).ToString() + " (Roll: " + thisAttackDamageRoll + " + attack modifier: " + AttackModifier + ") damage to " + attackedPlayer.Name + "!";
                }
            }

            // Add the ability modifier
            int modifiedAttackRoll = playerAttackRoll + AttackModifier;

            if (modifiedAttackRoll >= attackedPlayer.ArmourClass)
            {
                // Hit!
                thisAttackDamageRoll = rand.Next(1, AttackDamage);
                if (attackedPlayer.ApplyDamage(thisAttackDamageRoll + AttackModifier) <= 0)
                {
                    attackedPlayer.HitPoints = 0;
                    return "You killed " + attackedPlayer.Name +"!\r\n<Battle> " + Name + " Roll: " + playerAttackRoll + " + strength modifier " + AttackModifier + " = " + modifiedAttackRoll + "\r\nVs " + attackedPlayer.Name + " armour class " + attackedPlayer.ArmourClass + "\r\n\r\nHit! \r\n\r\n" + Name + " deals " + (thisAttackDamageRoll + AttackModifier).ToString() + " (Roll: " + thisAttackDamageRoll + " + attack modifier: " + AttackModifier + ") damage to " + attackedPlayer.Name + "!" + "\r\n\r\n" + Name + " : " + HitPoints + "\r\n" + attackedPlayer.Name + " : " + attackedPlayer.HitPoints; ;
                }
                else
                {
                    return "<Battle> " + Name + " Roll: " + playerAttackRoll + " + strength modifier " + AttackModifier + " = " + modifiedAttackRoll + "\r\nVs " + attackedPlayer.Name + " armour class " + attackedPlayer.ArmourClass + "\r\n\r\nHit! \r\n\r\n" + Name + " deals " + (thisAttackDamageRoll + AttackModifier).ToString() + " (Roll: " + thisAttackDamageRoll + " + attack modifier: " + AttackModifier + ") damage to " + attackedPlayer.Name + "!" + "\r\n\r\n" + Name + " : " + HitPoints + "\r\n" + attackedPlayer.Name + " : " + attackedPlayer.HitPoints;
                }
            }
            else
            {
                // Miss
                return "<Battle> " + Name + " Roll: " + playerAttackRoll + " + strength modifier " + AttackModifier + " = " + modifiedAttackRoll + "\r\nVs " + attackedPlayer.Name + " armour class " + attackedPlayer.ArmourClass + "\r\n\r\nMiss." + "\r\n\r\n" + Name + " : " + HitPoints + "\r\n" + attackedPlayer.Name + " : " + attackedPlayer.HitPoints;
            }
        }
    }

    // NPC base class, in case we want npc enemies later as well as friendlies
    public class NPC : Character
    {
        public NPC(String name, List<Item> startItems, String description, String speech) : base(name, startItems)
        {
            m_Description = description;
            m_Speech = speech;
        }

        // Look at response
        private String m_Description;
        public String Description { get { return m_Description; } }

        // Talk to response
        private String m_Speech;
        public String Speech { get { return m_Speech; } }
    }

    // Friendly
    public class Friendly : NPC
    {
        public Friendly(String name, List<Item> startItems, String description, String speech) : base(name, startItems, description, speech) { }
    }
}
