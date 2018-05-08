
namespace Server
{
    // Populate the passed in database table with items and thir information
    public class Items
    {
        public static void Init(SQLTable items)
        {
            items.AddEntry(new string[] {
                "grog",                                     // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "null",                                 // room
                    "null",                                 // owner
                    "Ah sweet grog. I love you grog.",      // description
                    "null",                                 // isEquipped
                    "You probably should not drive now.",   // useMessage
                    "5",                                    // healAmount
                    "0",                                    // damage
                    "0",                                    // armourClass
                    "null" });                              // roomUnlocks
            
            items.AddEntry(new string[] {
                "finaldoorkey",
                    Program.GetNextUniqueID().ToString(),
                    "cave",
                    "null",
                    "This key looks like it might open the final door!",
                    "null",
                    "null",
                    "0",
                    "0",
                    "0",
                    "<You use the key!\r\n\r\nCastle prison" });
            
            items.AddEntry(new string[] {
                "potion",
                    Program.GetNextUniqueID().ToString(),
                    "null",
                    "guard",
                    "This potion will replenish your health.",
                    "false",
                    "Drinking the potion heals your wounds.",
                    "50",
                    "0",
                    "0",
                    "null" });

            items.AddEntry(new string[] {
                "elixir",
                    Program.GetNextUniqueID().ToString(),
                    "null",
                    "fatguard",
                    "This elixir will replenish your health.",
                    "false",
                    "If mana was a thing then you would have a full amount now.",
                    "50",
                    "0",
                    "0",
                    "null" });

            items.AddEntry(new string[] {
                "medicine",
                    Program.GetNextUniqueID().ToString(),
                    "null",
                    "tallguard",
                    "This medicine will replenish your health.",
                    "false",
                    "You drink the calpol, it tastes amazing.",
                    "50",
                    "0",
                    "0",
                    "null" });

            items.AddEntry(new string[] {
                "medpack",
                    Program.GetNextUniqueID().ToString(),
                    "null",
                    "shortguard",
                    "This medpack will replenish your health.",
                    "false",
                    "Have you seen the band of brothers episode : Bastogne. It is like that.",
                    "50",
                    "0",
                    "0",
                    "null" });

            items.AddEntry(new string[] {
                "sweetroll",                                // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "Castle courtyard",                     // room
                    "null",                                 // owner
                    "I did play it but not finished fallout 4.",    // description
                    "false",                                // isEquipped
                    "You feel ready to explore the wasteland.",     // useMessage
                    "20",                                   // healAmount
                    "0",                                    // damage
                    "0",                                    // armourClass
                    "null" });                              // roomUnlocks
            
            items.AddEntry(new string[] {
                "egg",                                      // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "null",                                 // room
                    "chicken",                              // owner
                    "There is an angry chicken somewhere. The egg looks tasty.",    // description
                    "false",                                // isEquipped
                    "The world needs less chickens.",       // useMessage
                    "20",                                   // healAmount
                    "0",                                    // damage
                    "0",                                    // armourClass
                    "null" });                              // roomUnlocks
            
            items.AddEntry(new string[] {
                "fish",                                     // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "Lagoon",                               // room
                    "null",                                 // owner
                    "Trout are very valuable and immensely powerful.",  // description
                    "false",                                // isEquipped
                    "Why should it mean that the fish in the sea are all unable to sing? You eat the fish", // useMessage
                    "20",                                   // healAmount
                    "0",                                    // damage
                    "0",                                    // armourClass
                    "null" });                              // roomUnlocks
            
            items.AddEntry(new string[] {
                "branch",                                   // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "Mountain road",                        // room
                    "null",                                 // owner
                    "A large stick that you have found on the ground.", // description
                    "false",                                // isEquipped
                    "null",                                 // useMessage
                    "0",                                    // healAmount
                    "4",                                    // damage
                    "0",                                    // armourClass
                    "null" });                              // roomUnlocks
            
            items.AddEntry(new string[] {
                "rock",                                     // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "Mountain road",                        // room
                    "null",                                 // owner
                    "It is a heavy rock that you could bash things quite nicely with.", // description
                    "false",                                // isEquipped
                    "null",                                 // useMessage
                    "0",                                    // healAmount
                    "4",                                    // damage
                    "0",                                    // armourClass
                    "null" });                              // roomUnlocks
            
            items.AddEntry(new string[] {
                "club",                                     // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "Forest entrance",                      // room
                    "null",                                 // owner
                    "This is a large stick. It looks like it would be well balanced as a weapon.",  // description
                    "false",                                // isEquipped
                    "null",                                 // useMessage
                    "0",                                    // healAmount
                    "5",                                    // damage
                    "0",                                    // armourClass
                    "null" });                              // roomUnlocks
            
            items.AddEntry(new string[] {
                "pitchfork",                                // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "null",                                 // room
                    "peasant",                              // owner
                    "What do you use a pitchfork for? Farming? This makes no sense.",   // description
                    "false",                                // isEquipped
                    "null",                                 // useMessage
                    "0",                                    // healAmount
                    "0",                                    // damage
                    "0",                                    // armourClass
                    "null" });                              // roomUnlocks
            
            items.AddEntry(new string[] {
                "sword",                                    // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "null",                                 // room
                    "guard",                                // owner
                    "This is a large sword. It will kill easily. Have fun.",    // description
                    "false",                                // isEquipped
                    "null",                                 // useMessage
                    "0",                                    // healAmount
                    "8",                                    // damage
                    "0",                                    // armourClass
                    "null" });                              // roomUnlocks

            items.AddEntry(new string[] {
                "fatsword",                                 // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "null",                                 // room
                    "fatguard",                             // owner
                    "This is a large sword. It will kill easily. Have fun.",    // description
                    "false",                                // isEquipped
                    "null",                                 // useMessage
                    "0",                                    // healAmount
                    "7",                                    // damage
                    "0",                                    // armourClass
                    "null" });                              // roomUnlocks

            items.AddEntry(new string[] {
                "longsword",                                // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "null",                                 // room
                    "tallguard",                            // owner
                    "This is a large sword. It will kill easily. Have fun.",    // description
                    "false",                                // isEquipped
                    "null",                                 // useMessage
                    "0",                                    // healAmount
                    "8",                                    // damage
                    "0",                                    // armourClass
                    "null" });                              // roomUnlocks

            items.AddEntry(new string[] {
                "lightsaber",                               // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "null",                                 // room
                    "jedi",                                 // owner
                    "This is the weapon of a Jedi Knight. Not as clumsy or random as a blaster. An elegant weapon for a more civilized age.",   // description
                    "false",                                // isEquipped
                    "null",                                 // useMessage
                    "0",                                    // healAmount
                    "50",                                   // damage
                    "0",                                    // armourClass
                    "null" });                              // roomUnlocks
            
            items.AddEntry(new string[] {
                "mastersword",                              // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "'Castle courtyard",                    // room
                    "null",                                 // owner
                    "There is an image of the triforce on the hilt.",   // description
                    "false",                                // isEquipped
                    "null",                                 // useMessage
                    "0",                                    // healAmount
                    "8",                                    // damage
                    "0",                                    // armourClass
                    "null" });                              // roomUnlocks
            
            items.AddEntry(new string[] {
                "bustersword",                              // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "null",                                 // room
                    "shortguard",                           // owner
                    "This sword is much larger than you. It can hold three materia.",   // description
                    "false",                                // isEquipped
                    "null",                                 // useMessage
                    "0",                                    // healAmount
                    "12",                                   // damage
                    "0",                                    // armourClass
                    "null" });                              // roomUnlocks
            
            items.AddEntry(new string[] {
                "shield",                                   // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "null",                                 // room
                    "guard",                                // owner
                    "It is wildly over powered.",           // description
                    "false",                                // isEquipped
                    "null",                                 // useMessage
                    "0",                                    // healAmount
                    "0",                                    // damage
                    "4",                                    // armourClass
                    "null" });                              // roomUnlocks

            items.AddEntry(new string[] {
                "fatshield",                                // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "null",                                 // room
                    "fatguard",                             // owner
                    "It is wildly over powered.",           // description
                    "false",                                // isEquipped
                    "null",                                 // useMessage
                    "0",                                    // healAmount
                    "0",                                    // damage
                    "4",                                    // armourClass
                    "null" });                              // roomUnlocks

            items.AddEntry(new string[] {
                "tallshield",                               // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "null",                                 // room
                    "tallguard",                            // owner
                    "It is wildly over powered.",           // description
                    "false",                                // isEquipped
                    "null",                                 // useMessage
                    "0",                                    // healAmount
                    "0",                                    // damage
                    "4",                                    // armourClass
                    "null" });                              // roomUnlocks

            items.AddEntry(new string[] {
                "rustyshield",                              // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "null",                                 // room
                    "lostguard",                            // owner
                    "It is wildly over powered.",           // description
                    "false",                                // isEquipped
                    "null",                                 // useMessage
                    "0",                                    // healAmount
                    "0",                                    // damage
                    "4",                                    // armourClass
                    "null" });                              // roomUnlocks

            items.AddEntry(new string[] {
                "shinyshield",                              // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "null",                                 // room
                    "shortguard",                           // owner
                    "It is wildly over powered.",           // description
                    "false",                                // isEquipped
                    "null",                                 // useMessage
                    "0",                                    // healAmount
                    "0",                                    // damage
                    "4",                                    // armourClass
                    "null" });                              // roomUnlocks

            items.AddEntry(new string[] {
                "fullplate",                                // name
                    Program.GetNextUniqueID().ToString(),   // ID
                    "<You use the key!\r\n\r\nCastle prison",   // room
                    "null",                                 // owner
                    "t is so bulky you do not think you can even hold a shield at the same time.",  // description
                    "'false",                               // isEquipped
                    "null",                                 // useMessage
                    "0",                                    // healAmount
                    "0",                                    // damage
                    "8",                                    // armourClass
                    "null" });                              // roomUnlocks
        }
    }
}