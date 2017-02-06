using System;
using System.Collections.Generic;
using System.Data;

namespace TexasHoldemDBWCF
{
    public class TexasHoldemDBService : ITexasHoldemDB
    {
        public bool Register(string firstName, string lastName, string userName, string password)
        {
            return DBManager.Manager.Register(firstName, lastName, userName, password);
        }

        public Player Login(string userName,string pwd)
        {
           int userId =  DBManager.Manager.Login(userName, pwd);
           return GetPlayerInfo(userId);
        }

        public bool CheckIfExist(string inTable,string toSearch)
        {
            return DBManager.Manager.CheckIfExist(inTable, toSearch);
        }

        public int CreateTable(string name,int minAmountToEnter, int smallBlind, int bigBlind,int adminID)
        {
            return DBManager.Manager.CreateTable(name, minAmountToEnter, smallBlind, bigBlind, adminID);
        }

        public bool DeleteTable(int tableId)
        {
            return DBManager.Manager.DeleteTable(tableId);
        }

        public int GetId(string fromTable, string tableName)
        {
            return DBManager.Manager.GetId(fromTable, tableName);
        }

        public List<Table> GetAllGamesTables()
        {
            try
            {
                List<Table> availableTables = new List<Table>();
                var dt = DBManager.Manager.GetAllGamesTables();
                foreach (DataRow row in dt.Rows)
                {
                    availableTables.Add(new Table()
                    {
                        TableID = (int)DBManager.Manager.GetRelevantObject(row["ID"],DBManager.ResultsCode.Failed),
                        Name = ((string)DBManager.Manager.GetRelevantObject(row["Name"],"")).Trim(),
                        MaxPlayers = (int)DBManager.Manager.GetRelevantObject(row["Max Players"],DBManager.ResultsCode.Failed),
                        MinAmount = (int)DBManager.Manager.GetRelevantObject(row["AmountToEnter"],DBManager.ResultsCode.Failed),
                        SmallBlind = (int)DBManager.Manager.GetRelevantObject(row["Small Blind"], DBManager.ResultsCode.Failed),
                        BigBlind = (int)DBManager.Manager.GetRelevantObject(row["Big Blind"],DBManager.ResultsCode.Failed),
                        PlayersInTable = (int)DBManager.Manager.GetRelevantObject(row["Number Of Players"],DBManager.ResultsCode.Failed),
                        AdminId = (int)DBManager.Manager.GetRelevantObject(row["AdminID"], DBManager.ResultsCode.Failed),
                    });
                }
                return availableTables;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public bool JoinToTable(int tableId, int playerId)
        {
            return DBManager.Manager.JoinToTable(tableId, playerId);
        }

        public void LeaveTable(int tableId, int playerId)
        {
            DBManager.Manager.LeaveTable(tableId, playerId);
        }


        public Player GetPlayerInfo(int playerId)
        {
            DataRow dt = DBManager.Manager.GetPlayer(playerId);
            if(dt != null)
            {
                Player p = new Player()
                {
                    ID = playerId,
                    FirstName = ((string)DBManager.Manager.GetRelevantObject(dt["First Name"],"")).Trim(),
                    UserName = ((string)DBManager.Manager.GetRelevantObject(dt["User Name"],"")).Trim(),
                    LastName = ((string)DBManager.Manager.GetRelevantObject(dt["Last Name"],"")).Trim(),
                    TotalMoney = (int)DBManager.Manager.GetRelevantObject(dt["Total Money"],DBManager.ResultsCode.Failed)
                };
                return p;
            }
            return null;
        }

        public void UpdateMoney(int playerId, int totalMoney)
        {
            DBManager.Manager.UpdateMoney(playerId,totalMoney);
        }

    }
}
