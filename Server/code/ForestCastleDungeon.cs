using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server;

namespace Server
{
    public class ForestCastleDungeon
    {
        // Populate the passed in database table reference with the dungeon rooms information
        public static void Init(SQLTable rooms)
        {
            rooms.AddEntry(new string[] {
                "Mountain road",                            // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "End of the road",                      // North room
                    "null",                                 // South room
                    "null",                                 // East room
                    "null",                                 // West room
                    "The road leads down from the mountains into a wooded valley. To the north a castle looms above the treeline to the north. Your heroes instinct drives you to help drive evil from these lands.",  // Description
                    "false"});                              // isLocked

            rooms.AddEntry(new string[] {
                "End of the road",
                    Program.GetNextUniqueID().ToString(),
                    "null",
                    "Mountain road",
                    "Forest entrance",
                    "Castle stables",
                    "You are standing in a clearing. The road from the mountains finishes at a fork. A castle lies to the west, and a dark forest stretches out to the east.",
                    "false" });

            rooms.AddEntry(new string[] {
                "Forest entrance",
                    Program.GetNextUniqueID().ToString(),
                    "Dark forest",
                    "null",
                    "null",
                    "End of the road",
                    "A dark forest spreads out in front of you. Strange noises fill the air. The darkness in the trees reaches out to lure you in, but you wonder if you are strong enough to survive what lies within. The castle is far to the west, and you think you see a path through the trees to the north.",
                    "false" });

            rooms.AddEntry(new string[] {
                "Dark forest",
                    Program.GetNextUniqueID().ToString(),
                    "Lagoon",
                    "Forest entrance",
                    "null",
                    "Castle entrance",
                    "The trees here are packed so closely together that the light can barely break through to light the way in front of you. The forest thins towards the west and you know the castle lies somewhere to the south. As you fight the feeling of being lost, you think you hear water running to the north.",
                    "false" });

            rooms.AddEntry(new string[] {
                "Lagoon",
                    Program.GetNextUniqueID().ToString(),
                    "Cave",
                    "Dark forest",
                    "null",
                    "null",
                    "The sound of water reveals a lagoon in a clearing in the trees. The water is crystal clear. The dark forest stretches out to the south, and a cave entrance can be seen to the north.",
                    "false" });

            rooms.AddEntry(new string[] {
                "Cave",
                    Program.GetNextUniqueID().ToString(),
                    "null",
                    "Lagoon",
                    "null",
                    "<You use the key!\r\n\r\nCastle prison",
                    "You sense anger, fear, aggression... There is a hole falling straight down into the cave floor to the west.",
                    "false" });

            rooms.AddEntry(new string[] {
                "Castle stables",
                    Program.GetNextUniqueID().ToString(),
                    "Castle courtyard",
                    "null",
                    "End of the road",
                    "null",
                    "This looks like the side entrance to the castle. It smells of horses. North goes further into the castle, and east goes back out to the forest.",
                    "false" });

            rooms.AddEntry(new string[] {
                "Castle courtyard",
                    Program.GetNextUniqueID().ToString(),
                    "Castle stairs",
                    "Castle stables",
                    "Castle entrance",
                    "null",
                    "The courtyard is a bit like the main bit of Gondor from the last Lord of the Rings. I have written too many of these now. The stables are to the south, and stairs lead down to the north.",
                    "false" });

            rooms.AddEntry(new string[] {
                "Castle stairs",
                    Program.GetNextUniqueID().ToString(),
                    "<You use the key!\r\n\r\nCastle prison",
                    "Castle courtyard",
                    "null",
                    "null",
                    "You remember about your quest to find the evil guard captain. You feel like you are getting close. Go south to go back, or north towards the final room!",
                    "false" });

            rooms.AddEntry(new string[] {
                "<You use the key!\r\n\r\nCastle prison",
                    Program.GetNextUniqueID().ToString(),
                    "<Win> Castle guard room",
                    "Castle stairs",
                    "null",
                    "null",
                    "You see the best armour in the game! Noone would blame you if you wanted to go back and kill all the other players. There is a hole in the ceiling to the east, but you cannot reach it. The rest of the castle is up the stairs to the south.",
                    "true" });

            rooms.AddEntry(new string[] {
                "Castle entrance",
                    Program.GetNextUniqueID().ToString(),
                    "null",
                    "null",
                    "Dark forest",
                    "Castle courtyard",
                    "The Castle entrance towers above you. The courtyard lies to the west, and the forest is to the east.",
                    "false" });

            rooms.AddEntry(new string[] {
                "<Win> Castle guard room",
                    Program.GetNextUniqueID().ToString(),
                    "null",
                    "'<You use the key!\r\n\r\nCastle prison",
                    "null",
                    "null",
                    "The evil guard captain turns out to be you. You forgot you were him, then went out in disguise - that is why noone recognised you. Then you hit your head and forgot everything. M Night Shyamalan. You win!",
                    "false" });
        }
    }
}