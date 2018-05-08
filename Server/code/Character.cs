using System;

namespace Server
{
    /*
     * Dungeons and dragons inspired character sheet stat adjustments and functions to return start values
     */
    public abstract class Character
    {
        static int m_DefaultStartingHitPoints = 20;
        static int m_DefaultStartingArmourClass = 10;
        static int m_DefaultStartingAttackModifier = 0;
        static public String UnarmedAttackDamage = "3";
        static public int baseArmourClass = m_DefaultStartingArmourClass;

        /*
         * D&D style ability modifier
         */
        public static int AdjustAbilityModifier(int abilityScore, int attributeScore)
        {
            int returnValue = attributeScore;
            switch (abilityScore)
            {
                case 1: returnValue += -5; break;
                case 2: returnValue += -4; break;
                case 3: returnValue += -4; break;
                case 4: returnValue += -3; break;
                case 5: returnValue += -3; break;
                case 6: returnValue += -2; break;
                case 7: returnValue += -2; break;
                case 8: returnValue += -1; break;
                case 9: returnValue += -1; break;
                case 10: returnValue += 0; break;
                case 11: returnValue += 0; break;
                case 12: returnValue += 1; break;
                case 13: returnValue += 1; break;
                case 14: returnValue += 2; break;
                case 15: returnValue += 2; break;
                case 16: returnValue += 3; break;
                case 17: returnValue += 3; break;
                case 18: returnValue += 4; break;
                default: break;
            }
            return returnValue;
        }
        /*
         * The character sheet is sent as a string from the client in the program.cs. These functions take that String portion which is relevant and parses it, modifies the attribute that 
         * that ability affects (i.e. constitution affects hitpoints), then returns the int value to store in the database player table
         */
        public static int getStartingHitPoints(String constitution)
        {
            int.TryParse(constitution, out int i);
            return AdjustAbilityModifier(i, m_DefaultStartingHitPoints);
        }

        public static int getStartingAttackModifier(String strength)
        {
            int.TryParse(strength, out int i);
            return AdjustAbilityModifier(i, m_DefaultStartingAttackModifier);
        }

        public static int getStartingArmourClass(String dexterity)
        {
            int.TryParse(dexterity, out int i);
            return AdjustAbilityModifier(i, m_DefaultStartingArmourClass);
        }
    }
}
