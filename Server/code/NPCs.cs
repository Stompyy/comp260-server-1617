
namespace Server
{
    class NPCs
    {
        // Populate the passed in database table with NPCs and thir information
        public static void Init(SQLTable npcs)
        {
            npcs.AddEntry(new string[] {
                "chicken",                                  // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "End of the road",                      // room
                    "The chicken says nothing. It looks angry.",  // speech
                    "The chicken looks back at you.",       // description
                    "4", "10", "6", "18", "14", "8" });               // Character sheet

            npcs.AddEntry(new string[] {
                "jedi",                                     // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "Cave",                                 // room
                    "<jedi> Did you ever hear the tragedy of Darth Plagueis the Wise? I thought not. It is not a story the Jedi would tell you. It is a Sith legend. Darth Plagueis was a Dark Lord of the Sith, so powerful and so wise he could use the Force to influence the midichlorians to create life... He had such a knowledge of the dark side that he could even keep the ones he cared about from dying. The dark side of the Force is a pathway to many abilities some consider to be unnatural. He became so powerful... the only thing he was afraid of was losing his power, which eventually, of course, he did. Unfortunately, he taught his apprentice everything he knew, then his apprentice killed him in his sleep. It is ironic he could save others from death, but not himself.",  // speech
                    "For more than a thousand generations, the Jedi Knights were the guardians of peace and justice in the Old Republic. Before the dark times. Before the Empire.",  // description
                    "18", "20", "12", "10", "16", "4" });             // Character sheet

            npcs.AddEntry(new string[] {
                "peasant",                                  // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "Forest entrance",                      // room
                    "<peasant> Stop staring at my moustache.",  // speech
                    "The peasant has a remarkable moustache. A moustache that could launch a thousand ships. You stare in awe.",  // description
                    "7", "8", "9", "10", "11", "12" });               // Character sheet

            npcs.AddEntry(new string[] {
                "jockey",                                   // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "Castle stables",                       // room
                    "<jockey> I hate horses.",              // speech
                    "He does not really look like a jockey, but it is late at night and what other NPC would be at a stable?",  // description
                    "10", "10", "10", "10", "10", "10" });  // Character sheet

            npcs.AddEntry(new string[] {
                "guard",                                    // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "Mountain road",                        // room
                    "<guard> Finally, you are here! We cannot beat the guard captain by ourselves. That is why we sent for you. He is in the castle, but he will not be easy to find. Good Luck!",  // speech
                    "The guard looks relieved to see you.", // description
                    "10", "10", "10", "10", "10", "10" });  // Character sheet

            npcs.AddEntry(new string[] {
                "fatguard",                                 // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "Castle entrance",                      // room
                    "<guard> Have you played BattleScreens yet? It is well worth £2.99. Do not refund.",  // speech
                    "The guard is playing the latest hit iOS game on his iPhone.",  // description
                    "10", "10", "10", "10", "10", "10" });  // Character sheet

            npcs.AddEntry(new string[] {
                "tallguard",                                // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "Castle stables",                       // room
                    "<guard> Horses are so great.",         // speech
                    "The guard is wearing a tshirt with a picture of a horse on it.",  // description
                    "10", "10", "10", "10", "10", "10" });  // Character sheet

            npcs.AddEntry(new string[] {
                "shortguard",                               // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "Castle courtyard",                     // room
                    "<guard> I was not dancing.",           // speech
                    "The guard suddenly stops dancing.",    // description
                    "10", "10", "10", "10", "10", "10" });  // Character sheet

            npcs.AddEntry(new string[] {
                "lostguard",                                // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "Lagoon",                               // room
                    "<guard> Roux beats cfop",              // speech
                    "The guard is looking around unsure of his surroundings. He looks lost.",  // description
                    "10", "10", "10", "10", "10", "10" });  // Character sheet
        }
    }
}
