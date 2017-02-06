using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using TexasHoldemDBWCF;
using System.Data.SqlClient;
using System.Data;

namespace TexasHoldemDBWCF
{
    public class DBManager
    {

        #region Events
        public delegate void DbMessages(string msg);
        public event DbMessages DbMessagesNotifier;
        #endregion


        private const string DbPath = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\Studies\Programming\.Net\TEXAS HOLDEM\TEXASHOLDEMDBWCF\DB\TEXASHOLDEMDB.MDF;Integrated Security=True;Connect Timeout=30";
        private SqlConnection _connection;
        private static DBManager _manager;

        public DBManager(){
            OpenConnection();
        }
            
        public static DBManager Manager
        {
            get
            {
                if (_manager == null)
                    _manager = new DBManager();
                return _manager;
            }
        }

        public enum ResultsCode
        {
           Default = -1, Success = 1 , Failed = -1
        };

        public enum TablesInDB
        {
            GameTable,Player,PlayersInTables
        };

        public void OpenConnection()
        {
            if (_connection == null)
                _connection = new SqlConnection(DbPath);
            _connection.Open();
            if (DbMessagesNotifier != null)
                DbMessagesNotifier("Connection to DB is open");
        }

        public void CloseConnection()
        {
            if (_connection != null)
                _connection.Close();
        }

       public object GetRelevantObject(object recieved,object returnObj)
        {
            if(recieved == DBNull.Value || recieved == null)
		        return returnObj;
            return recieved;
        }


        public bool Register(string f_Name, string l_name, string userName, string Password)
        {
            if (_connection == null)
                OpenConnection();
            try
            {
                if (DbMessagesNotifier != null)
                    DbMessagesNotifier("trying to register user : " + f_Name + " " + l_name + " " + userName);
                SqlCommand command = new SqlCommand("INSERT INTO " + TablesInDB.Player + "([First Name],[Last Name],[User Name],Pwd) VALUES(@FirstName, @LastName,@UserName,@Pwd)", _connection);
                command.Parameters.AddWithValue("@FirstName", f_Name);
                command.Parameters.AddWithValue("@LastName", l_name);
                command.Parameters.AddWithValue("@UserName", userName);
                command.Parameters.AddWithValue("@Pwd", Password);
                if (command.ExecuteNonQuery() == (int)ResultsCode.Success)
                {
                    if (DbMessagesNotifier != null)
                        DbMessagesNotifier("User Succesfully Registered");
                    return true;
                }
                else
                {
                    if (DbMessagesNotifier != null)
                      DbMessagesNotifier("Could not registered user");
                    return false;
                }
            }
            catch (Exception e)
            {
                if (DbMessagesNotifier != null)
                    DbMessagesNotifier(e.Message);
                return false;
            }
        }

        public int Login(string userName,string pwd)
        {
            if (_connection == null)
                OpenConnection();

            if (CheckIfExist(TablesInDB.Player.ToString(), userName))
            {
                if (DbMessagesNotifier != null)
                    DbMessagesNotifier("User " + userName  +" is trying to login");
                SqlCommand command = new SqlCommand("SELECT ID FROM " + TablesInDB.Player.ToString() + " WHERE [User Name] = @userName AND pwd = @password", _connection);
                command.Parameters.AddWithValue("@userName", userName);
                command.Parameters.AddWithValue("@password", pwd);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    reader.Close();
                    return id;
                }
                reader.Close();
                return (int)DBManager.ResultsCode.Failed;
            }
            else
                return (int)DBManager.ResultsCode.Failed;
        }

        public bool CheckIfExist(string inTable, string toSearch)
        {
            if (_connection == null)
                OpenConnection();
            
            string field = "";
            if (inTable.Equals(TablesInDB.GameTable.ToString()))
                field = "Name";
            else if (inTable.Equals(TablesInDB.Player.ToString()))
                field = "[User Name]";
            try
            {
                if (DbMessagesNotifier != null)
                    DbMessagesNotifier("Check if Table " + toSearch + " is exist in table " + inTable);
                SqlCommand command = new SqlCommand("SELECT TOP 1 * FROM " + inTable + " WHERE " + field + " = @toSearch", _connection);
                command.Parameters.AddWithValue("@toSearch", toSearch);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception e)
            {
                if (DbMessagesNotifier != null)
                    DbMessagesNotifier(e.Message);
                return false;
            }
        }

        //function create table and return TableID
        public int CreateTable(string name, int minAmountToEnter, int smallBlind, int bigBlind,int adminId)
        {
            int result = (int)ResultsCode.Default;
            if (_connection == null)
                OpenConnection();
            try
            {
                if (DbMessagesNotifier != null)
                    DbMessagesNotifier("trying to create Table");
                SqlCommand command = new SqlCommand("INSERT INTO " + TablesInDB.GameTable + " (Name,AmountToEnter,[Small Blind],[Big Blind],AdminID,[Number Of Players]) VALUES(@Name,@AmountToEnter,@SmallBlind,@BigBlind,@AdminID,@NumPlayers)", _connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@AmountToEnter", minAmountToEnter);
                command.Parameters.AddWithValue("@SmallBlind", smallBlind);
                command.Parameters.AddWithValue("@BigBlind", bigBlind);
                command.Parameters.AddWithValue("@AdminID", adminId);
                command.Parameters.AddWithValue("@NumPlayers",0);
                result = command.ExecuteNonQuery();
                int tableId;
                if (result == (int)ResultsCode.Success && (tableId = GetId(TablesInDB.GameTable.ToString(), name)) > 0)
                {
                    if (DbMessagesNotifier != null)
                        DbMessagesNotifier("Table " + tableId + " has been created");
                    JoinToTable(tableId, adminId);
                    return tableId;
                }
                else
                    return (int)ResultsCode.Failed;
            }
            catch (Exception e)
            {
                if (DbMessagesNotifier != null)
                    DbMessagesNotifier(e.Message);
                return (int)ResultsCode.Failed;
            }
        }

        public bool DeleteTable(int tableId)
        {
            if (_connection == null)
                OpenConnection();
            try
            {
                if (DbMessagesNotifier != null)
                    DbMessagesNotifier("trying to delete table: " + tableId);
                SqlCommand command = new SqlCommand("DELETE FROM " + TablesInDB.PlayersInTables + " WHERE TableID = @tableID", _connection);
                command.Parameters.AddWithValue("@tableID", tableId);
                if (command.ExecuteNonQuery() >= 0)
                {
                    if (DbMessagesNotifier != null)
                        DbMessagesNotifier("Table " + tableId + " has been deleted from :  " + TablesInDB.PlayersInTables);
                    command = new SqlCommand("DELETE FROM " + TablesInDB.GameTable + " WHERE ID = @tableID", _connection);
                    command.Parameters.AddWithValue("@tableID", tableId);
                    if (command.ExecuteNonQuery() == (int)ResultsCode.Success)
                    {
                        if (DbMessagesNotifier != null)
                            if (DbMessagesNotifier != null) DbMessagesNotifier("Table " + tableId + " has been deleted from :  " + TablesInDB.GameTable);
                        return true;
                    }
                    else
                    {
                        if (DbMessagesNotifier != null) 
                            DbMessagesNotifier("Table " + tableId + " could not been deleted from :  " + TablesInDB.GameTable);
                        return false;
                    }
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                if (DbMessagesNotifier != null)
                    DbMessagesNotifier(e.Message);
                return false;
            }
        }

        //function returns TableID by its name
        public int GetId(string fromTable,string toSearch)
        {
            if (_connection == null)
                OpenConnection();
            try
            {
                if (DbMessagesNotifier != null)
                   DbMessagesNotifier("trying to get ID " + toSearch + " from table " + fromTable);
                SqlCommand command = new SqlCommand("SELECT ID FROM " + fromTable + " WHERE Name = @TableName", _connection);
                command.Parameters.AddWithValue("@TableName", toSearch);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    int id = (int)reader["ID"];
                    if (DbMessagesNotifier != null)
                        DbMessagesNotifier("GetID from " + fromTable + " id is " + id);
                    reader.Close();
                    return id;
                }
            }
            catch(Exception e)
            {
                if (DbMessagesNotifier != null)
                    DbMessagesNotifier(e.Message);
            }
            return (int)ResultsCode.Failed;
        }

        public DataTable GetAllGamesTables()
        {
            if (_connection == null)
                OpenConnection();
            try
            {
                if (DbMessagesNotifier != null)
                    DbMessagesNotifier("trying to get all tables");
                SqlDataAdapter adapter = new SqlDataAdapter();
                DataTable allTables = new DataTable();
                adapter.SelectCommand = new SqlCommand("SELECT * FROM " + TablesInDB.GameTable, _connection);
                adapter.Fill(allTables);
                return allTables;
            }
            catch (Exception e)
            {
                if (DbMessagesNotifier != null)
                    DbMessagesNotifier(e.Message);
                return null;
            }
        }

        public bool JoinToTable(int tableId, int playerId)
        {
            if (_connection == null)
                OpenConnection();
            try
            {
                if (DbMessagesNotifier != null)
                    DbMessagesNotifier("Player " + playerId + " is trying to join into table " + tableId);
                SqlCommand command = new SqlCommand("INSERT INTO " + TablesInDB.PlayersInTables +" (TableID,PlayerID) VALUES(@tableID,@playerID)",_connection);
                command.Parameters.AddWithValue("@tableID",tableId);
                command.Parameters.AddWithValue("@playerID",playerId);
                int result = command.ExecuteNonQuery();
                if(result == (int)ResultsCode.Success)
                {
                    if (DbMessagesNotifier != null)
                        DbMessagesNotifier("Player " + playerId + " has been joint to the table " + tableId);
                    command = new SqlCommand("UPDATE " + TablesInDB.GameTable +" SET [Number Of Players] = [Number Of Players] + 1 WHERE ID = @tableID",_connection);
                    command.Parameters.AddWithValue("@tableID",tableId);
                    return (command.ExecuteNonQuery() == (int)ResultsCode.Success);
                }
                if (DbMessagesNotifier != null)
                    DbMessagesNotifier("End JoinIntoTable");
                return false;
                
            }
            catch (Exception e)
            {
                if (DbMessagesNotifier != null)
                    DbMessagesNotifier(e.Message);
                return false;
            }
        }

        public void LeaveTable(int tableId, int playerId)
        {
            if (_connection == null)
                OpenConnection();
            try
            {
                if (DbMessagesNotifier != null)
                    DbMessagesNotifier("Player " + playerId + " is trying to leave from table " + tableId);
                SqlCommand command = new SqlCommand("DELETE FROM " + TablesInDB.PlayersInTables + " WHERE TableID = @tableID and PlayerID = @playerID", _connection);
                command.Parameters.AddWithValue("@tableID", tableId);
                command.Parameters.AddWithValue("@playerID", playerId);
                if (command.ExecuteNonQuery() == (int)ResultsCode.Success)
                {
                    if (DbMessagesNotifier != null)
                        DbMessagesNotifier("Player " + playerId +" has been deleted from the table " + tableId);
                    command = new SqlCommand("UPDATE " + TablesInDB.GameTable + " SET [Number Of Players] = [Number Of Players] - 1 WHERE ID = @tableID", _connection);
                    command.Parameters.AddWithValue("@tableID", tableId);
                    if (command.ExecuteNonQuery() == (int)ResultsCode.Success && DbMessagesNotifier != null)
                       DbMessagesNotifier("Number of player in table " + tableId  + "has been updated");
                }
            }
            catch (Exception e)
            {
                if(DbMessagesNotifier != null)           
                    DbMessagesNotifier(e.Message);
            }
        }

        public DataRow GetPlayer(int playerId)
        {
            
            if (_connection == null)
                OpenConnection();
            try
            {
                if (DbMessagesNotifier != null)  
                  DbMessagesNotifier("getting player from DB : " + playerId);
                SqlCommand command = new SqlCommand("SELECT * FROM " + TablesInDB.Player + " WHERE ID = @playerID", _connection);
                command.Parameters.AddWithValue("@playerID", playerId);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt.Rows[0];//return the specific record
            }
            catch (Exception e)
            {
                if (DbMessagesNotifier != null)  
                    DbMessagesNotifier(e.Message);
                return null;
            }
        }

        public void UpdateMoney(int playerId,int totalMoney)
        {
            if (_connection == null)
                OpenConnection();
            try
            {
                SqlCommand command = new SqlCommand("UPDATE " + TablesInDB.Player + " SET [Total Money] = @TotalMoney WHERE ID = @playerID", _connection);
                command.Parameters.AddWithValue("@totalMoney", totalMoney);
                command.Parameters.AddWithValue("@playerID", playerId);
                if (command.ExecuteNonQuery() == (int)ResultsCode.Success && DbMessagesNotifier != null)
                    DbMessagesNotifier("Amount " + totalMoney +" has been updated for user " + playerId);
            }
            catch (Exception e)
            {
                if (DbMessagesNotifier != null)  
                    DbMessagesNotifier(e.Message);
            }

        }
        
    }
}
