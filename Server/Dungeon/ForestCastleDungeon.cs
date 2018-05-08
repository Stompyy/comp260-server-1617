using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server;

namespace Dungeon
{
    // New dungeon!
    public class ForestCastleDungeon
    {
        // Items instantiation
        Item grog = new Item("grog", 0.5f, 4.0f, "Ah sweet grog. I love you grog.");

        Key finalDoorKey = new Key("finaldoorkey", 0.0f, 0.0f, "This key looks like it might open the final door!", null);

        static HealthItem potion = new HealthItem("potion", 1.0f, 20.0f, "This potion will replenish your health.");
        static HealthItem sweetRoll = new HealthItem("sweetroll", 2.0f, 3.0f, "I played but didn't finish fallout 4.");
        static HealthItem egg = new HealthItem("egg", 0.1f, 5.0f, "There is an angry chicken somewhere. The egg looks tasty.");
        static HealthItem fish = new HealthItem("fish", 1.0f, 12.0f, "Trout are very valuable and immensely powerful.");

        Weapon branch = new Weapon("branch", 5.0f, 1.0f, "A large stick that you have found on the ground", 4);
        Weapon rock = new Weapon("rock", 2.0f, 1.0f, "It is a heavy rock that you could bash things quite nicely with.", 4);
        Weapon club = new Weapon("club", 2.0f, 1.0f, "This is a large stick. It looks like it would be well balanced as a weapon.", 5);
        static Weapon pitchfork = new Weapon("pitchfork", 10.0f, 10.0f, "What do use a pitchfork for? Farming? Farming what? This makes no sense.", 6);
        static Weapon sword = new Weapon("sword", 5.0f, 100.0f, "This is a large sword. It will kill easily. Have fun.", 8);
        static Weapon lightsaber = new Weapon("lightsaber", 5.0f, 1000.0f, "This is the weapon of a Jedi Knight. Not as clumsy or random as a blaster. An elegant weapon for a more civilized age.", 50);
        Weapon masterSword = new Weapon("mastersword", 3.0f, 120.0f, "There is an image of the triforce on the hilt.", 8);
        static Weapon busterSword = new Weapon("bustersword", 7.0f, 150.0f, "This sword is much larger than you. It can hold three materia.", 12);

        static Armour shield = new Armour("shield", 5.0f, 20.0f, "It is wildly over powered.", 4);
        Armour fullPlate = new Armour("fullplate", 50.0f, 100.0f, "It is so bulky you don't think you can even hold a shield at the same time.", 6);

        Friendly guard1 = new Friendly("guard", new List<Item> { sword, shield, potion }, "The guard looks relieved to see you.", "<guard> Finally, you're here! We can't beat the guard captain by ourselves. That's why we sent for you. He's in the castle, but he won't be easy to find. Good Luck!");
        Friendly guard2 = new Friendly("guard", new List<Item> { sword, shield, potion }, "The guard is wearing a tshirt with a picture of a horse on.", "<guard> Horses are so great aren't they.");
        Friendly guard3 = new Friendly("guard", new List<Item> { sword, shield, potion }, "The guard suddenly stops dancing.", "<guard> I wasn't dancing.");
        Friendly guard4 = new Friendly("guard", new List<Item> { busterSword, shield, potion }, "The guard is playing the latest hit iOS game on his iPhone.", "<guard> Have you played BattleScreens yet? It's well worth £2.99. Don't refund.");
        Friendly lostGuard = new Friendly("guard", new List<Item> { sword, shield, potion }, "The guard is looking around unsure of his surroundings. He looks lost.", "<guard> Roux beats cfop");
        Friendly peasant = new Friendly("peasant", new List<Item> { pitchfork }, "The peasant has a remarkable moustache. A moustache that could launch a thousand ships. You stare in awe.", "<peasant> Stop staring at my moustache.");
        Friendly chicken = new Friendly("chicken", new List<Item> { egg }, "The chicken looks back at you.", "The chicken says nothing. It looks angry.");
        Friendly jockey = new Friendly("jockey", new List<Item> { }, "He doesn't really look like a jockey, but it's late at night and what other NPC would be at a stable?", "<jockey> I hate horses.");
        Friendly jedi = new Friendly("jedi", new List<Item> { lightsaber }, "For more than a thousand generations, the Jedi Knights were the guardians of peace and justice in the Old Republic. Before the dark times. Before the Empire", "<jedi> Did you ever hear the tragedy of Darth Plagueis the Wise? I thought not. It's not a story the Jedi would tell you. It's a Sith legend. Darth Plagueis was a Dark Lord of the Sith, so powerful and so wise he could use the Force to influence the midichlorians to create life... He had such a knowledge of the dark side that he could even keep the ones he cared about from dying. The dark side of the Force is a pathway to many abilities some consider to be unnatural. He became so powerful... the only thing he was afraid of was losing his power, which eventually, of course, he did. Unfortunately, he taught his apprentice everything he knew, then his apprentice killed him in his sleep. It's ironic he could save others from death, but not himself.");


        public void Init(SQLDatabase database)
        {
            SQLTable rooms = database.getTable("rooms");

            String[] roomTableFieldNames = new string[] {
                "name",
                "id",
                "north",
                "south",
                "east",
                "west",
                "description",
                "isLocked",
                "itemList",
                "npcList",
                "namesOfPlayersInRoom"
            };

            // No need to look at this...
            jedi.Dexterity = 20;

            rooms.AddEntry(new string[]
                {
                    "Mountain road",	// name
					"0", 					// ID
					"End of the road",	// N
					null,				// S
					null,				// E
					null,				// W
					"The road leads down from the mountains into a wooded valley. To the north a castle looms above the treeline to the north. Your hero's instinct drives you to help drive evil from these lands.",
                    "0",					// isLocked
					"branch rock",		// itemList
					"guard1",			// npcList
					""					// players in room
				}, roomTableFieldNames);

            rooms.AddEntry(new string[]
                {
                    "End of the road",	// name
					"1", 					// ID
					null,				// N
					"Mountain road",	// S
					"Forest entrance",	// E
					"Castle stables",	// W
					"You are standing in a clearing. The road from the mountains finishes at a fork. A castle lies to the west, and a dark forest stretches out to the east.",
                    "0",					// isLocked
					"",					// itemList
					"chicken",			// npcList
					""					// players in room
				}, roomTableFieldNames);

            rooms.AddEntry(new string[]
                {
                    "Forest entrance",	// name
					"2", 					// ID
					"Dark forest",		// N
					null,				// S
					null,				// E
					"End of the road",	// W
					"A dark forest spreads out in front of you. Strange noises fill the air. The darkness in the trees reaches out to lure you in, but you wonder if you are strong enough to survive what lies within. The castle is far to the west, and you think you see a path through the trees to the north.",
                    "0",					// isLocked
					"club",				// itemList
					"peasant",			// npcList
					""					// players in room
				}, roomTableFieldNames);

            rooms.AddEntry(new string[]
                {
                    "Dark forest",		// name
					"3", 					// ID
					"Lagoon",			// N
					"Forest entrance",	// S
					null,				// E
					"Castle entrance",	// W
					"The trees here are packed so closely together that the light can barely break through to light the way in front of you. The forest thins towards the west and you know the castle lies somewhere to the south. As you fight the feeling of being lost, you think you hear water running to the north.",
                    "0",					// isLocked
					"",					// itemList
					"",					// npcList
					""					// players in room
				}, roomTableFieldNames);

            rooms.AddEntry(new string[]
                {
                    "Lagoon",			// name
					"4", 					// ID
					"Cave",				// N
					"Dark forest",		// S
					null,				// E
					null,				// W
					"The sound of water reveals a lagoon in a clearing in the trees. The water is crystal clear. The dark forest stretches out to the south, and a cave entrance can be seen to the north",
                    "0",					// isLocked
					"fish",				// itemList
					"lostGuard",		// npcList
					""					// players in room
				}, roomTableFieldNames);

            rooms.AddEntry(new string[]
                {
                    "Cave",				// name
					"5", 					// ID
					null,				// N
					"Lagoon",			// S
					null,				// E
					"<You use the key!\r\n\r\nCastle prison",		// W
					"You sense anger, fear, aggression... There is a hole falling straight down into the cave floor to the west.",
                    "0",					// isLocked
					"finalDoorKey",		// itemList
					"jedi",				// npcList
					""					// players in room
				}, roomTableFieldNames);

            rooms.AddEntry(new string[]
                {
                    "Castle stables",	// name
					"6", 					// ID
					"Castle courtyard",	// N
					null,				// S
					"End of the road",	// E
					null,				// W
					"This looks like the side entrance to the castle. It smells of horses. North goes further into the castle, and east goes back out to the forest.",
                    "0",					// isLocked
					"shield",			// itemList
					"jockey guard2",	// npcList
					""					// players in room
				}, roomTableFieldNames);

            rooms.AddEntry(new string[]
                {
                    "Castle courtyard",	// name
					"7", 					// ID
					"Castle stairs",	// N
					"Castle stables",	// S
					"Castle entrance",	// E
					null,				// W
					"The courtyard is a bit like the main bit of Gondor from the last Lord of the Rings. I'm tired. The stables are to the south, and stairs lead down to the north.",
                    "0",					// isLocked
					"masterSword sweetRoll",			// itemList
					"guard3",			// npcList
					""					// players in room
				}, roomTableFieldNames);

            rooms.AddEntry(new string[]
                {
                    "Castle stairs",	// name
					"8", 					// ID
					"<You use the key!\r\n\r\nCastle prison",	// N
					"Castle courtyard",	// S
					null,				// E
					null,				// W
					"You remember about your quest to find the evil guard captain. You feel like you are getting close. Go south to go back, or north towards the final room!",
                    "0",					// isLocked
					"",					// itemList
					"",					// npcList
					""					// players in room
				}, roomTableFieldNames);

            rooms.AddEntry(new string[]
                {
                    "<You use the key!\r\n\r\nCastle prison",	// name
					"9", 					// ID
					"<Win> Castle guard room",	// N
					"Castle stairs",	// S
					null,				// E
					null,				// W
					"You see the best armour in the game! Noone would blame you if you wanted to go back and kill all the other players. There is a hole in the ceiling to the east, but you can't reach it. The rest of the castle is up the stairs to the south.",
                    "1",					// isLocked
					"fullPlate",		// itemList
					"",					// npcList
					""					// players in room
				}, roomTableFieldNames);

            // Sort this out Richard you prick
            // Set key to unlock this room
            finalDoorKey.RoomThisKeyUnlocks = 9;

            rooms.AddEntry(new string[]
                {
                    "Castle entrance",	// name
					"10", 				// ID
					null,				// N
					null,				// S
					"Dark forest",		// E
					"Castle courtyard",	// W
					"The Castle entrance towers above you. The courtyard lies to the west, and the forest is to the east.",
                    "0",					// isLocked
					"",					// itemList
					"guard4",			// npcList
					""					// players in room
				}, roomTableFieldNames);

            rooms.AddEntry(new string[]
                {
                    "<Win> Castle guard room",	// name
					"11", 				// ID
					null,				// N
					"<You use the key!\r\n\r\nCastle prison",	// S
					null,				// E
					null,				// W
					"The evil guard captain turns out to be you. You forgot you were him, then went out in disguise - that's why noone recognised you. Then you hit your head and forgot everything. M Night Shyamalan. You win!",
                    "0",					// isLocked
					"",					// itemList
					"",			// npcList
					""					// players in room
				}, roomTableFieldNames);
        }

        // Initialise the start room for all Player instances
        // The room in which all Player instances will start the game in
        private int m_StartRoom = 0;
        public int StartRoom { get { return m_StartRoom; } set { m_StartRoom = value; } }

        // Returns the description member of the Room instance, and a list of the exits
        public string DescribeRoom(Room currentRoom)
        {
            String message = currentRoom.name + "\r\n\r\n" + currentRoom.description;
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
        public String UpdateRoom(ref Room currentRoom, ref Player player, String direction)
        {
            // Checks if target room is locked
            if (roomMap[direction].IsLocked)
            {
                // Inventory check for key
                if (player.Inventory.Count > 0)
                {
                    foreach (Item key in player.Inventory)
                    {
                        // Checks key valid for target room
                        if (((Key)key).RoomThisKeyUnlocks == roomMap[direction])
                        {
                            lock (roomMap)
                            {
                                currentRoom.RemovePlayer(player.Name);
                                currentRoom = roomMap[direction];
                                currentRoom.AddPlayer(player.Name);
                                return "You unlock the door with the key.\r\n\r\n";
                            }
                        }
                    }
                }
                return "You do not have the key.\r\n\r\n";
            }
            lock (roomMap)
            {

                currentRoom.RemovePlayer(player.Name);
                currentRoom = roomMap[direction];
                currentRoom.AddPlayer(player.Name);
                return "";
            }
        }
    }
}