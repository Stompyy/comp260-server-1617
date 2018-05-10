using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;

namespace Server
{
    /*
     * Controls class parses control messages sent from the client and alters the state of the dungeon and the player
     */
    public class Controls
    {
        // Common function variables used by nearly every control
        static String m_CurrentRoomName;
        static String m_PlayerName;

        // Database tables
        SQLTable t_Logins;
        SQLTable t_Dungeon;
        SQLTable t_Players;
        SQLTable t_Items;
        SQLTable t_Npcs;

        // Table field name strings
        String f_Description = "description";
        String f_Owner = "owner";
        String f_Room = "room";
        String f_HealAmount = "healAmount";
        String f_UseMessage = "useMessage";
        String f_IsEquipped = "isEquipped";
        String f_Damage = "damage";
        String f_Speech = "speech";
        String f_IsLoggedIn = "isLoggedIn";
        String f_HitPoints = "hitPoints";
        String f_MaxHitPoints = "maxHitPoints";
        String f_CurrentRoom = "currentRoom";
        String f_AttackDamage = "attackDamage";
        String f_AttackModifier = "attackModifier";
        String f_ArmourClass = "armourClass";

        // Table value strings
        String v_True = "true";
        String v_False = "false";
        String v_Null = "null";

        // To add a new line in a winforms text window
        String newLine = "\r\n";

        // Single Random number stream to use as dice rolls for certain gameplay elements
        Random rand = new Random();

        /*
         * Constructor sets all SQLTable references
         */
        public Controls(SQLTable argDungeon, SQLTable argPlayers, SQLTable argItems, SQLTable argNpcs, SQLTable argLogins)
        {
            t_Dungeon = argDungeon;
            t_Players = argPlayers;
            t_Items = argItems;
            t_Npcs = argNpcs;
            t_Logins = argLogins;
        }

        /*
         * Update function parses the input message, alters the state of the tables in the database based upon the iterperated instructions, and returns a string outgoing message for the program.cs to parse outbound to the clients
         */
        public string Update(String thisPlayerName, String targetedPlayerName, String inputMessage)
        {
            m_PlayerName = thisPlayerName;
            m_CurrentRoomName = t_Players.getStringFieldFromName(m_PlayerName, f_CurrentRoom);

            // Parse inputMessage
            String[] input = inputMessage.Split(' ');

            // Initialise return outputMessage
            String outputMessage = "";

            switch (input[0].ToLower())
            {
                case "help":
                    outputMessage = "\r\nCommands are ....\r\n";
                    outputMessage += "help - for this screen\r\n";
                    outputMessage += "stats - to view your character sheet\r\n";
                    outputMessage += "look around - to look around\r\n";
                    outputMessage += "look at inventory - to look at inventory\r\n";
                    outputMessage += "look at ... - to return a description of ...\r\n";
                    outputMessage += "pick up ... - to add an item in the room to your inventory\r\n";
                    outputMessage += "drop ... - to drop an unequipped item from your inventory into the room\r\n";
                    outputMessage += "use ... - to use an item in your inventory. If it can be used\r\n";
                    outputMessage += "equip ... - to equip a weapon or armour item\r\n";
                    outputMessage += "unequip ... - to unequip a weapon or armour item\r\n";
                    outputMessage += "give ... to ... - to give an item in your inventory to another player in the room\r\n";
                    outputMessage += "say ... - to message all players in your current room\r\n";
                    outputMessage += "pickpocket ... - to attempt to pickpocket a player in your current room based on a dexterity check\r\n";
                    outputMessage += "talk to ... - to talk to an NPC in the room\r\n";
                    outputMessage += "go [north | south | east | west]  - to travel between locations\r\n";
                    outputMessage += "attack ... - to attack another player";

                    break;

                case "look":
                    try
                    {
                        if (input[1].ToLower() == "around")
                        {
                            // "Look Around" will return the room description
                            return describeRoom(m_CurrentRoomName, m_PlayerName, t_Logins);
                        }
                        // "Look At ..." 
                        else if (input[1].ToLower() == "at")
                        {
                            String targetObjectName = input[2].ToLower();

                            // List items in player's inventory
                            List<String> inventory = t_Items.getNamesFromField(f_Owner, m_PlayerName);
                            if (targetObjectName == "inventory")
                            {
                                // If there are items in the inventory
                                if (inventory.Count > 0)
                                {
                                    outputMessage = "\r\nInventory:";
                                    foreach (String itemName in inventory)
                                    {
                                        outputMessage += newLine + itemName;
                                    }
                                    return outputMessage;
                                }
                                else
                                {
                                    return "\r\nYou are not carrying anything.";
                                }
                            }

                            // Will try and return a description of the item in the players inventory with ... name
                            if (inventory.Contains(targetObjectName))
                            {
                                return newLine + t_Items.getStringFieldFromName(targetObjectName, f_Description);
                            }

                            // Will try and return a description of the item in the current room
                            List<String> roomItems = t_Items.getNamesFromField(f_Room, m_CurrentRoomName);
                            if (roomItems.Contains(targetObjectName))
                            {
                                return newLine + t_Items.getStringFieldFromName(targetObjectName, f_Description);
                            }

                            // Will try and return a description of an NPC in the current room
                            List<String> roomNPCs = t_Npcs.getNamesFromField(f_Room, m_CurrentRoomName);
                            if (roomNPCs.Contains(targetObjectName))
                            {
                                return newLine + t_Npcs.getStringFieldFromName(targetObjectName, f_Description);
                            }
                            
                            return "You can not see a " + targetObjectName + ".";
                        }
                        else
                        {
                            return "\r\nYou don't really want to look there do you";
                        }
                    }
                    catch (Exception)
                    {
                        //handle error
                        return "\r\nWhere would you like to look?";
                    }

                case "say":
                    // <Room> prefix informs the program.cs to send out a room wide message
                    outputMessage = "<Room> <" + m_PlayerName + "> ";

                    for (var i = 1; i < input.Length; i++)
                    {
                        outputMessage += input[i] + " ";
                    }
                    break;

                case "stats":
                    {
                        // Returns the character sheet and current player information for this player
                        outputMessage += m_PlayerName + ": character sheet:" + newLine + newLine;
                        outputMessage += "Strength: "       + t_Players.getIntFieldFromName(m_PlayerName, "strength") + newLine;
                        outputMessage += "Dexterity: "      + t_Players.getIntFieldFromName(m_PlayerName, "dexterity") + newLine;
                        outputMessage += "Constitution: "   + t_Players.getIntFieldFromName(m_PlayerName, "constitution") + newLine;
                        outputMessage += "Intelligence: "   + t_Players.getIntFieldFromName(m_PlayerName, "intelligence") + newLine;
                        outputMessage += "Wisdom: "         + t_Players.getIntFieldFromName(m_PlayerName, "wisdom") + newLine;
                        outputMessage += "Charisma: "       + t_Players.getIntFieldFromName(m_PlayerName, "charisma") + newLine + newLine;
                        outputMessage += "Hitpoints: "      + t_Players.getIntFieldFromName(m_PlayerName, "hitpoints") + newLine;
                        outputMessage += "Armour class: "   + t_Players.getIntFieldFromName(m_PlayerName, "armourClass") + newLine;
                        outputMessage += "AttackModifier: " + t_Players.getIntFieldFromName(m_PlayerName, "attackModifier") + newLine;
                        outputMessage += "Attack damage: 1d" + t_Players.getIntFieldFromName(m_PlayerName, "attackDamage");
                        return outputMessage;
                    }

                case "talk":
                    // Checks for " talk to (NPCName)" command
                    List<String> roomNPCsToTalkTo = t_Npcs.getNamesFromField(f_Room, m_CurrentRoomName);

                    if (roomNPCsToTalkTo.Count == 0)
                    {
                        // The room has no NPCs in. Also checked in the TalkToNPC() below
                        return "There are no NPCs in this room to talk to.";
                    }
                    try
                    {
                        if (input[1].ToLower() == "to")
                        {
                            if (t_Npcs.getStringFieldFromName(input[2].ToLower(), f_Room) == m_CurrentRoomName)
                            {
                                // return the speech component of that NPC
                                return t_Npcs.getStringFieldFromName(input[2].ToLower(), f_Speech);
                            }
                            else
                            {
                                return "Try to 'talk to ...' NPCs instead.";
                            }
                        }
                        else
                        {
                            return "Try to 'talk to ...' NPCs instead.";
                        }
                    }
                    catch (Exception)
                    {
                        return "Try to 'talk to ...' NPCs instead.";
                    }

                case "eat":
                case "use":
                    // To heal through eating food or using healing items
                    try
                    {
                        // Get all items that have this player as their owner
                        List<String> inventory = t_Items.getNamesFromField(f_Owner, m_PlayerName);

                        String itemName = input[1].ToLower();

                        // if targeted item is in this players inventory
                        if (inventory.Contains(itemName))
                        {
                            // Get heal amount field from item table for this item
                            int thisHealAmount = t_Items.getIntFieldFromName(itemName, f_HealAmount);

                            // Only items that are capable of healing will have a non zero value here
                            if (thisHealAmount > 0)
                            {
                                int potentialHitPoints = t_Players.getIntFieldFromName(m_PlayerName, f_HitPoints) + thisHealAmount;

                                // Do not heal more than the max hit points allowed for this player
                                if (potentialHitPoints > t_Players.getIntFieldFromName(m_PlayerName, f_MaxHitPoints))
                                {
                                    t_Players.setFieldFromName(m_PlayerName, t_Players.getIntFieldFromName(m_PlayerName, f_MaxHitPoints), f_HitPoints);
                                }
                                else
                                {
                                    t_Players.setFieldFromName(m_PlayerName, potentialHitPoints, f_HitPoints);
                                }
                            }
                            // Remove player as the owner of that item
                            t_Items.setFieldFromName(itemName, v_Null, f_Owner);

                            // Return the use message for this item
                            return t_Items.getStringFieldFromName(itemName, f_UseMessage);
                        }
                        return "You do not have a " + itemName + " in your inventory.";
                    }
                    catch (Exception)
                    {
                        return "What would you like to " + input[0].ToLower() + "?";
                    }

                case "drop":
                    // Drop an item into the currentRoom. Ses the owner field in the database table to null, and the room field to the currentRoom
                    try
                    {
                        String potentialItemName = input[1].ToLower();

                        // Check that the requested item is owned by the player
                        List<String> inventory = t_Items.getNamesFromField(f_Owner, m_PlayerName);
                        if (inventory.Contains(potentialItemName))
                        {
                            // Check that the requested item is not currently equipped
                            if (t_Items.getStringFieldFromName(potentialItemName, f_IsEquipped) == v_True)
                            {
                                return "You will have to unequip the " + potentialItemName + " first.";
                            }
                            else
                            {
                                // Remove the owner
                                t_Items.setFieldFromName(potentialItemName, v_Null, f_Owner);

                                // Set room field to current room
                                t_Items.setFieldFromName(potentialItemName, m_CurrentRoomName, f_Room);

                                return "You drop the " + potentialItemName + " on the floor.";
                            }
                        }
                        else
                        {
                            return "You need to have a " + potentialItemName + " if you want to drop it.";
                        }
                    }
                    catch (Exception)
                    {
                        return "What would you like to drop?";
                    }

                case "pick":
                    // Picks up an available item. Sets the owner to to the current player in the item database table, and the room field to null
                    try
                    {
                        if (input[1].ToLower() == "up")
                        {
                            String potentialItemName = input[2].ToLower();

                            // 
                            if (t_Items.getStringFieldFromName(potentialItemName, f_Room) == m_CurrentRoomName)
                            {
                                t_Items.setFieldFromName(potentialItemName, v_Null, f_Room);
                                t_Items.setFieldFromName(potentialItemName, m_PlayerName, f_Owner);

                                return "\r\nYou pick up the " + potentialItemName + " and add it to your inventory.";
                            }
                            else
                            {
                                return "\r\nThere is no " + potentialItemName + " in the room.";
                            }
                        }
                        else
                        {
                            return "\r\nThat does not need picking right now thank you";
                        }
                    }
                    catch (Exception)
                    {
                        outputMessage = "\r\nWhat would you like to pick?";
                    }
                    break;

                case "equip":
                    // Equip an unequipped item in the players inventory and adjust the players stats
                    try
                    {
                        String potentialItemName = input[1].ToLower();
                        
                        // Check requested item is owned by the current player
                        List<String> inventory = t_Items.getNamesFromField(f_Owner, m_PlayerName);
                        if (inventory.Contains(potentialItemName)) 
                        {
                            // Check that the item is not already equipped
                            if (t_Items.getStringFieldFromName(potentialItemName, f_IsEquipped) == v_False)
                            {
                                // Set the item as equipped = true in the database table
                                t_Items.setFieldFromName(potentialItemName, v_True, f_IsEquipped);

                                // If the item is a weapon then it will have a non zero damage field
                                int damage = t_Items.getIntFieldFromName(potentialItemName, f_Damage);
                                if (damage > 0)
                                {
                                    // Adjust the player's damage rating and return an equipping message
                                    t_Players.setFieldFromName(m_PlayerName, damage, f_AttackDamage);
                                    return "You equip the " + potentialItemName + ".\r\n\r\nIt has a damage rating of 1d" + damage + ".";
                                }
                                // Else if the item is armour then it will have a non zero armour class field
                                int armourClass = t_Items.getIntFieldFromName(potentialItemName, f_ArmourClass);
                                if (armourClass > 0)
                                {
                                    // Adjust the player's armour class rating and return an equipping message
                                    t_Players.setFieldFromName(m_PlayerName, armourClass, f_ArmourClass);
                                    return "You equip the " + potentialItemName + ".\r\n\r\nYour armour class is now " + armourClass + ".";
                                }
                                // If the item has zero value damage and armoour class fields then it is not meant to be equipped
                                return "You try, but " + potentialItemName + " is not really meant for that.";
                            }
                            else
                            {
                                return potentialItemName + " is already equipped.";
                            }
                        }
                        return "You do not have a " + potentialItemName + " in your inventory.";
                    }
                    catch (Exception)
                    {
                        return "What would you like to equip?";
                    }

                case "unequip":
                    // The opposite of equip Requires looking up the static Character sheet default values for base unequipped stats 
                    try
                    {
                        // if potential item is both equipped by this player and owned by this player
                        String potentialItemName = input[1].ToLower();
                        if (t_Items.getStringFieldFromName(potentialItemName, f_Owner) == m_PlayerName && t_Items.getStringFieldFromName(potentialItemName, f_IsEquipped) == v_True)
                        {
                            // Alter the database table entry in the isEquipped field
                            t_Items.setFieldFromName(potentialItemName, v_False, f_IsEquipped);

                            // As with equipping, check the damage rating to see if it was a weapon
                            int damage = t_Items.getIntFieldFromName(potentialItemName, f_Damage);
                            if (damage > 0)
                            {
                                // Set the players damage rating to default unarmed and return the information
                                t_Players.setFieldFromName(m_PlayerName, Character.UnarmedAttackDamage, f_AttackDamage);
                                return "You unequip the " + potentialItemName + ".\r\n\r\nUnarmed attack damage is 1d" + Character.UnarmedAttackDamage + ".";
                            }
                            // Else check the armour class to see if it was an armour
                            int armourClass = t_Items.getIntFieldFromName(potentialItemName, f_ArmourClass);
                            if (armourClass > 0)
                            {
                                // Set the players armour class rating to default unarmed and return the information
                                t_Players.setFieldFromName(m_PlayerName, Character.baseArmourClass, f_ArmourClass);
                                return "You unequip the " + potentialItemName + ".\r\n\r\nYour armour class is now " + Character.baseArmourClass + ".";
                            }
                            else
                            {
                                return "This should not happen. Equipped items will always have either a damage component or an armour class.";
                            }
                        }
                        else
                        {
                            return "You do not have a " + potentialItemName + " equipped.";
                        }
                    }
                    catch (Exception)
                    {
                        return "What would you like to unequip?";
                    }

                case "give":
                    // Change the ownership field of an item from the current player name to a targeted player name
                    try
                    {
                        // If message is in the form "give (Item) to (targetedPlayer)"
                        if (input.Length >= 4 && input[2] == "to")
                        {
                            // Check if targeted player is valid
                            if (targetedPlayerName != null)
                            {
                                // Check that targeted player is in the player's currentRoom and is Logged in - Sanity check
                                if (t_Players.getStringFieldFromName(targetedPlayerName, f_CurrentRoom) == m_CurrentRoomName && t_Logins.getStringFieldFromName(targetedPlayerName, f_IsLoggedIn) == v_True)
                                {
                                    // Check player is not trying to give things to themselves
                                    if (targetedPlayerName != m_PlayerName)
                                    {
                                        String potentialItemName = input[1].ToLower();

                                        // Check that targeted item is owned by this player
                                        if (t_Items.getStringFieldFromName(potentialItemName, f_Owner) == m_PlayerName)
                                        {
                                            // Check that the item is not currently equipped
                                            if (t_Items.getStringFieldFromName(potentialItemName, f_IsEquipped) == v_True)
                                            {
                                                return "You will have to unequip that item first.";
                                            }
                                            else
                                            {
                                                // Set the owner field of this item in the database table to the targeted player
                                                t_Items.setFieldFromName(potentialItemName, targetedPlayerName, f_Owner);

                                                // @<Gift> prefix to return message will tell the program.cs to inform both involved players of the gift
                                                return "@<Gift> You give your " + potentialItemName + " to " + targetedPlayerName + ".@<Gift> " + m_PlayerName + " has given you a " + potentialItemName + ".";
                                            }
                                        }
                                        else
                                        {
                                            return "You do not have a " + potentialItemName + " in your inventory.";
                                        }
                                    }
                                    else
                                    {
                                        return "You thank yourself, then realise you are stupid.";
                                    }
                                }
                                else
                                {
                                    return "There is noone with that name in this room with you.";
                                }
                            }
                            else
                            {
                                return "Who do you want to give something to?";
                            }
                        }
                        else
                        {
                            return "That's not how to politely give things.";
                        }
                    }
                    catch (Exception)
                    {
                        return "What would you like to give, and to who?";
                    }

                case "pickpocket":
                    try
                    {
                        // Check there is someone else in the room to try and pickpocket
                        List<String> playersInThisRoom = t_Players.getNamesFromField(f_Room, m_CurrentRoomName);
                        List<String> npcsInThisRoom = t_Npcs.getNamesFromField(f_Room, m_CurrentRoomName);

                        String pickpocketTarget = input[1];

                        // PlayersInThisRoom will always be at least one because the current player is counted
                        if (playersInThisRoom.Count == 1 && npcsInThisRoom.Count == 0)
                        {
                            return "There is noone here to try and pickpocket.";
                        }

                        // If there is at least another word after that
                        if (input.Length > 1)
                        {
                            // Check for a valid targetedPlayer in the room and logged in
                            if (playersInThisRoom.Contains(targetedPlayerName) && t_Logins.getStringFieldFromName(targetedPlayerName, f_IsLoggedIn) == v_True)
                            {
                                // Check player is not trying to pickpocket themselves.
                                if (targetedPlayerName != m_PlayerName)
                                {
                                    // Pickpocket function handles the success/fail and returns the appropriate String message
                                    return pickPocket(targetedPlayerName);
                                }
                                else
                                {
                                    return "You question your life choices that have led you to try doing that.";
                                }
                            }
                            else if (npcsInThisRoom.Contains(pickpocketTarget))
                            {
                                // Pickpocket function handles the success/fail and returns the appropriate String message
                                return pickPocketNPC(pickpocketTarget);
                            }
                            else
                            {
                                return "There is noone with that name in this room with you.";
                            }
                        }
                        else
                        {
                            return "Who would you like to try and pickpocket?";
                        }
                    }
                    catch
                    {
                        return "Who would you like to try and pickpocket?";
                    }

                case "attack":
                    // Check there is someone else in the room to fight
                    List<String> playersInRoom = t_Players.getNamesFromField(f_Room, m_CurrentRoomName);

                    // PlayersInThisRoom will always be at least one because the current player is counted
                    if (playersInRoom.Count == 1)
                    {
                        return "There are no other players here to fight.";
                    }
                    // If there is at least another word after that
                    if (input.Length > 1)
                    {
                        // Check for a valid targetedPlayer
                        if (targetedPlayerName != null && t_Logins.getStringFieldFromName(targetedPlayerName, f_IsLoggedIn) == v_True)
                        {
                            // Check player is not trying to fight themselves.
                            if (targetedPlayerName != m_PlayerName)
                            {
                                // Fight function handles the success/fail and returns the appropriate String message
                                return fightPlayer(targetedPlayerName);
                            }
                            else
                            {
                                // Idiot check. D&D style attack rolls
                                int selfAttackRoll = rand.Next(1, 20);
                                int selfDamage;

                                // Natural 1 or critical hit check!
                                if (selfAttackRoll == 1 || selfAttackRoll == 20)
                                {
                                    // Get max damage for damage roll
                                    int maxDamage = t_Players.getIntFieldFromName(m_PlayerName, f_AttackDamage);

                                    // Random roll for damage
                                    selfDamage = rand.Next(1, maxDamage) + rand.Next(1, maxDamage) + t_Players.getIntFieldFromName(m_PlayerName, f_AttackModifier);

                                    // If kill shot
                                    if (t_Players.getIntFieldFromName(m_PlayerName, f_HitPoints) < selfDamage)
                                    {
                                        // Self attacks will not kill, at most will drop health to 1
                                        t_Players.setFieldFromName(m_PlayerName, 1, f_HitPoints);
                                        return "You tried to kill yourself but that would leave less fun for the other better players...\r\n\r\nRoll: " + selfAttackRoll + "\r\n\r\n" +
                                            "You did not stand a chance. Ruthlessly, you hit yourself for " + selfDamage + " damage.\r\n\r\nCritical hit. Congratulations.\r\n\r\n" +
                                            "Hit points : " + t_Players.getIntFieldFromName(m_PlayerName, f_HitPoints);

                                    }
                                    else
                                    {
                                        // Attack feedback
                                        return "Roll: " + selfAttackRoll + "\r\n\r\nYou did not stand a chance. Ruthlessly, you hit yourself for " + selfDamage + " damage.\r\n\r\n" +
                                            "Critical hit. Congratulations.\r\n\r\nHit points : " + t_Players.getIntFieldFromName(m_PlayerName, f_HitPoints);
                                    }
                                }
                                else
                                {
                                    // roll for damage
                                    selfDamage = rand.Next(1, t_Players.getIntFieldFromName(m_PlayerName, f_AttackDamage)) + t_Players.getIntFieldFromName(m_PlayerName, f_AttackModifier);

                                    // If kill shot
                                    if (t_Players.getIntFieldFromName(m_PlayerName, f_HitPoints) < selfDamage)
                                    {
                                        // Self attacks will not kill, at most will drop health to 1
                                        t_Players.setFieldFromName(m_PlayerName, 1, f_HitPoints);
                                        return "You tried to kill yourself but that would leave less fun for the other better players...\r\n\r\nRoll: " + selfAttackRoll + "\r\n\r\n" +
                                            "You instinctively sense the attack coming and try to block it.\r\n\r\nYou hit yourself for " + selfDamage + " damage.\r\n\r\n" +
                                            "You are an idiot.\r\n\r\nWell done.\r\n\r\nHit points : " + t_Players.getIntFieldFromName(m_PlayerName, f_HitPoints);

                                    }
                                    else
                                    {
                                        // Attack feedback
                                        return "Roll: " + selfAttackRoll + "\r\n\r\nYou instinctively sense the attack coming and try to block it.\r\n\r\nYou hit yourself for " + selfDamage + " damage.\r\n\r\n" +
                                            "You are an idiot.\r\n\r\nWell done.\r\n\r\nHit points : " + t_Players.getIntFieldFromName(m_PlayerName, f_HitPoints);
                                    }
                                }
                            }
                        }
                        else
                        {
                            return "There is noone with that name in this room with you.";
                        }
                    }
                    else
                    {
                        return "Who would you like to try and fight champ?";
                    }

                case "inventory":
                    {
                        // Get all items in the item table that have this player name as their owner
                        List<String> inventory = t_Items.getNamesFromField(f_Owner, m_PlayerName);

                        // List items in player's inventory
                        if (inventory.Count > 0)
                        {
                            outputMessage = "\r\nInventory:";
                            foreach (String itemName in inventory)
                            {
                                outputMessage += newLine + itemName;
                            }
                            return outputMessage;
                        }
                        else
                        {
                            return "\r\nYou are not carrying anything.";
                        }
                    }

                case "go":
                    // Movement controls
                    try
                    {
                        String direction = input[1].ToLower();

                        // Ensure it is valid direction
                        if (direction == "north" || direction == "south" || direction == "east" || direction == "west")
                        {
                            // Get the next room name from the dungeon table
                            String newRoomName = t_Dungeon.getStringFieldFromName(m_CurrentRoomName, direction);
                            
                            // If not null
                            if (newRoomName != v_Null)
                            {
                                // Checks if target room is locked
                                if (t_Dungeon.getStringFieldFromName(newRoomName, "isLocked") == v_True)
                                {
                                    // Inventory check for key for this room
                                    if (t_Items.getNamesFromTwoFields(f_Owner, m_PlayerName, "roomUnlocks", newRoomName).Count() > 0)
                                    {
                                        // Set player's current room as the new room name
                                        t_Players.setFieldFromName(m_PlayerName, newRoomName, f_CurrentRoom);

                                        // Return a description of the new room
                                        return describeRoom(newRoomName, m_PlayerName, t_Logins);
                                    }
                                    return "You do not have the key.\r\n\r\n";
                                }

                                // Set player's current room as the new room name
                                t_Players.setFieldFromName(m_PlayerName, newRoomName, f_CurrentRoom);
                                
                                // Return a description of the new room
                                return describeRoom(newRoomName, m_PlayerName, t_Logins);
                            }
                            else
                            {
                                // Handle error
                                outputMessage = describeRoom(m_CurrentRoomName, m_PlayerName, t_Logins);
                                outputMessage += "\r\nCan not go " + direction + " from here.";
                                return outputMessage;
                            }
                        }
                        else
                        {
                            return direction + " is not a valid direction.";
                        }
                    }
                    catch
                    {
                        return "Which way would you like to go?";
                    }

                default:
                    //handle error
                    outputMessage = "\r\nERROR";
                    outputMessage += "\r\nCan not " + inputMessage + ". It's probably not a good idea.";
                    break;
            }

            return outputMessage;
        }

        /*
         * Pickpocket another user's player
         */
        private String pickPocket(String targetPlayerName)
        {
            // Check target player has something to steal in their inventory that is not equipped
            List <String> targetAvailableInventory = t_Items.getNamesFromTwoFields(f_Owner, targetPlayerName, f_IsEquipped, v_False);
            int targetAvailableItemCount = targetAvailableInventory.Count();

            if (targetAvailableItemCount > 0)
            {
                // Not D&D method but quite good substitute. D&D relies more upon perception and situational bonuses given by DM
                // Rolls a dice with minimum 1, and maximum the player's dexterityy.
                int playerDexRoll = rand.Next(1, t_Players.getIntFieldFromName(m_PlayerName, "dexterity"));
                int targetSavingRoll = rand.Next(1, t_Players.getIntFieldFromName(targetPlayerName, "dexterity"));

                if (playerDexRoll > targetSavingRoll)
                {
                    // Choose a random item from the target player's available inventory
                    String stolenItemName = targetAvailableInventory[rand.Next(0, targetAvailableItemCount - 1)];

                    // Assign that item's owner field to current player name
                    t_Items.setFieldFromName(stolenItemName, m_PlayerName, f_Owner);

                    // return player message, targeted player is not informed on success
                    return "Success! You pickpocket " + targetPlayerName + " and steal a " + stolenItemName + ".\r\n\r\nRoll: " + playerDexRoll + "\r\nSave: " + targetSavingRoll;
                }
                else
                {
                    // Server will pick up on this message when sending the message back to the player client, and inform the targeted player
                    return "@<PickPocket> attempt failed. " + targetPlayerName + " slowly turns round and looks at you. They know what you have just tried to do.\r\n\r\n" +
                        "Roll: " + playerDexRoll + "\r\nSave: " + targetSavingRoll + "@<PickPocket> " + m_PlayerName + " has just tried to pickpocket you! They failed.";
                }
            }
            else
            {
                return targetPlayerName + " has nothing unequipped in their inventory.";
            }
        }

        /*
         * Pickpocket an NPC function. Slightly different to pickPocket player function in SQL query type and return message
         */
        private String pickPocketNPC(String targetNPCName)
        {
            // Check target player has something to steal
            List<String> targetAvailableInventory = t_Items.getNamesFromField(f_Owner, targetNPCName);
            int targetAvailableItemCount = targetAvailableInventory.Count();

            if (targetAvailableItemCount > 0)
            {
                // Not D&D method but quite good substitute. D&D relies more upon perception and situational bonuses given by DM
                // Rolls a dice with minimum 1, and maximum m_Dexterityy.
                int playerDexRoll = rand.Next(1, t_Players.getIntFieldFromName(m_PlayerName, "dexterity"));
                int targetSavingRoll = rand.Next(1, t_Npcs.getIntFieldFromName(targetNPCName, "dexterity"));

                if (playerDexRoll > targetSavingRoll)
                {
                    // Choose a random item from the target player's available inventory
                    String stolenItemName = targetAvailableInventory[rand.Next(0, targetAvailableItemCount - 1)];

                    // Assign that item's owner field to current player name
                    t_Items.setFieldFromName(stolenItemName, m_PlayerName, f_Owner);

                    return "Success! You pickpocket " + targetNPCName + " and steal a " + stolenItemName + ".\r\n\r\nRoll: " + playerDexRoll + "\r\nSave: " + targetSavingRoll;
                }
                else
                {
                    return "<PickPocket> attempt failed. " + targetNPCName + " slowly turns round and looks at you. They know what you have just tried to do.\r\n\r\n" +
                        "Roll: " + playerDexRoll + "\r\nSave: " + targetSavingRoll;
                }
            }
            else
            {
                return targetNPCName + " has nothing unequipped in their inventory.";
            }
        }

        /*
         * This is quite fun
         */
        private String fightPlayer(String attackedPlayerName)
        {
            // Roll attack throw against attacked player's armour class
            int playerAttackRoll = rand.Next(1, 20);
            int maxAttackDamage = t_Players.getIntFieldFromName(m_PlayerName, f_AttackDamage);
            int attackModifier = t_Players.getIntFieldFromName(m_PlayerName, f_AttackModifier);
            int thisAttackDamageRoll;

            if (playerAttackRoll == 1)
            {
                // Critical Miss
                return "<Battle> Roll: 1 - Critical miss!\r\n\r\n" + m_PlayerName + " fights like a dairy farmer.";
            }
            else if (playerAttackRoll == 20)
            {
                // Critical Hit. Two damage rolls!
                thisAttackDamageRoll = rand.Next(1, maxAttackDamage) + rand.Next(1, maxAttackDamage);
                int enemyHitPoints = t_Players.getIntFieldFromName(attackedPlayerName, f_HitPoints);
                int totalAttackDamage = (thisAttackDamageRoll + attackModifier);
                if (totalAttackDamage < 1) totalAttackDamage = 1;
                if (enemyHitPoints - totalAttackDamage <= 0)
                {
                    // Long winded return message contans all information about the fight
                    t_Players.setFieldFromName(attackedPlayerName, 0, f_HitPoints); ;
                    return "You killed " + attackedPlayerName + "!\r\n<Battle> Roll: 20 - Critical hit!\r\n\r\n" + m_PlayerName + " deals " + (thisAttackDamageRoll + attackModifier).ToString() + " " +
                        "(Roll: " + thisAttackDamageRoll + " + attack modifier: " + attackModifier + ") damage to " + attackedPlayerName + "!";
                }
                else
                {
                    // Long winded return message contans all information about the fight
                    t_Players.setFieldFromName(attackedPlayerName, enemyHitPoints - (thisAttackDamageRoll + attackModifier), f_HitPoints);
                    return "<Battle> Roll: 20 - Critical hit!\r\n\r\n" + m_PlayerName + " deals " + (thisAttackDamageRoll + attackModifier).ToString() + " " +
                        "(Roll: " + thisAttackDamageRoll + " + attack modifier: " + attackModifier + ") damage to " + attackedPlayerName + "!";
                }
            }

            // Add the ability modifier
            int modifiedAttackRoll = playerAttackRoll + attackModifier;

            if (modifiedAttackRoll >= t_Players.getIntFieldFromName(attackedPlayerName, f_ArmourClass))
            {
                // Hit!
                thisAttackDamageRoll = rand.Next(1, maxAttackDamage);
                int enemyHitPoints = t_Players.getIntFieldFromName(attackedPlayerName, f_HitPoints);
                int totalAttackDamage = (thisAttackDamageRoll + attackModifier);
                if (totalAttackDamage < 1) totalAttackDamage = 1;
                if (enemyHitPoints - (thisAttackDamageRoll + attackModifier) <= 0)
                {
                    // Long winded return message contans all information about the fight
                    t_Players.setFieldFromName(attackedPlayerName, 0, f_HitPoints);
                    return "You killed " + attackedPlayerName + "!\r\n<Battle> " + m_PlayerName + " Roll: " + playerAttackRoll + " + strength modifier " + attackModifier + " = " + modifiedAttackRoll + "\r\n" +
                        "Vs " + attackedPlayerName + " armour class " + t_Players.getIntFieldFromName(attackedPlayerName, f_ArmourClass) + "\r\n\r\n" +
                        "Hit! \r\n\r\n" + m_PlayerName + " deals " + (thisAttackDamageRoll + attackModifier).ToString() + " (Roll: " + thisAttackDamageRoll + " + attack modifier: " + attackModifier + ") " +
                        "damage to " + attackedPlayerName + "!" + "\r\n\r\n" + m_PlayerName + " : " + t_Players.getIntFieldFromName(m_PlayerName, f_HitPoints) + "\r\n" + 
                        attackedPlayerName + " : " + t_Players.getIntFieldFromName(attackedPlayerName, f_HitPoints);
                }
                else
                {
                    // Long winded return message contans all information about the fight
                    t_Players.setFieldFromName(attackedPlayerName, enemyHitPoints - (thisAttackDamageRoll + attackModifier), f_HitPoints);
                    return "<Battle> " + m_PlayerName + " Roll: " + playerAttackRoll + " + strength modifier " + attackModifier + " = " + modifiedAttackRoll + "\r\n" +
                        "Vs " + attackedPlayerName + " armour class " + t_Players.getIntFieldFromName(attackedPlayerName, f_ArmourClass) + "\r\n\r\nHit! \r\n\r\n" + m_PlayerName + " deals " + 
                        (thisAttackDamageRoll + attackModifier).ToString() + " (Roll: " + thisAttackDamageRoll + " + attack modifier: " + attackModifier + ") damage to " + attackedPlayerName + "!" + 
                        "\r\n\r\n" + m_PlayerName + " : " + t_Players.getIntFieldFromName(m_PlayerName, f_HitPoints) + "\r\n" + attackedPlayerName + " : " + t_Players.getIntFieldFromName(attackedPlayerName, f_HitPoints);
                }
            }
            else
            {
                // Miss message
                return "<Battle> " + m_PlayerName + " Roll: " + playerAttackRoll + " + strength modifier " + attackModifier + " = " + modifiedAttackRoll + "\r\nVs " + attackedPlayerName + " armour class " + 
                    t_Players.getIntFieldFromName(attackedPlayerName, f_ArmourClass) + "\r\n\r\nMiss." + "\r\n\r\n" + m_PlayerName + " : " + t_Players.getIntFieldFromName(m_PlayerName, f_HitPoints) + "\r\n" + 
                    attackedPlayerName + " : " + t_Players.getIntFieldFromName(attackedPlayerName, f_HitPoints);
            }
        }

        /*
         * Returns the description field of the room in the dungeon table
         * Also lists items in the room, NPCs in the room, and players in the room which
         * is why the queyring player name is passed in so as not to list themselves as another player
         * and the login table to check if a player is logged in before listing them
         */
        public String describeRoom(String roomName, String queryingPlayerName, SQLTable loginTable)
        {
            // Start with the dungeon table's description field of that room entry
            String message = roomName + "\r\n\r\n" + t_Dungeon.getStringFieldFromName(roomName, f_Description);
            message += "\r\n\r\nExits:\r\n";

            // List the exits if that room has a non empty field for that direction in the table
            if (t_Dungeon.getStringFieldFromName(roomName, "north") != v_Null) message += "north, ";
            if (t_Dungeon.getStringFieldFromName(roomName, "south") != v_Null) message += "south, ";
            if (t_Dungeon.getStringFieldFromName(roomName, "east") != v_Null) message += "east, ";
            if (t_Dungeon.getStringFieldFromName(roomName, "west") != v_Null) message += "west ";
            
            // Get all items with this room as their room field
            List<String> itemsInRoom = t_Items.getNamesFromField(f_Room, roomName);
            String currentRoomItemNames = "";

            foreach (String itemName in itemsInRoom)
            {
                // Sanity check
                if (itemName != "") currentRoomItemNames += itemName + ", ";
            }

            // If there are no itemNames then the string will still be "". If no items then do not say anything about items
            if (currentRoomItemNames != "")
            {
                String currentRoomItemNamesMessage = "\r\n\r\nIn the room you see the following items: " + currentRoomItemNames;
                message += currentRoomItemNamesMessage;
            }
            
            // Get all NPCs with this room as their room field
            List<String> npcsInRoom = t_Npcs.getNamesFromField(f_Room, roomName);
            String currentRoomNPCNames = "";

            foreach (String npcName in npcsInRoom)
            {
                // Sanity check
                if (npcName != "") currentRoomNPCNames += npcName + ", ";
            }

            // If there are no npcNames then the string will still be "". If no NPCs then do not say anything about NPCs
            if (currentRoomNPCNames != "")
            {
                String currentRoomNPCNamesMessage = "\r\n\r\nIn the room are the following NPCs: " + currentRoomNPCNames;
                message += currentRoomNPCNamesMessage;
            }

            // Get all players with this room as their room field
            List<String> playersInRoom = t_Players.getNamesFromField(f_CurrentRoom, roomName);
            String currentRoomPlayerNames = "";

            foreach (String otherPlayerName in playersInRoom)
            {
                // No need to say the current player's name
                if (otherPlayerName != queryingPlayerName && otherPlayerName != "")
                {
                    // Don't include players in the player table that are not currently logged in
                    if (loginTable.getStringFieldFromName(otherPlayerName, f_IsLoggedIn) == v_True)
                    {
                        currentRoomPlayerNames += otherPlayerName + ", ";
                    }
                }
            }

            // If there are no otherPlayerNames then the string will still be "". If no other players then do not say anything about other players
            if (currentRoomPlayerNames != "")
            {
                String currentRoomPlayerNamesMessage = "\r\n\r\nIn the room are the following other players: " + currentRoomPlayerNames;
                message += currentRoomPlayerNamesMessage;
            }

            // return the completed room description message
            return message;
        }
    }
}
