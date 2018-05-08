using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if TARGET_LINUX
using Mono.Data.Sqlite;
using sqliteConnection  = Mono.Data.Sqlite.SqliteConnection;
using sqliteCommand = Mono.Data.Sqlite.SqliteCommand;
using sqliteDataReader = Mono.Data.Sqlite.SqliteDataReader;
#endif

#if TARGET_WINDOWS
using System.Data.SQLite;
using sqliteConnection = System.Data.SQLite.SQLiteConnection;
using sqliteCommand = System.Data.SQLite.SQLiteCommand;
using sqliteDataReader = System.Data.SQLite.SQLiteDataReader;
#endif

namespace Server
{
    /*
     * Database class for the database that holds all the game's tables
     */
    public class SQLDatabase
    {
        String m_DataBaseName;
        sqliteConnection m_Connection;
        List<SQLTable> m_TableList;

        /*
         * Constructor creates a new database
         */
        public SQLDatabase(String dataBaseName)
        {
            m_DataBaseName = dataBaseName;
            m_TableList = new List<SQLTable>();
            CreateNew();
        }

        /*
         * Creates new database
         */
        public void CreateNew()
        {
            try
            {
                // Creates database
                sqliteConnection.CreateFile(m_DataBaseName);

                m_Connection = new sqliteConnection("Data Source=" + m_DataBaseName + ";Version=3;FailIfMissing=True");
                m_Connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Create DB failed: " + ex);
            }
        }

        /*
         * Opens the connection
         */
        public void Open()
        {
            try
            {
                m_Connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Open existing DB failed: " + ex);
            }
        }

        /*
         * Creates, adds, and returns a reference to the login table
         */
        public LoginTable addLoginTable(String tableName, string tableColumns)
        {
            LoginTable newTable = new LoginTable(m_Connection, tableName, tableColumns);
            m_TableList.Add(newTable);
            return newTable;
        }

        /*
         * Creates, adds, and returns a reference to the dungeon table
         */
        public DungeonTable addDungeonTable(String tableName, string tableColumns)
        {
            DungeonTable newTable = new DungeonTable(m_Connection, tableName, tableColumns);
            m_TableList.Add(newTable);
            return newTable;
        }

        /*
         * Creates, adds, and returns a reference to the players table
         */
        public PlayersTable addPlayersTable(String tableName, string tableColumns)
        {
            PlayersTable newTable = new PlayersTable(m_Connection, tableName, tableColumns);
            m_TableList.Add(newTable);
            return newTable;
        }

        /*
         * Creates, adds, and returns a reference to the items table
         */
        public ItemsTable addItemsTable(String tableName, string tableColumns)
        {
            ItemsTable newTable = new ItemsTable(m_Connection, tableName, tableColumns);
            m_TableList.Add(newTable);
            return newTable;
        }

        /*
         * Creates, adds, and returns a reference to the NPC table
         */
        public NPCsTable addNPCTable(String tableName, string tableColumns)
        {
            NPCsTable newTable = new NPCsTable(m_Connection, tableName, tableColumns);
            m_TableList.Add(newTable);
            return newTable;
        }

        /*
         * Creates, adds, and returns a reference to the ID table
         */
        public IdTable addIDTable(String tableName, string tableColumns)
        {
            IdTable newTable = new IdTable(m_Connection, tableName, tableColumns);
            m_TableList.Add(newTable);
            return newTable;
        }
    }
}