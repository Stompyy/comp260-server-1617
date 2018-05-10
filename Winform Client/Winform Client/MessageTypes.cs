using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MessageTypes
{
    enum MessageType
    {
        publicMessage,
        privateMessage,
        clientListMessage,
        clientNameMessage,
        gameMessage,
        playerInitMessage,
        createNewUserMsg
    }

    /*
     * Msg class handles the information and stream between client and server
     */
    public abstract class Msg
    {
        public Msg() { mID = 0; }
        public int mID;

        public abstract MemoryStream WriteData();
        public abstract void ReadData(BinaryReader read);

        public static Msg DecodeStream(BinaryReader read)
        {
            int id;
            Msg m = null;

            id = read.ReadInt32();

            switch (id)
            {
                case PublicChatMsg.ID:
                    m = new PublicChatMsg();
                    break;

                case PrivateChatMsg.ID:
                    m = new PrivateChatMsg();
                    break;

                case ClientListMsg.ID:
                    m = new ClientListMsg();
                    break;

                case ClientNameMsg.ID:
                    m = new ClientNameMsg();
                    break;

                case GameMsg.ID:
                    m = new GameMsg();
                    break;

                case PlayerInitMsg.ID:
                    m = new PlayerInitMsg();
                    break;

                case PlayerDeadMsg.ID:
                    m = new PlayerDeadMsg();
                    break;

                case LoginMsg.ID:
                    m = new LoginMsg();
                    break;

                case CreateNewUserMsg.ID:
                    m = new CreateNewUserMsg();
                    break;



                default:
                    throw (new Exception());
            }

            if (m != null)
            {
                m.mID = id;
                m.ReadData(read);
            }

            return m;
        }
    }

    /*
     * Used to send and receive public chat messages between clients
     */
    public class PublicChatMsg : Msg
    {
        public const int ID = 1;
        public String msg;

        public PublicChatMsg() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);

            write.Write(ID);
            write.Write(msg);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            msg = read.ReadString();
        }
    }

    /*
     * Used to send and receive private messages between clients
     */
    public class PrivateChatMsg : Msg
    {
        public const int ID = 2;
        public String msg;
        public String destination;

        public PrivateChatMsg() { mID = ID; }
        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(msg);
            write.Write(destination);

            write.Close();

            return stream;
        }
        public override void ReadData(BinaryReader read)
        {
            msg = read.ReadString();
            destination = read.ReadString();
        }
    }

    /*
     * Used to recieve an upto date client list from the server
     */
    public class ClientListMsg : Msg
    {
        public const int ID = 3;
        public List<String> clientList;

        public ClientListMsg()
        {
            mID = ID;

            clientList = new List<String>();
        }
        public override MemoryStream WriteData()
        {

            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);

            write.Write(ID);
            write.Write(clientList.Count);
            foreach (String s in clientList)
            {
                write.Write(s);
            }

            write.Close();

            return stream;
        }
        public override void ReadData(BinaryReader read)
        {
            int count = read.ReadInt32();

            clientList.Clear();

            for (int i = 0; i < count; i++)
            {
                clientList.Add(read.ReadString());
            }
        }
    }


    /*
     * Used to recieve a clientName from the server, although this is handled client side on new user creation now
     */
    public class ClientNameMsg : Msg
    {
        public const int ID = 4;

        public String name;

        public ClientNameMsg() { mID = ID; }

        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(name);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            name = read.ReadString();
        }
    }

    /*
     * Used to send all game commands, and recieve all gameplay information
     */
    public class GameMsg : Msg
    {
        public const int ID = 5;
        public String msg;

        public GameMsg()
        {
            mID = ID;
        }
        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(msg);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            msg = read.ReadString();
        }
    }

    /*
     * Used to send a character sheet, although this is handled in the CreateNewUserMessage now
     */
    public class PlayerInitMsg : Msg
    {
        public const int ID = 6;
        public String msg;

        public PlayerInitMsg()
        {
            mID = ID;
        }
        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(msg);

            write.Close();

            return stream;
        }


        public override void ReadData(BinaryReader read)
        {
            msg = read.ReadString();
        }
    }

    /*
     * Used to inform a player when they have died
     */
    public class PlayerDeadMsg : Msg
    {
        public const int ID = 7;
        public String msg;

        public PlayerDeadMsg()
        {
            mID = ID;
        }
        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(msg);

            write.Close();

            return stream;
        }

        public override void ReadData(BinaryReader read)
        {
            msg = read.ReadString();
        }
    }

    /*
     * Used during the create a new user process. For checking userNAme availability, sending character sheets, and sending generated password salts and hashes
     */
    public class CreateNewUserMsg : Msg
    {
        public const int ID = 8;
        public String msg;

        public CreateNewUserMsg() { mID = ID; }
        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(msg);

            write.Close();

            return stream;
        }
        public override void ReadData(BinaryReader read)
        {
            msg = read.ReadString();
        }
    }

    /*
     * Used to authenticate during the login process. Both for sending userName and hashed passwords, salt requests, recieving salts requested, and confirmations
     */
    public class LoginMsg : Msg
    {
        public const int ID = 9;
        public String msg;

        public LoginMsg() { mID = ID; }
        public override MemoryStream WriteData()
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter write = new BinaryWriter(stream);
            write.Write(ID);
            write.Write(msg);

            write.Close();

            return stream;
        }
        public override void ReadData(BinaryReader read)
        {
            msg = read.ReadString();
        }
    }
}