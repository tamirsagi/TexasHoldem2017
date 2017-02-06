using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace TexasHoldemDBWCF
{
    [ServiceContract]
    public interface ITexasHoldemDB
    {
        [OperationContract]
        bool Register(string f_Name, string l_name, string userName, string Password);
        [OperationContract]
        Player Login(string userName, string pwd);
        [OperationContract]
        bool CheckIfExist(string inTable,string toSearch);
        [OperationContract]
        int CreateTable(string name, int minAmountToEnter, int smallBlind, int bigBlind,int adminId);
        [OperationContract]
        bool DeleteTable(int tableId);
        [OperationContract]
        int GetId(string fromTable,string tableName);
        [OperationContract]
        List<Table> GetAllGamesTables();
        [OperationContract]
        bool JoinToTable(int tableId, int playerId);
        [OperationContract]
        void LeaveTable(int tableId, int playerId);
        [OperationContract]
        Player GetPlayerInfo(int playerId);
        [OperationContract]
        void UpdateMoney(int playerId, int newAmount);
        
    }

    [DataContract]
    public class Table
    {
        [DataMember]
        public int TableID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int MinAmount { get; set; }
        [DataMember]
        public int MaxPlayers { get; set; }
        [DataMember]
        public int SmallBlind { get; set; }
        [DataMember]
        public int BigBlind { get; set; }
        [DataMember]
        public int PlayersInTable { get; set; }
        [DataMember]
        public int AdminId { get; set; }
    }

    [DataContract]
    public class Player
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Pwd { get; set; }
        [DataMember]
        public int TotalMoney { get; set; }
    }

}
