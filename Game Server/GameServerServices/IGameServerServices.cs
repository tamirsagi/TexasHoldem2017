using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Game_Server
{
    [ServiceContract(Namespace = ServerManager.NameSpace, SessionMode = SessionMode.Required,
        CallbackContract = typeof(IGameServerServicesCallbacks)) ]
    
    public interface IGameServerServices
    {
        [OperationContract]
        bool Register(string firstName,string lastNAme,string userName, string password);
        [OperationContract]
        TexasHoldemDBWCF.Player Login(string userName, string password);
        [OperationContract]
        int CreateNewTable(string tableName, int minAmount, int smallBlind, int bigBlind, int adminId,int amountToPlay);
        [OperationContract]
        bool DeleteTable(int tableId);
        [OperationContract]
        List<PlayersInTable> JoinIntoTable(int tableId, int playerId, int initialAmount);
        [OperationContract]
        void LeaveTable(int tableId,int playerId,bool wasKicked);
        [OperationContract()]
        List<TexasHoldemDBWCF.Table> GetAllGamesTables();
        [OperationContract]
        int GetCurrentBalance(int playerId);
        [OperationContract]
        void UpdateBalance(int playerId,int amount);
        [OperationContract]
        void SendChatMessage(int tableId,string sender,string msg);
        [OperationContract]
        void PlayerIsReadyToPlay(int tableId);
        [OperationContract]
        void Check(int tableId, int playerId);
        [OperationContract]
        void Fold(int tableId, int playerId);
        [OperationContract]
        void Call(int tableId, int playerId,int amount);
        [OperationContract]
        void Raise(int tableId, int playerId, int amount);
        [OperationContract]
        void KickPlayer(int tableId, int playerIdToKick);
    }

    [DataContract]
    public enum TablesInDb
    {
        [EnumMember]
        GameTable,
        [EnumMember]
        Player,
        [EnumMember]
        PlayersInTables
    };

    [DataContract]
    public class PlayersInTable
    {
        [DataMember]
        public int PlayerId { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public int PoistionInTable { get; set; }
        [DataMember]
        public int BalanceInTable { get; set; }
        [DataMember]
        public bool IsPlaying { get; set; }
        [DataMember]
        public Hand PlayerHand { get; set; }
    }

}
