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
        Socket client;
        private Thread myThread;
        bool bQuit = false;
        bool bConnected = false;
        
        // D&D style character sheet
        static int 
            m_Strength, 
            m_Dexterity,
            m_Constitution,
            m_Intelligence,
            m_Wisdom,
            m_Charisma;

        static bool playerReady = false;
        static bool rollAgain = false;
        static bool CharacterSheetSentToServer = false;

        List<String> currentClientList = new List<String>();

        // D&D method (best 3 values of 4d6)
        static int RollAbility(Random rand)
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

        // Rolls a new character ability sheet and displays it
        static public void RollNewCharacterSheet(Form1 form, Random rand,
            ref int strength,
            ref int dexterity,
            ref int constitution,
            ref int intelligence,
            ref int wisdom,
            ref int charisma)
        {
            strength = RollAbility(rand);
            dexterity = RollAbility(rand);
            constitution = RollAbility(rand);
            intelligence = RollAbility(rand);
            wisdom = RollAbility(rand);
            charisma = RollAbility(rand);

            form.AddGameText(" ");
            form.AddMessageText(
                "Strength: " + strength.ToString() +
                "\r\nDexterity: " + dexterity.ToString() +
                "\r\nConstitution: " + constitution.ToString() +
                "\r\nIntelligence: " + intelligence.ToString() +
                "\r\nWisdom: " + wisdom.ToString() +
                "\r\nCharisma: " + charisma.ToString()
                );
            
            form.AddMessageText("\r\n\r\nReroll character? Y/N \r\n\r\n");
        }
        
        static public void clientProcess(Object o)
        {            
            Form1 form = (Form1)o;

            // Random stream from which to roll dice for character sheet
            Random rand = new Random();

            // Initial roll
            RollNewCharacterSheet(form, rand,
                ref m_Strength,
                ref m_Dexterity,
                ref m_Constitution,
                ref m_Intelligence,
                ref m_Wisdom,
                ref m_Charisma);

            // Pre joining the server loop
            while (!playerReady)
            {
                if (rollAgain)
                {
                    // Reroll character sheet when prompted
                    rollAgain = false;
                    RollNewCharacterSheet(form, rand,
                        ref m_Strength,
                        ref m_Dexterity,
                        ref m_Constitution,
                        ref m_Intelligence,
                        ref m_Wisdom,
                        ref m_Charisma);
                }
            }
            
            while ((form.bConnected == false) && (form.bQuit == false))
            {
                try
                {
                    form.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    // Only need to connect to local machine for this task
                    form.client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8500));
                    form.bConnected = true;
                    form.AddMessageText("Connected to server");

                    Thread receiveThread;

                    receiveThread = new Thread(clientReceive);
                    receiveThread.Start(o);

                    while ((form.bQuit == false) && (form.bConnected == true))
                    {
                        if (form.IsDisposed == true)
                        {
                            form.bQuit = true;
                            form.client.Close();
                        }
                    }                    

                    receiveThread.Abort();
                }
                catch (System.Exception)
                {
                    form.AddMessageText("No server!");
                    Thread.Sleep(1000);
                }               
            }
        }

        static void clientReceive(Object o)
        {
            Form1 form = (Form1)o;

            while (form.bConnected == true)
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    int result;

                    result = form.client.Receive(buffer);

                    if (result > 0)
                    {
                        MemoryStream stream = new MemoryStream(buffer);
                        BinaryReader read = new BinaryReader(stream);

                        Msg m = Msg.DecodeStream(read);

                        if (m != null)
                        {
                            Console.Write("Got a message: ");
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
                                        form.bQuit = true;
                                    }
                                    break;

                                default:
                                    break;
                            }
                        }
                    }     
                    
                }
                catch (Exception)
                {
                    form.bConnected = false;
                    Console.WriteLine("Lost server!");
                }

            }
        }
        public Form1()
        {
            InitializeComponent();

            myThread = new Thread(clientProcess);
            myThread.Start(this);

            Application.ApplicationExit += delegate { OnExit(); };
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
                currentClientList.Clear();
                currentClientList.Add("All");

                foreach (String s in clientList.clientList)
                {
                    currentClientList.Add(s);
                }
                currentClientList.Add("Game");
                listBox_ClientList.DataSource = currentClientList;             
            }
        }


        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (textBox_Input.Text.Length > 0)
            {
                if (!playerReady)
                {
                    if (textBox_Input.Text.ToLower() == "y" || textBox_Input.Text.ToLower() == "yes")
                    {
                        // Uncomment the below line to remove the y/yes on each reroll. Leaving it there makes multiple rerolls easier
                        //textBox_Input.Text = "";
                        rollAgain = true;
                    }

                    else if (textBox_Input.Text.ToLower() == "n" || textBox_Input.Text.ToLower() == "no")
                    {
                        textBox_Input.Text = "";
                        playerReady = true;
                    }
                }
                else if (client != null)
                {
                    try
                    {
                        // Send character sheet on first connected exchange (button click)
                        if (!CharacterSheetSentToServer)
                        {
                            // Send accepted player character sheet to the server for player initialisation
                            PlayerInitMsg characterSheetString = new PlayerInitMsg();
                            characterSheetString.msg =
                                m_Strength.ToString() + " " +
                                m_Dexterity.ToString().ToString() + " " +
                                m_Constitution.ToString() + " " +
                                m_Intelligence.ToString() + " " +
                                m_Wisdom.ToString() + " " +
                                m_Charisma.ToString();
                            MemoryStream outStream = characterSheetString.WriteData();
                            client.Send(outStream.GetBuffer());

                            CharacterSheetSentToServer = true;
                        }
                    
                        if (listBox_ClientList.SelectedIndex == 0)
                        {
                            PublicChatMsg publicMsg = new PublicChatMsg();

                            publicMsg.msg = textBox_Input.Text;
                            MemoryStream outStream = publicMsg.WriteData();
                            client.Send(outStream.GetBuffer());
                        }
                        // If == last item ("Game" Msg)
                        else if (listBox_ClientList.SelectedIndex == listBox_ClientList.Items.Count - 1)
                        {
                            GameMsg GameMessage = new GameMsg();
                            GameMessage.msg = textBox_Input.Text;
                            GameMessage.destination = currentClientList[listBox_ClientList.SelectedIndex];
                            MemoryStream outStream = GameMessage.WriteData();
                            client.Send(outStream.GetBuffer());
                        }
                        else
                        {
                            PrivateChatMsg privateMsg = new PrivateChatMsg();

                            privateMsg.msg = textBox_Input.Text;
                            privateMsg.destination = currentClientList[listBox_ClientList.SelectedIndex];
                            MemoryStream outStream = privateMsg.WriteData();
                            client.Send(outStream.GetBuffer());
                        }

                    }
                    catch (System.Exception)
                    {
                    }

                    textBox_Input.Text = "";
                }
            }
        }

        private void OnExit()
        {
            bQuit = true;
            Thread.Sleep(500);
            if (myThread != null)
            {
                myThread.Abort();
            }
        }

        private void listBox_ClientList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox_Output_TextChanged(object sender, EventArgs e)
        {

        }

        // Send a String message to the game on the server
        private void SendGameMessage(String message)
        {
            GameMsg GameMessage = new GameMsg();
            GameMessage.msg = message;

            // Send to game clientList index
            GameMessage.destination = currentClientList[listBox_ClientList.Items.Count - 1];
            MemoryStream outStream = GameMessage.WriteData();
            client.Send(outStream.GetBuffer());
        }

        // Winform button shortcuts
        private void LookAround_Click(object sender, EventArgs e)
        {
            if (playerReady)
                SendGameMessage("look around");
        }

        private void Inventory_Click(object sender, EventArgs e)
        {
            if (playerReady)
                SendGameMessage("look at inventory");
        }

        // Key down short cuts spoof pre defined messages to be sent to the server
        private void Form1_KeyDown(object sender, KeyEventArgs keyEvent)
        {
            if (playerReady)
            {
                // Uncomment below for arrow key navigation

                //switch (keyEvent.KeyCode)
                //{
                //    case Keys.Up:
                //        SendGameMessage("go north");
                //        break;
                //    case Keys.Left:
                //        SendGameMessage("go west");
                //        break;
                //    case Keys.Down:
                //        SendGameMessage("go south");
                //        break;
                //    case Keys.Right:
                //        SendGameMessage("go east");
                //        break;
                //}
            }
        }

        // Controller based on buttons on the winform client
        private void North_Click(object sender, EventArgs e)
        {
            if (playerReady)
                SendGameMessage("go north");
        }

        private void South_Click(object sender, EventArgs e)
        {
            if (playerReady)
                SendGameMessage("go south");
        }

        private void East_Click(object sender, EventArgs e)
        {
            if (playerReady)
                SendGameMessage("go east");
        }

        private void West_Click(object sender, EventArgs e)
        {
            if (playerReady)
                SendGameMessage("go west");
        }
    }
}
