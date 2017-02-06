using System;
using System.Collections.Generic;
using TexasHoldemDBWCF;
namespace Game_Server
{
    public class DBServicesHandler
    {
        #region events

        public delegate void DbServiceMessages(string msg);
        public static event DbServiceMessages DbServicesNotifier;
        #endregion

        public static DBService.TexasHoldemDBClient DBproxy { get; set; }
        private static DBServicesHandler _dbServiceHandler;
        
        private DBServicesHandler() { }

        public static DBServicesHandler DbHandler
        {
            get
            {
                if (_dbServiceHandler == null)
                {
                    _dbServiceHandler = new DBServicesHandler();
                    ConnectToDbServer();
                }
                return _dbServiceHandler; 
            }
        }

        //function creates channel to DBServer ,WCF services
        public static void ConnectToDbServer()
        {
            DBproxy = new DBService.TexasHoldemDBClient();
            DBproxy.Open();
        }

        public bool Register(string firstName,string lastName,string userName, string password)
        {
            ////client section
            if (DBproxy.CheckIfExist(TablesInDb.Player.ToString(),userName))
                return false;
            return DBproxy.Register(firstName, lastName, userName, password);
        }

        public TexasHoldemDBWCF.Player Login(string userName,string pwd)
        {
            return DBproxy.Login(userName, pwd);
        }

        public bool CheckIfExist(string inTable,string toSearch)
        {
            return DBproxy.CheckIfExist(inTable, toSearch);
        }

        public int CreateTable(string tableName, int minAmount, int smallBlind, int bigBlind,int adminId)
        {
            return DBproxy.CreateTable(tableName, minAmount, smallBlind, bigBlind, adminId);
        }

        public bool DeleteTable(int tableId)
        {
            return DBproxy.DeleteTable(tableId);
        }

        public int GetId(string fromTable,string toSearch)
        {
            return DBproxy.GetId(fromTable, toSearch);
        }

        public List<TexasHoldemDBWCF.Table> GetAllGamesTables()
        {
           return DBproxy.GetAllGamesTables();
        }

        public bool JoinToTable(int tableId,int playerId)
        {
            try
            {
                return DBproxy.JoinToTable(tableId, playerId);
            }catch(Exception e)
            {
                if (DbServicesNotifier != null)
                    DbServicesNotifier(e.Message);
                return false;
            }
        }

        public void LeaveTable(int tableId, int playerId)
        {
            try
            {
                DBproxy.LeaveTable(tableId, playerId);
            }
            catch (Exception e)
            {
                if (DbServicesNotifier != null)
                    DbServicesNotifier(e.Message);
            }
        }

        public Player GetPlayerInfo(int playerId)
        {
            var p = DBproxy.GetPlayerInfo(playerId);
            return new Player(p.ID, p.FirstName, p.LastName, p.UserName, p.TotalMoney);
        }

        public void UpdateMoney(int playerId,int total)
        {
            DBproxy.UpdateMoney(playerId, total);
        }

    }
}
