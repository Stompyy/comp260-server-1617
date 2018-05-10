using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

using MessageTypes;

namespace Winform_Client
{
    public partial class Form1 : Form
    {
        public Socket m_Server;
        private Thread m_Thread;
        bool m_Quit = false;
        bool m_Connected = false;
        List<String> m_CurrentClientList = new List<String>();
        
        static Form1 m_MainForm;
        static LoginForm m_LoginForm;
        static RegisterNewUser m_RegisterNewUserForm;

        // Constructor
        public Form1()
        {
            m_MainForm = this;
            InitializeComponent();

            // Hide the main form until login is complete and gameplay starts
            Hide();
            m_Thread = new Thread(clientProcess);
            m_Thread.Start(this);

            Application.ApplicationExit += delegate { OnExit(); };
        }

        /*
         * Connect to the server
         */
        static public void clientProcess(Object o)
        {            
            Form1 form = (Form1)o;
            
            while ((form.m_Connected == false) && (form.m_Quit == false))
            {
                try
                {
                    form.m_Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    form.m_Server.Connect(new IPEndPoint(IPAddress.Parse("192.168.1.224"), 8500));//"165.227.227.143"
                    form.m_Connected = true;

                    m_MainForm.Invoke(new MethodInvoker(delegate ()
                    {
                        m_LoginForm.showConnectedMessage();
                    }));

                    Thread receiveThread = new Thread(clientReceive);
                    receiveThread.Start(o);

                    while ((form.m_Quit == false) && (form.m_Connected == true))
                    {
                        if (form.IsDisposed == true)
                        {
                            form.m_Quit = true;
                            form.m_Server.Close();
                        }
                    }                    

                    receiveThread.Abort();
                }
                catch (System.Exception)
                {
                    m_MainForm.Enabled = false;

                    if (m_LoginForm.Visible == false)
                        m_LoginForm.ShowDialog();

                    // Invokes access cross thread functions
                    m_LoginForm.Invoke(new MethodInvoker(delegate ()
                    {
                        m_LoginForm.showDisconnectedMessage();
                    }));
                    m_MainForm.Invoke(new MethodInvoker(delegate ()
                    {
                        m_MainForm.Hide();
                    }));

                    form.AddMessageText("No server!");
                    Thread.Sleep(1000);
                }
            }
            Application.Restart();
        }

        /*
         * Receive messages from the server
         */
        static void clientReceive(Object o)
        {
            Form1 form = (Form1)o;

            while (form.m_Connected == true)
            {
                try
                {
                    byte[] buffer = new byte[4096];

                    if (form.m_Server.Receive(buffer) > 0)
                    {
                        MemoryStream stream = new MemoryStream(buffer);
                        BinaryReader read = new BinaryReader(stream);

                        Msg m = Msg.DecodeStream(read);

                        if (m != null)
                        {
                            switch (m.mID)
                            {
                                case PublicChatMsg.ID:
                                    {
                                        PublicChatMsg publicMsg = (PublicChatMsg)m;
                                        form.AddMessageText(publicMsg.msg);
                                    }
                                    break;

                                case PrivateChatMsg.ID:
                                    {
                                        PrivateChatMsg privateMsg = (PrivateChatMsg)m;
                                        form.AddMessageText(privateMsg.msg);
                                    }
                                    break;

                                case ClientListMsg.ID:
                                    {
                                        ClientListMsg clientList = (ClientListMsg)m;
                                        form.SetClientList(clientList);
                                    }
                                    break;

                                case ClientNameMsg.ID:
                                    {
                                        ClientNameMsg clientName = (ClientNameMsg)m;
                                        form.SetClientName(clientName.name);
                                    }
                                    break;

                                case GameMsg.ID:
                                    {
                                        GameMsg gameMessage = (GameMsg)m;
                                        form.AddGameText(gameMessage.msg);
                                    }
                                    break;

                                case PlayerDeadMsg.ID:
                                    {
                                        // Player is dead. Quit.
                                        PlayerDeadMsg playerDeadMsg = (PlayerDeadMsg)m;
                                        form.AddGameText(playerDeadMsg.msg);
                                        form.m_Quit = true;
                                    }
                                    break;

                                case LoginMsg.ID:
                                    {
                                        LoginMsg recievedLoginMsg = (LoginMsg)m;
                                        switch (recievedLoginMsg.msg)
                                        {
                                            case "LoginAccepted":
                                                m_MainForm.Invoke(new MethodInvoker(delegate ()
                                                {
                                                    // Close the login window and enable and show the game window
                                                    m_LoginForm.Close();
                                                    m_MainForm.Enabled = true;
                                                    m_MainForm.Show();
                                                    m_MainForm.WindowState = FormWindowState.Normal;

                                                    // Trigger the initial message for the game describing the current room 
                                                    m_MainForm.SendGameMessage("look around");
                                                }));

                                                break;

                                            case "LoginFailed":
                                                m_MainForm.Invoke(new MethodInvoker(delegate ()
                                                {
                                                    // Clear text boxes and display error message
                                                    m_LoginForm.ClearTextBoxes("Incorrect login details");
                                                }));
                                                break;

                                            case "UserAlreadyLoggedIn":
                                                m_MainForm.Invoke(new MethodInvoker(delegate ()
                                                {
                                                    // Clear text boxes and display error message
                                                    m_LoginForm.ClearTextBoxes("User logged in already");
                                                }));
                                                break;

                                            default:
                                                // If not one of the above confirm/deny strings then the message is the password salt sent back from the server in order to compare password hashs.
                                                Byte[] salt = Convert.FromBase64String(recievedLoginMsg.msg);

                                                m_MainForm.Invoke(new MethodInvoker(delegate ()
                                                {
                                                    // Handing the salt back to the login form as we don't want to store the plaintext password anywhere, it is only ever read from the password box
                                                    m_LoginForm.logInWithSaltedHash(salt);
                                                }));
                                                break;
                                        }
                                    }
                                    break;

                                case CreateNewUserMsg.ID:
                                    {
                                        // Returned through new user creation process
                                        CreateNewUserMsg recievedNameCheck = (CreateNewUserMsg)m;
                                        switch (recievedNameCheck.msg)
                                        {
                                            case "NameAvailable":
                                                m_MainForm.Invoke(new MethodInvoker(delegate ()
                                                {
                                                    m_RegisterNewUserForm.UserNameAccepted();
                                                }));
                                                break;

                                            case "NameTaken":
                                                m_MainForm.Invoke(new MethodInvoker(delegate ()
                                                {
                                                    m_RegisterNewUserForm.UserNameRejected();
                                                }));
                                                break;

                                            case "Success":
                                                m_MainForm.Invoke(new MethodInvoker(delegate ()
                                                {
                                                    m_RegisterNewUserForm.Close();
                                                }));
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    form.m_Connected = false;
                    Console.WriteLine("Lost server!");
                    try
                    {
                        m_MainForm.Enabled = false;
                        
                        // if the login window is closed
                        if (m_LoginForm.Visible == false)
                            m_LoginForm.ShowDialog();

                        m_MainForm.Invoke(new MethodInvoker(delegate ()
                        {
                            m_LoginForm.showDisconnectedMessage();
                            m_MainForm.Hide();
                        }));
                    }
                    catch { }
                }
            }
            Application.Restart();
        }

        private delegate void AddTextDelegate(String s);

        // Clears the text box before writing the String
        private void AddGameText(String s)
        {
            if (textBox_Output.InvokeRequired)
            {
                Invoke(new AddTextDelegate(AddGameText), new object[] { s });
            }
            else
            {
                // Comment out the following line to give a steady stream of previous messages to scroll back through
                textBox_Output.Text = "----------";

                // Append will scroll the textbox to the end of the new string, then add a new line
                textBox_Output.AppendText("\r\n" + s + "\r\n");
            }
        }

        // Writes the String under the currently displayed text in the text box
        private void AddMessageText(String s)
        {
            if (textBox_Output.InvokeRequired)
            {
                Invoke(new AddTextDelegate(AddMessageText), new object[] { s });
            }
            else
            {
                // Append will scroll the textbox to the end of the new string, then add a new line
                textBox_Output.AppendText("\r\n" + s + "\r\n");
            }
        }

        /*
         * Sets the client name in the upper left of the game window
         */
        private delegate void SetClientNameDelegate(String s);
        private void SetClientName(String s)
        {
            if (this.InvokeRequired)
            {
                Invoke(new SetClientNameDelegate(SetClientName), new object[] {s});
            }
            else
            {
                Text = s;
            }
        }

        /*
         * Sets the chat name list in the chat window
         */
        private delegate void SetClientListDelegate(ClientListMsg clientList);
        private void SetClientList(ClientListMsg clientList)
        {
            if (this.InvokeRequired)
            {
                Invoke(new SetClientListDelegate(SetClientList), new object[] { clientList });
            }
            else
            {
                listBox_ClientList.DataSource = null;
                m_CurrentClientList.Clear();
                // initialises the list with an 'All' field for chat to all other users
                m_CurrentClientList.Add("All");

                // Add each other users name to the list
                foreach (String s in clientList.clientList)
                {
                    m_CurrentClientList.Add(s);
                }
                listBox_ClientList.DataSource = m_CurrentClientList;             
            }
        }

        /*
         * Sends a user creation message to the server
         */
        public void sendNewUserInfo(String newUserInfoString)
        {
            CreateNewUserMsg newUserMsg = new CreateNewUserMsg();
            newUserMsg.msg = newUserInfoString;
            MemoryStream outStream = newUserMsg.WriteData();
            m_Server.Send(outStream.GetBuffer());
        }

        /*
         * Different function name purely for readability. The server will parse the recieved string and act differently upon it dependining on it's composition
         */
        public void checkNameAvailability(String userName)
        {
            sendNewUserInfo(userName);
        }

        /*
         * Sends a login message to the server
         */
        public void sendLoginDetails(String loginDetails)
        {
            LoginMsg loginMsg = new LoginMsg();
            loginMsg.msg = loginDetails;
            MemoryStream outStream = loginMsg.WriteData();
            m_Server.Send(outStream.GetBuffer());

            // Set The form's clientName as the username portion of the outbound message.
            // It does not matter if the login message is unsuccessful as this form will only become visible on a succeesful login
            m_MainForm.Invoke(new MethodInvoker(delegate ()
            {
                m_MainForm.SetClientName(loginDetails.Split(' ')[0]);
            }));
        }

        /*
         * Sends a game message to the server
         */
        private void buttonSend_Click(object sender, EventArgs e)
        {
            // If there is a message to send
            if (textBox_Input.Text.Length > 0)
            {
                // Sanity check that we are connected to the server
                if (m_Server != null)
                {
                    try
                    {
                        GameMsg GameMessage = new GameMsg();
                        GameMessage.msg = SanitiseString(textBox_Input.Text);
                        MemoryStream outStream = GameMessage.WriteData();
                        m_Server.Send(outStream.GetBuffer());
                    }
                    catch (System.Exception)
                    {
                    }
                    // Clear the text box
                    textBox_Input.Text = "";
                }
            }
        }

        /*
         * Sanitise the string. Found at https://stackoverflow.com/questions/11395775/clean-the-string-is-there-any-better-way-of-doing-it?utm_medium=organic&utm_source=google_rich_qa&utm_campaign=google_rich_qa
         * But easy enough I should have just done it myself
         */
        public string SanitiseString(string dirtyString)
        {
            HashSet<char> removeChars = new HashSet<char>("?&^$#@!()+-,:;<>’\'-_*");
            StringBuilder result = new StringBuilder(dirtyString.Length);
            foreach (char c in dirtyString)
                if (!removeChars.Contains(c)) // prevent dirty chars
                    result.Append(c);
            return result.ToString();
        }

        /*
         * Sends a chat message to other clients connected to the server
         */
        private void sendChatButton_Click(object sender, EventArgs e)
        {
            // If there is a message to send
            if (textBox_chatMessage.Text.Length > 0)
            {
                // Sanity check that we are connected to the server
                if (m_Server != null)
                {
                    try
                    {
                        // index = 0 is a chat message to all chat names in the chat name list
                        if (listBox_ClientList.SelectedIndex == 0)
                        {
                            PublicChatMsg publicMsg = new PublicChatMsg();
                            publicMsg.msg = SanitiseString(textBox_chatMessage.Text);
                            MemoryStream outStream = publicMsg.WriteData();
                            m_Server.Send(outStream.GetBuffer());
                        }
                        else
                        {
                            PrivateChatMsg privateMsg = new PrivateChatMsg();
                            privateMsg.msg = SanitiseString(textBox_chatMessage.Text);

                            // destination is the specific other client name that we are messaging
                            privateMsg.destination = m_CurrentClientList[listBox_ClientList.SelectedIndex];
                            MemoryStream outStream = privateMsg.WriteData();
                            m_Server.Send(outStream.GetBuffer());
                        }
                    }
                    catch (System.Exception)
                    {
                    }
                    // Clear the text box
                    textBox_chatMessage.Text = "";
                }
            }
        }

        /*
         * On opening the main form, creates all other forms and references
         */
        private void Form1_Load(object sender, EventArgs e)
        {
            m_RegisterNewUserForm = new RegisterNewUser(this);
            m_LoginForm = new LoginForm(this, m_RegisterNewUserForm);
            m_LoginForm.ShowDialog();
        }

        private void OnExit()
        {
            m_Quit = true;
            Thread.Sleep(500);
            if (m_Thread != null)
            {
                m_Thread.Abort();
            }
        }

        /*
         * Send a String message to the game on the server
         */
        private void SendGameMessage(String message)
        {
            GameMsg GameMessage = new GameMsg();
            GameMessage.msg = message;
            MemoryStream outStream = GameMessage.WriteData();
            m_Server.Send(outStream.GetBuffer());
        }

        /*
         * Winform button shortcuts
         */
        private void LookAround_Click(object sender, EventArgs e)
        {
            SendGameMessage("look around");
        }

        private void Inventory_Click(object sender, EventArgs e)
        {
            SendGameMessage("look at inventory");
        }
        
        private void North_Click(object sender, EventArgs e)
        {
            SendGameMessage("go north");
        }

        private void South_Click(object sender, EventArgs e)
        {
            SendGameMessage("go south");
        }

        private void East_Click(object sender, EventArgs e)
        {
            SendGameMessage("go east");
        }

        private void West_Click(object sender, EventArgs e)
        {
            SendGameMessage("go west");
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            SendGameMessage("help");
        }

        private void showStatsButton_Click(object sender, EventArgs e)
        {
            SendGameMessage("stats");
        }
    }
}
