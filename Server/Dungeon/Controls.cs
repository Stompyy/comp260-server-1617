using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Sockets;

namespace Dungeon
{
    // Controls class parses control messages sent from the client and alters the state of the dungeon and the player
    public class Controls
    {
        static Item GetItemFomListByName(String itemName, List<Item> itemList)
        {
            foreach (Item item in itemList)
            {
                if (item.Name == itemName)
                {
                    return item;
                }
            }
            return null;
        }

        // Simple constructor
        public Controls(ref ForestCastleDungeon dungeon) { m_Dungeon = dungeon; }
        private ForestCastleDungeon m_Dungeon;

        // Single Random number stream to use as dice rolls for certain gameplay elements
        Random rand = new Random();

        // Update function
        public string Update(ref Player player, ref Player targetedPlayer, out Room currentRoom, String inputMessage)
        {

            currentRoom = player.CurrentRoom;
            // Get the soon to be needed information from the player
            //Room currentRoom = player.CurrentRoom;
            String playerName = player.Name;

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
                            outputMessage = m_Dungeon.DescribeRoom(currentRoom);

                            // And the current players in the room
                            if (currentRoom.PlayersInRoom.Count > 1)
                            {
                                outputMessage += "\r\nThe other players in this room are: ";

                                foreach (String playerNameInRoom in currentRoom.PlayersInRoom)
                                {
                                    if (playerNameInRoom != playerName)
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
                            // Will try and return a description of the item in the current room
                            for (int i = 0; i < currentRoom.ItemList.Count; i++)
                            {
                                if (currentRoom.ItemList[i].Name == (input[2].ToLower()))
                                {
                                    outputMessage += "\r\n" + currentRoom.ItemList[i].Description;
                                    return outputMessage;
                                }
                            }
                            // Will try and return a description of an NPC in the current room
                            for (int i = 0; i < currentRoom.NPCList.Count; i++)
                            {
                                if (currentRoom.NPCList[i].Name == (input[2].ToLower()))
                                {
                                    outputMessage += "\r\n" + currentRoom.NPCList[i].Description;
                                    return outputMessage;
                                }
                            }
                            outputMessage += "You can not see a " + input[2].ToLower() + ".";
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
                    outputMessage = "<Room chat> <" + playerName + "> ";

                    for (var i = 1; i < input.Length; i++)
                    {
                        outputMessage += input[i] + " ";
                    }
                    break;

                case "stats":
                    {
                        return player.GetStats();
                    }

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
                    catch (Exception)
                    {
                        return "Try to 'talk to ...' NPCs instead.";
                    }

                case "eat":
                    try
                    {
                        String itemName = input[1].ToLower();
                        foreach (Item item in player.Inventory)
                        {
                            if (item.Name == itemName)
                            {
                                outputMessage = item.Use(ref player);
                                return outputMessage;
                            }
                        }
                        return "You do not have a " + input[1].ToLower() + " in your inventory.";
                    }
                    catch (Exception)
                    {
                        return "What would you like to eat?";
                    }

                case "use":
                    try
                    {
                        String itemName = input[1].ToLower();
                        foreach (Item item in player.Inventory)
                        {
                            if (item.Name == itemName)
                            {
                                outputMessage = item.Use(ref player);
                                return outputMessage;
                            }
                        }
                        return "You do not have a " + input[1].ToLower() + " in your inventory.";
                    }
                    catch (Exception)
                    {
                        return "What would you like to use?";
                    }

                case "drop":
                    try
                    {
                        // Check that the requested item is not currently equipped
                        if (GetItemFomListByName(input[1].ToLower(), player.EquippedItems) != null)
                        {
                            return "You will have to unequip the " + input[1].ToLower() + " first.";
                        }
                        else
                        {
                            // Function also returns if item is available
                            if (player.DropItem(input[1].ToLower(), ref currentRoom))
                            {
                                player.CurrentRoom = currentRoom;
                                return "You drop the " + input[1].ToLower() + " on the floor.";
                            }
                            else
                            {
                                return "You need to have a " + input[1].ToLower() + " if you want to drop it.";
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return "What would you like to drop?";
                    }

                case "pick":
                    try
                    {
                        // multi word parsing
                        if (input[1].ToLower() == "up")
                        {
                            // Function also returns if item is available
                            if (currentRoom.PickUp(ref player, input[2].ToLower()))
                            {
                                player.CurrentRoom = currentRoom;
                                return "\r\nYou pick up the " + input[2].ToLower() + " and add it to your inventory.";
                            }
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

                case "equip":
                    try
                    {
                        String itemName = input[1].ToLower();
                        try
                        {
                            foreach (Item weapon in player.Inventory)
                            {
                                if (weapon.Name == input[1].ToLower())
                                {
                                    player.EquipWeapon((Weapon)weapon);
                                    return "You equip the " + input[1].ToLower() + ".\r\n\r\nIt has a damage rating of 1d" + ((Weapon)weapon).Damage + ".";
                                }
                            }
                        }
                        catch { }
                        try
                        {
                            foreach (Item armour in player.Inventory)
                            {
                                if (armour.Name == input[1].ToLower())
                                {
                                    player.EquipArmour((Armour)armour);
                                    return "You equip the " + input[1].ToLower() + ".\r\n\r\nYour armour class is now " + player.ArmourClass + ".";
                                }
                            }
                        }
                        catch { }
                        try
                        {
                            foreach (Item item in player.Inventory)
                            {
                                if (item.Name == input[1].ToLower())
                                {
                                    return "You try, but " + input[1].ToLower() + " is not really supposed to do that.";
                                }
                            }
                        }
                        catch { }
                        return "You do not have a " + input[1].ToLower() + " in your inventory.";
                    }
                    catch (Exception)
                    {
                        return "What would you like to equip?";
                    }

                case "unequip":
                    if (player.EquippedItemsCount > 0)
                    {
                        String itemName = input[1].ToLower();
                        try
                        {
                            foreach (Item weapon in player.EquippedItems)
                            {
                                if (weapon.Name == input[1].ToLower())
                                {
                                    player.UnequipWeapon((Weapon)weapon);
                                    return "You unequip the " + input[1].ToLower() + ".\r\n\r\nUnarmed attack damage is 1d" + player.AttackDamage + ".";
                                }
                            }
                        }
                        catch { }
                        try
                        {
                            foreach (Item armour in player.EquippedItems)
                            {
                                if (armour.Name == input[1].ToLower())
                                {
                                    player.UnequipArmour((Armour)armour);
                                    return "You Unequip the " + input[1].ToLower() + ".\r\n\r\nYour armour class is now " + player.ArmourClass + ".";
                                }
                            }
                        }
                        catch { }
                        return "You do not have a " + input[1].ToLower() + " equipped.";
                    }
                    else
                    {
                        return "You have no equipped items.";
                    }

                case "give":
                    try
                    {
                        // If message is in the form "give (Item) to (targetedPlayer)"
                        if (input.Length >= 4 && input[2] == "to")
                        {
                            // Check if targeted player is valid
                            if (targetedPlayer != null)//.Name != "unassigned")
                            {
                                // Check player is not trying to give things to themselves
                                if (targetedPlayer != player)
                                {
                                    foreach (Item item in player.EquippedItems)
                                    {
                                        if (item.Name == input[1])
                                        {
                                            return "You will have to unequip that item first.";
                                        }
                                    }
                                    if (player.GiveItem(input[1], ref targetedPlayer))
                                    {

                                        return "@<Gift> You give your " + input[1] + " to " + targetedPlayer.Name + ".@<Gift> " + player.Name + " has given you a " + input[1] + ".";
                                    }
                                    else
                                    {
                                        return "You do not have a " + input[1] + " in your inventory.";
                                    }
                                }
                                else
                                {
                                    return "You thank yourself, then suddenly realise you are stupid.";
                                }
                            }
                            else
                            {
                                return "There is noone with that name in this room with you.";
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
                    // Check there is someone else in the room to try and pickpocket
                    if (currentRoom.PlayersInRoom.Count == 1 && currentRoom.NPCList.Count == 0)
                    {
                        return "There is noone here to try and pickpocket.";
                    }

                    // If there is at least another word after that
                    if (input.Length > 1)
                    {
                        // Check for a valid targetedPlayer
                        if (targetedPlayer != null)
                        {
                            // Check player is not trying to pickpocket themselves.
                            if (targetedPlayer != player)
                            {
                                // Pickpocket function handles the success/fail and returns the appropriate String message
                                return player.PickPocketPlayer(ref targetedPlayer, rand);
                            }
                            else
                            {
                                return "You question your life choices that have led you to try doing that.";
                            }
                        }
                        else
                        {
                            // Check for NPC pickpocketing
                            foreach (Character character in player.CurrentRoom.NPCList)
                            {
                                if (character.Name == input[1])
                                {
                                    return player.PickpocketNPC(character, rand);
                                }
                            }
                            return "There is noone with that name in this room with you.";
                        }
                    }
                    else
                    {
                        return "Who would you like to try and pickpocket?";
                    }

                case "attack":
                    // Check there is someone else in the room to fight
                    if (currentRoom.PlayersInRoom.Count == 1)
                    {
                        return "There are no other players here to fight.";
                    }
                    // If there is at least another word after that
                    if (input.Length > 1)
                    {
                        // Check for a valid targetedPlayer
                        if (targetedPlayer != null)
                        {
                            // Check player is not trying to fight themselves.
                            if (targetedPlayer != player)
                            {
                                // Fight function handles the success/fail and returns the appropriate String message
                                return player.FightPlayer(ref targetedPlayer, rand);
                            }
                            else
                            {
                                // Idiot check
                                int selfAttackRoll = rand.Next(1, 20);
                                int selfDamage;

                                // Natural 1 or critical hit check!
                                if (selfAttackRoll == 1 || selfAttackRoll == 20)
                                {
                                    selfDamage = rand.Next(1, player.AttackDamage) + rand.Next(1, player.AttackDamage) + player.AttackModifier;
                                    outputMessage = "Roll: " + selfAttackRoll + "\r\n\r\nYou did not stand a chance. Ruthlessly, you hit yourself for " + selfDamage + " damage.\r\n\r\nCritical hit. Congratulations.\r\n\r\nHit points : " + player.HitPoints;
                                    return outputMessage;
                                }
                                else
                                {
                                    selfDamage = rand.Next(1, player.AttackDamage) + player.AttackModifier;
                                    player.ApplyDamage(selfDamage);
                                    return "Roll: " + selfAttackRoll + "\r\n\r\nYou instinctively sense the attack coming and try to block it.\r\n\r\nYou hit yourself for " + selfDamage + " damage.\r\n\r\nYou are an idiot.\r\n\r\nWell done.\r\n\r\nHit points : " + player.HitPoints;
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
                        if (player.Inventory.Count > 0)
                        {
                            outputMessage = "\r\nCurrent inventory:";
                            foreach (Item item in player.Inventory)
                            {
                                outputMessage += "\r\n" + item.Name;
                            }
                        }
                        else
                        {
                            return "\r\nYou are not carrying anything.";
                        }
                    }
                    break;

                case "go":
                    // Movement
                    if ((input[1].ToLower() == "north") && (currentRoom.north != null))
                    {
                        outputMessage = m_Dungeon.UpdateRoom(ref currentRoom, ref player, currentRoom.north);
                        outputMessage += m_Dungeon.DescribeRoom(currentRoom);
                    }
                    else
                    {
                        if ((input[1].ToLower() == "south") && (currentRoom.south != null))
                        {
                            outputMessage = m_Dungeon.UpdateRoom(ref currentRoom, ref player, currentRoom.south);
                            outputMessage += m_Dungeon.DescribeRoom(currentRoom);
                        }
                        else
                        {
                            if ((input[1].ToLower() == "east") && (currentRoom.east != null))
                            {
                                outputMessage = m_Dungeon.UpdateRoom(ref currentRoom, ref player, currentRoom.east);
                                outputMessage += m_Dungeon.DescribeRoom(currentRoom);
                            }
                            else
                            {
                                if ((input[1].ToLower() == "west") && (currentRoom.west != null))
                                {
                                    outputMessage = m_Dungeon.UpdateRoom(ref currentRoom, ref player, currentRoom.west);
                                    outputMessage += m_Dungeon.DescribeRoom(currentRoom);
                                }
                                else
                                {
                                    //handle error
                                    outputMessage = m_Dungeon.DescribeRoom(currentRoom);

                                    outputMessage += "\r\n\r\n\r\nERROR";
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
