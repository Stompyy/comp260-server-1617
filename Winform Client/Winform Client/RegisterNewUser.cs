using MessageTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Winform_Client
{
    public partial class RegisterNewUser : Form
    {
        // A reference to the main form for all message sending functions
        Form1 m_MainForm;

        bool validUserNameChosen = false;
        bool validPasswordChosen = false;

        // D&D style character sheet
        int m_Strength,
            m_Dexterity,
            m_Constitution,
            m_Intelligence,
            m_Wisdom,
            m_Charisma;

        Random random = new Random();

        /*
         * Constructor references the main form passed in on creation and rolls a new random character sheet
         */
        public RegisterNewUser(Form1 mainForm)
        {
            m_MainForm = mainForm;

            InitializeComponent();

            RollNewCharacterSheet(random,
                        ref m_Strength,
                        ref m_Dexterity,
                        ref m_Constitution,
                        ref m_Intelligence,
                        ref m_Wisdom,
                        ref m_Charisma);
        }

        /*
         * Calls the function to reroll the random character sheet
         */
        private void ReRollCharacterSheet_Click(object sender, EventArgs e)
        {
            RollNewCharacterSheet(random,
                        ref m_Strength,
                        ref m_Dexterity,
                        ref m_Constitution,
                        ref m_Intelligence,
                        ref m_Wisdom,
                        ref m_Charisma);
        }

        /*
         * Dungeons and dragons method (best 3 values of 4d6)
         */
        private int RollAbility(Random rand)
        {
            // 4 x 1d6 rolls
            int a = rand.Next(1, 7);
            int b = rand.Next(1, 7);
            int c = rand.Next(1, 7);
            int d = rand.Next(1, 7);

            int[] rolls = { a, b, c, d };

            // Remove the lowest value and return the sum
            return (a + b + c + d - rolls.Min());
        }

        /*
         * Rolls a new character ability sheet and displays it
         */
        void RollNewCharacterSheet(Random random,
            ref int strength,
            ref int dexterity,
            ref int constitution,
            ref int intelligence,
            ref int wisdom,
            ref int charisma)
        {
            strength = RollAbility(random);
            dexterity = RollAbility(random);
            constitution = RollAbility(random);
            intelligence = RollAbility(random);
            wisdom = RollAbility(random);
            charisma = RollAbility(random);

            ShowCharacterSheet(
                "Strength: " + strength.ToString() +
                "\r\nDexterity: " + dexterity.ToString() +
                "\r\nConstitution: " + constitution.ToString() +
                "\r\nIntelligence: " + intelligence.ToString() +
                "\r\nWisdom: " + wisdom.ToString() +
                "\r\nCharisma: " + charisma.ToString()
                );
        }

        private void PasswordBox1_TextChanged(object sender, EventArgs e)
        {
            CheckPasswordsMatch();
        }

        private void PasswordBox2_TextChanged(object sender, EventArgs e)
        {
            CheckPasswordsMatch();
        }

        /*
         * If both password text boxes have text in, check to see if they are the same and adjust the visibility of the green tick or red cross to reflect this. If they match and a valid userName has been chosen then enable the register button
         */
        private void CheckPasswordsMatch()
        {
            // If both password boxes contain some text
            if (PasswordBox1.Text.Length > 0 || PasswordBox2.Text.Length > 0)
            {
                // If the passwords match
                if (PasswordBox1.Text == PasswordBox2.Text)
                {
                    PasswordGreenTick.Visible = true;
                    PasswordRedCross.Visible = false;
                    validPasswordChosen = true;
                    if (validUserNameChosen)
                    {
                        RegisterButton.Enabled = true;
                    }
                }
                // Else passwords do not match
                else
                {
                    PasswordGreenTick.Visible = false;
                    PasswordRedCross.Visible = true;
                    validPasswordChosen = false;
                    RegisterButton.Enabled = false;
                }
            }
            else
            // If no text in either password box then do not display any green tick or red cross UI
            {
                PasswordGreenTick.Visible = false;
                PasswordRedCross.Visible = false;
                validPasswordChosen = false;
                RegisterButton.Enabled = false;
            }
        }

        /*
         * Tells the main form to send a message to the server requesting a query on the player database as to whether a userName exists within or not. Sanitisation 
         * occurs here. Not just for protection against SQL injection attacks but also to fit with the server's controls parsing. For instanec, spaces will confuse the
         * controls to the server when parsing individual names from a game message string i.e. "attack Big Rich" will have the server looking for a player named "Big"
         */
        private void CheckNameAvailabilityButton_Click(object sender, EventArgs e)
        {
            if (UserNameChoice.Text.Length > 0)
            {
                String forbiddenChars = "? &^$#@!()+-,:;<>’\'-_*";
                foreach (char c in forbiddenChars)
                {
                    // If any illegal characters are found then reject the userName and return
                    if (UserNameChoice.Text.Contains(c))
                    {
                        UserNameRejected();
                        return;
                    }
                }
                // If no illegal chars found then send the check availability message to the server
                m_MainForm.checkNameAvailability(UserNameChoice.Text);
            }
        }

        /*
         * Shows a green tick next to the requested userName and if a matching password has been entered in both password text boxes, enables the register button
         */
        public void UserNameAccepted()
        {
            UserNameGreenTick.Visible = true;
            UserNameRedCross.Visible = false;
            validUserNameChosen = true;
            if (validPasswordChosen)
            {
                RegisterButton.Enabled = true;
            }
        }

        /*
         * Shows a red cross next to the requested userName
         */
        public void UserNameRejected()
        {
            UserNameGreenTick.Visible = false;
            UserNameRedCross.Visible = true;
            validUserNameChosen = false;
            RegisterButton.Enabled = false;
        }

        /*
         * When changing the text in the UserName choice text box, disables all ui until the checkNameAvailabilility button has been pressed
         */
        private void UserNameChoice_TextChanged(object sender, EventArgs e)
        {
            if (validUserNameChosen)
            {
                validUserNameChosen = false;
                UserNameGreenTick.Visible = false;
                UserNameRedCross.Visible = false;
                RegisterButton.Enabled = false;
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /*
         * Tells the main form to send a Create new user message to the server with all of the character sheet values, the userName, the encrypted(password + salt), and the salt, all seperated with a space
         */
        private void RegisterButton_Click(object sender, EventArgs e)
        {
            if (validUserNameChosen && validPasswordChosen)
            {
                m_MainForm.sendNewUserInfo(m_Strength.ToString() + " " +
                                m_Dexterity.ToString().ToString() + " " +
                                m_Constitution.ToString() + " " +
                                m_Intelligence.ToString() + " " +
                                m_Wisdom.ToString() + " " +
                                m_Charisma.ToString() + " " + 
                                UserNameChoice.Text + " " +
                                Encryption.encryptPasswordWithSalt(PasswordBox1.Text, out String salt) + " " +
                                salt);
            }
        }

        private delegate void AddTextDelegate(String s);

        /*
         * Displays the current character sheet values in the form
         */
        private void ShowCharacterSheet(String characterSheetString)
        {
            if (CharacterSheet.InvokeRequired)
            {
                Invoke(new AddTextDelegate(ShowCharacterSheet), new object[] { characterSheetString });
            }
            else
            {
                CharacterSheet.Clear();
                CharacterSheet.AppendText(characterSheetString);
            }
        }

        /*
         * Clears all text boxes
         */
        public void ClearTextBoxes()
        {
            UserNameChoice.Clear();
            PasswordBox1.Clear();
            PasswordBox2.Clear();
        }
    }
}
