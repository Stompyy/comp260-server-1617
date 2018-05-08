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
     * Base class for the SQLTable derivisions
     * Contains all querying functions that create sql queries and sends them to the database
     */
    public class SQLTable
    {
        // The connection to the table
        public sqliteConnection m_Connection;

        // The table's self reference within the database
        public String m_TableName;

        // The string representation of the columns as appear in the SQL table constructing command
        String m_TableColumns;

        /*
         * Constructor
         */
        public SQLTable(sqliteConnection connection, String tableName, String tableColumns)
        {
            m_Connection = connection;
            m_TableName = tableName;
            m_TableColumns = tableColumns;

            sqliteCommand command = new sqliteCommand("create table " + m_TableName + " (" + m_TableColumns + ")", m_Connection);

            command.Parameters.Add("@name", System.Data.DbType.String).Value = m_TableName;
            command.ExecuteNonQuery();
        }

        /*
         * Returns the name of the table
         */
        public String getName()
        {
            return m_TableName;
        }

        /*
         * Returns true if the query exists somewhere in the field in this table
         */
        public bool queryExists(String query, String field)
        {
            sqliteCommand command = new sqliteCommand("select * from " + m_TableName + " where @field = @query", m_Connection);

            command.Parameters.Add("@query", System.Data.DbType.String).Value = query;
            command.Parameters.Add("@field", System.Data.DbType.String).Value = field;

            sqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /*
         * Returns true if the query exists in the field in this table for entry name in column Name
         */
        public bool queryOtherFieldFromName(String name, String query, String field)
        {
            sqliteCommand command = new sqliteCommand("select @field from " + m_TableName + " where name = @name", m_Connection);

            command.Parameters.Add("@name", System.Data.DbType.String).Value = name;
            command.Parameters.Add("@field", System.Data.DbType.String).Value = field;

            sqliteDataReader reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                if (reader[field].ToString() == query)
                {
                    return true;
                }
            }
            return false;
        }

        /*
         * Returns the string found at the field column for 'name' name
         */
        public String getStringFieldFromName(String name, String field)
        {
            sqliteCommand command = new sqliteCommand("select @field from " + m_TableName + " where name = @name", m_Connection);

            command.Parameters.Add("@name", System.Data.DbType.String).Value = name;
            command.Parameters.Add("@field", System.Data.DbType.String).Value = field;

            sqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                String Debug = reader[field].ToString();
                return reader[field].ToString();
            }
            return null;
        }

        /*
         * Returns the int found at the field column for 'name' name
         */
        public int getIntFieldFromName(String name, String field)
        {
            sqliteCommand command = new sqliteCommand("select * from " + m_TableName + " where name = @name", m_Connection);

            command.Parameters.Add("@name", System.Data.DbType.String).Value = name;
            command.Parameters.Add("@field", System.Data.DbType.String).Value = field;

            sqliteDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if (int.TryParse(reader[field].ToString(), out int j))
                    {
                        return j;
                    }
                }
            }
            return -1;
        }

        /*
         * Returns the string found at the field column for 'id' id
         */
        public String getStringFieldFromId(int id, String field)
        {
            sqliteCommand command = new sqliteCommand("select @field from " + m_TableName + " where id = @id", m_Connection);

            command.Parameters.Add("@id", System.Data.DbType.UInt32).Value = id;
            command.Parameters.Add("@field", System.Data.DbType.String).Value = field;

            sqliteDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                return reader[field].ToString();
            }
            return null;
        }

        /*
         * Returns all the names of entrys where the field == query
         */
        public List<String> getNamesFromField(String field, String query)
        {
            List<String> returnList = new List<string>();

            try
            {
                sqliteCommand command = new sqliteCommand("select name from " + m_TableName + " where @field = @query", m_Connection);

                command.Parameters.Add("@query", System.Data.DbType.String).Value = query;
                command.Parameters.Add("@field", System.Data.DbType.String).Value = field;

                sqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    returnList.Add(reader["name"].ToString());
                }
            }
            catch { }

            return returnList;
        }

        /*
         * Returns all the names of entrys where the field == query for two queries
         */
        public List<String> getNamesFromTwoFields(String fieldOne, String queryOne, String fieldTwo, String queryTwo)
        {
            sqliteCommand command = new sqliteCommand("select name from " + m_TableName + " where @field1 = @query1 and @field2 = @query2", m_Connection);

            command.Parameters.Add("@query1", System.Data.DbType.String).Value = queryOne;
            command.Parameters.Add("@query2", System.Data.DbType.String).Value = queryTwo;
            command.Parameters.Add("@field1", System.Data.DbType.String).Value = fieldOne;
            command.Parameters.Add("@field2", System.Data.DbType.String).Value = fieldTwo;

            sqliteDataReader reader = command.ExecuteReader();

            List<String> returnList = new List<string>();

            while (reader.NextResult())
            {
                returnList.Add(reader.ToString());
            }
            return returnList;
        }

        /*
         * Sets the field with the String data for the entry where 'name' == name
         */
        public void setFieldFromName(String name, String data, String field)
        {
            sqliteCommand command = new sqliteCommand("update " + m_TableName + " set @field = @data where name = @name", m_Connection);

            command.Parameters.Add("@data", System.Data.DbType.String).Value = data;
            command.Parameters.Add("@name", System.Data.DbType.String).Value = name;
            command.Parameters.Add("@field", System.Data.DbType.String).Value = field;

            command.ExecuteNonQuery();
        }

        /*
         * Sets the field with the int data for the entry where 'name' == name
         */
        public void setFieldFromName(String name, int data, String field)
        {
            sqliteCommand command = new sqliteCommand("update " + m_TableName + " set " + field + " = @data where name = @name", m_Connection);

            command.Parameters.Add("@data", System.Data.DbType.Int32).Value = data;
            command.Parameters.Add("@name", System.Data.DbType.String).Value = name;
            command.Parameters.Add("@field", System.Data.DbType.String).Value = field;

            command.ExecuteNonQuery();
        }

        /*
         * Sets the field with the String data for the entry where 'id' == id
         */
        public void setFieldFromID(int id, String data, String field)
        {
            sqliteCommand command = new sqliteCommand("update " + m_TableName + " set @field = @data where id = @id", m_Connection);

            command.Parameters.Add("@id", System.Data.DbType.UInt32).Value = id;
            command.Parameters.Add("@data", System.Data.DbType.String).Value = data;
            command.Parameters.Add("@field", System.Data.DbType.String).Value = field;

            command.ExecuteNonQuery();
        }

        /*
         * Sets the field with the int data for the entry where 'id' == id
         */
        public void setFieldFromID(int id, int data, String field)
        {
            sqliteCommand command = new sqliteCommand("update " + m_TableName + " set @field = @data where id = @id", m_Connection);

            command.Parameters.Add("@id", System.Data.DbType.UInt32).Value = id;
            command.Parameters.Add("@data", System.Data.DbType.UInt32).Value = data;
            command.Parameters.Add("@field", System.Data.DbType.String).Value = field;

            command.ExecuteNonQuery();
        }

        /*
         * Overridden in derived classes to exactly match the parameters to the specific table's fields
         */
        public virtual void AddEntry(string[] dataArray) { }
    }

    /*
     * Holds information for each room
     */
    public class DungeonTable : SQLTable
    {
        public DungeonTable(sqliteConnection connection, String tableName, String tableColumns) : base(connection, tableName, tableColumns) { }

        /*
         * Specific parameters for this table's fields and their data types
         */
        public override void AddEntry(string[] dataArray)
        {
            try
            {
                sqliteCommand command = new sqliteCommand("insert into " + m_TableName + " values (@name, @id, @north, @south, @east, @west, @description, @isLocked)", m_Connection);

                command.Parameters.Add("@name", System.Data.DbType.String).Value = dataArray[0];
                command.Parameters.Add("@id", System.Data.DbType.UInt32).Value = dataArray[1];
                command.Parameters.Add("@north", System.Data.DbType.String).Value = dataArray[2];
                command.Parameters.Add("@south", System.Data.DbType.String).Value = dataArray[3];
                command.Parameters.Add("@east", System.Data.DbType.String).Value = dataArray[4];
                command.Parameters.Add("@west", System.Data.DbType.String).Value = dataArray[5];
                command.Parameters.Add("@description", System.Data.DbType.String).Value = dataArray[6];
                command.Parameters.Add("@isLocked", System.Data.DbType.String).Value = dataArray[7];

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to add: " + dataArray[0] + " to DB " + ex);
            }
        }
    }

    /*
     * Holds information for each login. The id field will match the appropriate player's table entry id field
     */
    public class LoginTable : SQLTable
    {
        public LoginTable(sqliteConnection connection, String tableName, String tableColumns) : base(connection, tableName, tableColumns) { }

        /*
         * Specific parameters for this table's fields and their data types
         */
        public override void AddEntry(string[] dataArray)
        {
            try
            {
                sqliteCommand command = new sqliteCommand("insert into " + m_TableName + " values (@name, @password, @salt, @isLoggedIn, @id)", m_Connection);

                command.Parameters.Add("@name", System.Data.DbType.String).Value = dataArray[0];
                command.Parameters.Add("@password", System.Data.DbType.String).Value = dataArray[1];
                command.Parameters.Add("@salt", System.Data.DbType.String).Value = dataArray[2];
                command.Parameters.Add("@isLoggedIn", System.Data.DbType.String).Value = dataArray[3];
                command.Parameters.Add("@id", System.Data.DbType.UInt32).Value = dataArray[4];

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to add: " + dataArray[0] + " to DB " + ex);
            }
        }
    }

    /*
     * Holds information about each player representation
     */
    public class PlayersTable : SQLTable
    {
        public PlayersTable(sqliteConnection connection, String tableName, String tableColumns) : base(connection, tableName, tableColumns) { }

        /*
         * Specific parameters for this table's fields and their data types
         */
        public override void AddEntry(string[] dataArray)
        {
            try
            {
                sqliteCommand command = new sqliteCommand("insert into " + m_TableName + " values (@name, @id, @startRoom, @strength, @dexterity, @constitution, @intelligence, @wisdom, @charisma, @hitPoints, @maxHitPoints, @attackModifier, @attackDamage, @armourClass)", m_Connection);

                command.Parameters.Add("@name", System.Data.DbType.String).Value = dataArray[0];
                command.Parameters.Add("@id", System.Data.DbType.UInt32).Value = dataArray[1];
                command.Parameters.Add("@startRoom", System.Data.DbType.String).Value = dataArray[2];
                command.Parameters.Add("@strength", System.Data.DbType.Int32).Value = dataArray[3];
                command.Parameters.Add("@dexterity", System.Data.DbType.Int32).Value = dataArray[4];
                command.Parameters.Add("@constitution", System.Data.DbType.Int32).Value = dataArray[5];
                command.Parameters.Add("@intelligence", System.Data.DbType.Int32).Value = dataArray[6];
                command.Parameters.Add("@wisdom", System.Data.DbType.Int32).Value = dataArray[7];
                command.Parameters.Add("@charisma", System.Data.DbType.Int32).Value = dataArray[8];
                command.Parameters.Add("@hitPoints", System.Data.DbType.Int32).Value = dataArray[9];
                command.Parameters.Add("@maxHitPoints", System.Data.DbType.Int32).Value = dataArray[10];
                command.Parameters.Add("@attackModifier", System.Data.DbType.Int32).Value = dataArray[11];
                command.Parameters.Add("@attackDamage", System.Data.DbType.Int32).Value = dataArray[12];
                command.Parameters.Add("@armourClass", System.Data.DbType.Int32).Value = dataArray[13];

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to add: " + dataArray[0] + " to DB " + ex);
            }
        }
    }

    /*
     * Holds information about each item in the dungeon
     */
    public class ItemsTable : SQLTable
    {
        public ItemsTable(sqliteConnection connection, String tableName, String tableColumns) : base(connection, tableName, tableColumns) { }

        /*
         * Specific parameters for this table's fields and their data types
         */
        public override void AddEntry(string[] dataArray)
        {
            try
            {
                sqliteCommand command = new sqliteCommand("insert into " + m_TableName + " values (@name, @id, @startRoom, @owner, @description, @isEquipped, @useMessage, @healAmount, @damage, @armourClass, @roomUnlocks)", m_Connection);

                command.Parameters.Add("@name", System.Data.DbType.String).Value = dataArray[0];
                command.Parameters.Add("@id", System.Data.DbType.UInt32).Value = dataArray[1];
                command.Parameters.Add("@startRoom", System.Data.DbType.String).Value = dataArray[2];
                command.Parameters.Add("@owner", System.Data.DbType.String).Value = dataArray[3];
                command.Parameters.Add("@description", System.Data.DbType.String).Value = dataArray[4];
                command.Parameters.Add("@isEquipped", System.Data.DbType.String).Value = dataArray[5];
                command.Parameters.Add("@useMessage", System.Data.DbType.String).Value = dataArray[6];
                command.Parameters.Add("@healAmount", System.Data.DbType.Int32).Value = dataArray[7];
                command.Parameters.Add("@damage", System.Data.DbType.Int32).Value = dataArray[8];
                command.Parameters.Add("@armourClass", System.Data.DbType.Int32).Value = dataArray[9];
                command.Parameters.Add("@roomUnlocks", System.Data.DbType.String).Value = dataArray[10];

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to add: " + dataArray[0] + " to DB " + ex);
            }
        }
    }

    /*
     * holds information about each non playable charcater (NPC)
     */
    public class NPCsTable : SQLTable
    {
        public NPCsTable(sqliteConnection connection, String tableName, String tableColumns) : base(connection, tableName, tableColumns) { }

        /*
         * Specific parameters for this table's fields and their data types
         */
        public override void AddEntry(string[] dataArray)
        {
            try
            {
                sqliteCommand command = new sqliteCommand("insert into " + m_TableName + " values (@name, @id, @room, @speech, @description, @strength, @dexterity, @constitution, @intelligence, @wisdom, @charisma)", m_Connection);

                command.Parameters.Add("@name", System.Data.DbType.String).Value = dataArray[0];
                command.Parameters.Add("@id", System.Data.DbType.UInt32).Value = dataArray[1];
                command.Parameters.Add("@room", System.Data.DbType.String).Value = dataArray[2];
                command.Parameters.Add("@speech", System.Data.DbType.String).Value = dataArray[3];
                command.Parameters.Add("@description", System.Data.DbType.String).Value = dataArray[4];
                command.Parameters.Add("@strength", System.Data.DbType.Int32).Value = dataArray[5];
                command.Parameters.Add("@dexterity", System.Data.DbType.Int32).Value = dataArray[6];
                command.Parameters.Add("@constitution", System.Data.DbType.Int32).Value = dataArray[7];
                command.Parameters.Add("@intelligence", System.Data.DbType.Int32).Value = dataArray[8];
                command.Parameters.Add("@wisdom", System.Data.DbType.Int32).Value = dataArray[9];
                command.Parameters.Add("@charisma", System.Data.DbType.Int32).Value = dataArray[10];

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to add: " + dataArray[0] + " to DB " + ex);
            }
        }
    }

    /*
     * A simple table with one entry that holds the next available unique id availabe to assign to an entry
     */
    public class IdTable : SQLTable
    {
        public IdTable(sqliteConnection connection, String tableName, String tableColumns) : base(connection, tableName, tableColumns) { }

        /*
         * Specific parameters for this table's fields and their data types
         */
        public void AddIDEntry(string name, int id)
        {
            try
            {
                sqliteCommand command = new sqliteCommand("insert into " + m_TableName + " values (@name, @id)", m_Connection);

                command.Parameters.Add("@name", System.Data.DbType.String).Value = name;
                command.Parameters.Add("@id", System.Data.DbType.Int32).Value = id;

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to add: " + name + " to DB " + ex);
            }
        }
    }
}

