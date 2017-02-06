using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;

namespace Game_Server
{
    [ServiceBehavior(Namespace = ServerManager.NameSpace, 
        ConcurrencyMode = ConcurrencyMode.Reentrant ,InstanceContextMode = InstanceContextMode.PerSession)]

    public class GameServerServices : IGameServerServices
    {
        #region Events
        public delegate void GameServerServicesMessages(string msg);
        public static event GameServerServicesMessages GameServerServicesMessagesNotifier;

        #endregion


        public bool Register(string firstName,string lastName,string userName, string password)
        {
            return (DBServicesHandler.DbHandler.Register(firstName, lastName, userName, password));
        }

        public int CreateNewTable(string tableName, int minAmount, int smallBlind, int bigBlind,int adminId ,int amountToPlay)
        {
            if (GameServerServicesMessagesNotifier != null)
                GameServerServicesMessagesNotifier("In CreateNewTable");
            if (DBServicesHandler.DbHandler.CheckIfExist(TablesInDb.GameTable.ToString(), tableName))
            {
                if (GameServerServicesMessagesNotifier != null)
                    GameServerServicesMessagesNotifier("Table name is already exist");
                return (int) ServerManager.Status.Failed;
            }
            int tableId;
            if ((tableId = DBServicesHandler.DbHandler.CreateTable(tableName, minAmount, smallBlind, bigBlind,adminId)) > 0)
            {
                Player admin  = DBServicesHandler.DbHandler.GetPlayerInfo(adminId);
                admin.MoneyToPlay = amountToPlay;
                Table newTable = new Table(tableId, tableName, minAmount, smallBlind, bigBlind, Table.MaxPlayersInTable, adminId);
                newTable.JoinToTable(admin);
                var tableAdmin = OperationContext.Current.GetCallbackChannel<IGameServerServicesCallbacks>();
                ServerManager.ClientsInTables.Add(tableId, new List<IGameServerServicesCallbacks>() { tableAdmin });
                if (GameServerServicesMessagesNotifier != null)
                    GameServerServicesMessagesNotifier("table " + newTable + " has been created");
                Thread tableThread = new Thread(() => newTable.RunTable());
                tableThread.Name = ServerManager.ThreadName + tableName;
                ServerManager.Tables.Add(tableId, newTable);
                ServerManager.ThreadsPerTable.Add(tableId, new List<Thread>() { tableThread });
                tableThread.Start();
                return tableId;
            }
            else
                return (int)ServerManager.Status.Failed;
        }

        public bool DeleteTable(int tableId)
        {
            if (GameServerServicesMessagesNotifier != null)
                GameServerServicesMessagesNotifier("DeleteTable");
            return DBServicesHandler.DbHandler.DeleteTable(tableId);
        }

        public List<TexasHoldemDBWCF.Table> GetAllGamesTables()
        {
            if (GameServerServicesMessagesNotifier != null)
                GameServerServicesMessagesNotifier("GetAllGamesTables");
            return DBServicesHandler.DbHandler.GetAllGamesTables();
        }

        public TexasHoldemDBWCF.Player Login(string userName, string password)
        {
            if (GameServerServicesMessagesNotifier != null)
                GameServerServicesMessagesNotifier("Login");
            TexasHoldemDBWCF.Player user =  DBServicesHandler.DbHandler.Login(userName, password);
            if(user != null)
            {
                IGameServerServicesCallbacks client = OperationContext.Current.GetCallbackChannel<IGameServerServicesCallbacks>();
                if (!ServerManager.ClientsInServer.Contains(client))
                {
                    ServerManager.ClientsInServer.Add(client);
                    if (GameServerServicesMessagesNotifier != null)
                        GameServerServicesMessagesNotifier("User Logged in Client : " + client +" \nplayer: " + userName);
                }
            }
            return user;
        }

        public List<PlayersInTable> JoinIntoTable(int tableId, int playerId, int initialAmount)
        {
            if (GameServerServicesMessagesNotifier != null)
                GameServerServicesMessagesNotifier("In JoinToTable");
            var newPlayer = DBServicesHandler.DbHandler.GetPlayerInfo(playerId);
            newPlayer.MoneyToPlay = initialAmount;
            if (ServerManager.Tables[tableId].GameState == Table.TableSate.Started)
                newPlayer.IsPlaying = false;
            else
                newPlayer.IsPlaying = true;
            if (!ServerManager.Tables[tableId].IsTableFull)
            {
                DBServicesHandler.DbHandler.JoinToTable(tableId, playerId);
                var player = OperationContext.Current.GetCallbackChannel<IGameServerServicesCallbacks>();
                int seat =  ServerManager.Tables[tableId].JoinToTable(newPlayer);
                PlayersInTable joinedPlayer = new PlayersInTable()
                {
                    PlayerId = playerId,
                    UserName = newPlayer.UserName,
                    BalanceInTable = initialAmount,
                    PoistionInTable = seat,
                    IsPlaying = newPlayer.IsPlaying
                    
                };
                foreach (var client in ServerManager.ClientsInTables[tableId])
                    if (client != null)
                        client.NotifyPlayerJoinedTable(joinedPlayer);
                int posToInsert = ServerManager.ClientsInTables[tableId].FindIndex(client => client == null);
                if (posToInsert != Table.EmptySeat)
                    ServerManager.ClientsInTables[tableId][posToInsert] = player;
                else
                    ServerManager.ClientsInTables[tableId].Add(player);
                if (GameServerServicesMessagesNotifier != null)
                    GameServerServicesMessagesNotifier(newPlayer.UserName + " has joint the room : " + tableId);
                var relevantTable = ServerManager.Tables[tableId].PlayersIdByOrder;
                List<PlayersInTable> playersInTables = new List<PlayersInTable>();
                int Id;
                for (int i = 0; i < relevantTable.Count; i++)
                {
                    if ((Id = relevantTable[i]) != Table.EmptySeat)
                    {
                        playersInTables.Add(new PlayersInTable()
                        {
                            PlayerId = Id,
                            UserName = ServerManager.Tables[tableId].Players[Id].UserName,
                            PoistionInTable = i,
                            BalanceInTable = ServerManager.Tables[tableId].Players[Id].MoneyToPlay,
                            IsPlaying = ServerManager.Tables[tableId].Players[Id].IsPlaying
                        });
                    }
                }
                return playersInTables;
            }
            else
            {
                if (GameServerServicesMessagesNotifier != null)
                    GameServerServicesMessagesNotifier("Player " + playerId + "Could not Join To Table,Table is full");
                return null;
            }
        }

        public void LeaveTable(int tableId,int playerId,bool wasKicked)
        {
            if (GameServerServicesMessagesNotifier != null)
                GameServerServicesMessagesNotifier("LeaveTable");
            DBServicesHandler.DbHandler.LeaveTable(tableId, playerId);
            Player currentPlayer = DBServicesHandler.DbHandler.GetPlayerInfo(playerId);
            int positionInTable = ServerManager.Tables[tableId].LeaveTable(currentPlayer);
            ServerManager.ClientsInTables[tableId][positionInTable] = null;
            foreach (var client in ServerManager.ClientsInTables[tableId])
                if(client != null)
                    client.NotifyPlayerLeftTable(currentPlayer.UserName, positionInTable, wasKicked);
            
            if (ServerManager.ClientsInTables[tableId].Count == 0)
                ServerManager.ClientsInTables.Remove(tableId);
            if (GameServerServicesMessagesNotifier != null)
            {
                string extra = "";
                if (wasKicked)
                    extra = "has been kicked from table";
                else
                    extra = "has left the table";
                GameServerServicesMessagesNotifier("Player " + playerId + " " + extra + tableId);
            }

        }

        public void UpdateBalance(int playerId,int amount)
        {
            if (GameServerServicesMessagesNotifier != null)
                GameServerServicesMessagesNotifier("UpdateBalance");
            DBServicesHandler.DbHandler.UpdateMoney(playerId,amount);
        }

        public int GetCurrentBalance(int playerId)
        {
            int money = DBServicesHandler.DbHandler.GetPlayerInfo(playerId).TotalMoney;
            if (GameServerServicesMessagesNotifier != null)
                GameServerServicesMessagesNotifier("Current Balance : " + money);
            return money;
        }

        public void SendChatMessage(int tableId, string sender, string msg)
        {
            if (GameServerServicesMessagesNotifier != null)
                GameServerServicesMessagesNotifier("SendChatMessage\n" + sender + ":" + msg);
            ServerManager.Tables[tableId].ChatMessage = new Message(sender,msg);
        }

        public void PlayerIsReadyToPlay(int tableId)
        {
            if (GameServerServicesMessagesNotifier != null)
                GameServerServicesMessagesNotifier("Another Player is Ready");
            ServerManager.Tables[tableId].PlayersAreReady++;
        }


        public void Check(int tableId, int playerId)
        {
            if (GameServerServicesMessagesNotifier != null)
                GameServerServicesMessagesNotifier("Table + " + tableId + "User " + playerId + " has Checked");
            foreach (var client in ServerManager.ClientsInTables[tableId])
                if(client != null)
                    client.NotifyPlayerAction(ServerManager.Tables[tableId].Players[playerId].UserName, Player.PlayerAction.Check,0);
            ServerManager.Tables[tableId].Players[playerId].Bet(0);

        }

        public void Fold(int tableId, int playerId)
        {
            if (GameServerServicesMessagesNotifier != null)
                GameServerServicesMessagesNotifier("Table + " + tableId + "User " + playerId + " has Fold");
            foreach (var client in ServerManager.ClientsInTables[tableId])
                if (client != null)
                    client.NotifyPlayerAction(ServerManager.Tables[tableId].Players[playerId].UserName, Player.PlayerAction.Fold,0);
            ServerManager.Tables[tableId].Players[playerId].Fold();
        }

        public void Call(int tableId, int playerId, int amount)
        {
            if (GameServerServicesMessagesNotifier != null)
                GameServerServicesMessagesNotifier("Table + " + tableId + "User " + playerId + " has Call with " + amount);
            foreach (var client in ServerManager.ClientsInTables[tableId])
                if (client != null)
                    client.NotifyPlayerAction(ServerManager.Tables[tableId].Players[playerId].UserName, Player.PlayerAction.Call, amount);
            ServerManager.Tables[tableId].Players[playerId].Bet(amount);
        }

        public void Raise(int tableId, int playerId, int amount)
        {
            if (GameServerServicesMessagesNotifier != null)
                GameServerServicesMessagesNotifier("Table + " + tableId + "User " + playerId + " has Raised with " + amount);
            foreach (var client in ServerManager.ClientsInTables[tableId])
                if (client != null)
                    client.NotifyPlayerAction(ServerManager.Tables[tableId].Players[playerId].UserName, Player.PlayerAction.Raise, amount);
            ServerManager.Tables[tableId].Players[playerId].Raise(amount);

        }

        public void KickPlayer(int tableId, int playerIdToKick)
        {
            if (GameServerServicesMessagesNotifier != null)
                GameServerServicesMessagesNotifier("Admin " + ServerManager.Tables[tableId].AdminId +
                                                   " in Table " + tableId + "Has Kicked Player " + playerIdToKick);
            int clientIndex = ServerManager.Tables[tableId].PlayersIdByOrder.FindIndex(id => id == playerIdToKick);
            ServerManager.ClientsInTables[tableId][clientIndex].NotifyPlayerKicked();
        }
    }
}
