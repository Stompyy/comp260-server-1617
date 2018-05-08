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
    public partial class LoginForm : Form
    {
        // A reference to the main form for all message sending functions
        Form1 m_MainForm;

        // A reference to the create new user form 
        RegisterNewUser m_RegisterNewUserForm;

        // Only allow buttons to be activated when connected
        bool m_IsConnected = false;

        /*
         * Constructor
         */
        public LoginForm(Form1 mainform, RegisterNewUser registerNewUserForm)
        {
            // Initialise references
            m_MainForm = mainform;
            m_RegisterNewUserForm = registerNewUserForm;
            InitializeComponent();
        }

        /*
         * Opens the create a new user form
         */
        private void Register_Click(object sender, EventArgs e)
        {
            m_RegisterNewUserForm.ClearTextBoxes();
            m_RegisterNewUserForm.ShowDialog();
        }

        /*
         * Clears all current text boxes and displays an error message
         */
        public void ClearTextBoxes(String errorMessage)
        {
            UserName.Clear();
            Password.Clear();
            loginSuccessMessage.Text = errorMessage;
        }

        /*
         * Tells the main form to send a login details message.
         * 
         * This starts the authentication process:
         * 
         * 1. Send the userName to the server with a request for the salt
         * 2. The server looks up the userName in the player database and sends back the user specific salt stored against that existing userName
         * 3. Upon receiving the salt, the client encrypts the string currently entered in the password_textbox (never storing the string plain text anywhere) and sends
         *      it back to the server with the userName
         * 4. Upon recieving both the userName and the encrypted(password + salt), the server compares this against the encrypted(password + salt) it has stored for that username
         * 5. If identical, then the password is correct, the server returns "Success" and the main form will start the game
         */
        private void LoginButton_Click(object sender, EventArgs e)
        {
            m_MainForm.sendLoginDetails(UserName.Text + " RequestSalt");
        }

        /*
         * Shows connected in the login window and enables the buttons
         */
        public void showConnectedMessage()
        {
            ConnectedMessage.Text = "Connected";
            RegisterButton.Enabled = true;
            m_IsConnected = true;

            // Only enable the LoginButton if there is text entered in the boxes
            if (m_IsConnected && UserName.Text.Length > 0 && Password.Text.Length > 0)
            {
                LoginButton.Enabled = true;
            }
        }

        /*
         * Shows a not connected message in the form and disables the buttons
         */
        public void showDisconnectedMessage()
        {
            ConnectedMessage.Text = "Not connected";
            RegisterButton.Enabled = false;
            LoginButton.Enabled = false;
            m_IsConnected = false;
        }

        /*
         * Tells the main form to send a Login message with the userName and encrypted(password + salt) seperated by a space
         */
        public void logInWithSaltedHash(Byte[] salt)
        {
            m_MainForm.sendLoginDetails(UserName.Text + " " + Encryption.encryptPasswordWithSalt(Password.Text, salt));
        }

        /*
         * When text is changed in the userName box check whether to show enable the loginButton
         */
        private void UserName_TextChanged(object sender, EventArgs e)
        {
            if (m_IsConnected && UserName.Text.Length > 0 && Password.Text.Length > 0)
            {
                LoginButton.Enabled = true;
            }
        }

        /*
         * When text is changed in the password box check whether to show enable the loginButton
         */
        private void Password_TextChanged(object sender, EventArgs e)
        {
            if (m_IsConnected && UserName.Text.Length > 0 && Password.Text.Length > 0)
            {
                LoginButton.Enabled = true;
            }
        }
    }
}
