using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using Client.GameServerServices;
using System.Windows.Documents;
using System.Threading;
namespace Client
{
    /// <summary>
    /// this is the only thing that is not GUI related, could be replaced with any other controller.
    /// </summary>
    public class Controller : IController , GameServerServices.IGameServerServicesCallback
    {
        #region Delegates and Events
        
        public delegate void SetSharedCard(Controller user, Card card, Table table, int toWho, int location);
        public event SetSharedCard SetSharedCard_Handler;

        public delegate void ReceiveChatText(Controller user, Table table, string toDisplay);
        public event ReceiveChatText ReceiveChatText_Handler;

        public delegate void ReceivePot(Controller user, Table table, int pot);
        public event ReceivePot ReceivePot_Handler;

        public delegate void ReceiveCurrentPlayer(Controller user, Table table, string currentPlayer, int minAmountToBet, int currentPlayerBalance);
        public event ReceiveCurrentPlayer ReceiveCurrentPlayer_Handler;

        public delegate void ReceivedGameEvents(Controller user, Table table, string toDisplay);
        public event ReceivedGameEvents ReceivedGameEvents_Handler;

        public delegate void NotifyPlayerHasJoinedTable(Controller user, Table table, Player player);
        public event NotifyPlayerHasJoinedTable NotifyPlayerJoinedTable_Handler;

        public delegate void NotifyPlayersRoles(Controller user, Table table, int smallBlindID, int bigBlindID);
        public event NotifyPlayersRoles NotifyPlayersRoles_Handler;

        public delegate void NotifyDeletePlayerFromTable(Controller user, Table table, string playerName);
        public event NotifyDeletePlayerFromTable NotifyDeletePlayerFromTable_Handler;

        public delegate void NotifyToCloseTableAfterKick(Controller user, Table table);
        public event NotifyToCloseTableAfterKick NotifyToCloseTableAfterKick_Handler;

        public delegate void SetAllPlayersCards(Controller user, Table table, Player players);
        public event SetAllPlayersCards SetAllPlayersCards_Handler;

        public delegate void NotifyFinishedRound(Controller user, Table table);
        public event NotifyFinishedRound NotifyFinishedRound_Handler;

        public delegate void NotifyPlayerHasFolded(Controller user, Table table, string playerName);
        public event NotifyPlayerHasFolded NotifyPlayerHasFolded_Handler;

        public delegate void NotifyPlayerIsAdmin(Controller user, Table table, string playerName);
        public event NotifyPlayerIsAdmin NotifyPlayerIsAdmin_Handler;
        
        #endregion

        #region Class members

        public IControllerListener controllerListener;
        private static Controller mInstance;
        public string Address { get; set; }
        public int balance { get; set; }
        public readonly int CardDelay = 2000;
        public readonly string ConnectionLost = "Connection to the server lost, please try again...";
        public GameServerServices.GameServerServicesClient proxy { get; set; }

        #endregion

        #region Constructors
        
        private Controller(string address)
        {
            Address = address;
            CreateChannelToGameService(Address);
            controllerListener = new ControllerListener();
            controllerListener.subscribe_set_shared_card(this, null, null, -1, -1);
            controllerListener.subscribe_receive_chat_text(this, User.Table, "");
            controllerListener.subscribe_receive_pot(this, User.Table, -1);
            controllerListener.subscribe_receive_current_player(this, User.Table, "", -1, -1);
            controllerListener.subscribe_received_game_events(this, User.Table, "");
            controllerListener.subscribe_notify_player_joined_table(this, User.Table, null);
            controllerListener.subscribe_notify_players_roles(this, User.Table, -1, -1);
            controllerListener.subscribe_notify_to_close_table_after_kick(this, User.Table);
            controllerListener.subscribe_notify_delete_player_from_table(this, User.Table, "");
            controllerListener.subscribe_set_all_players_cards(this, User.Table, null);
            controllerListener.subscribe_notify_finished_round(this, User.Table);
            controllerListener.subscribe_notify_player_has_folded(this, User.Table, "");
            controllerListener.subscribe_notify_player_is_admin(this, User.Table, "");
        }

        private Controller()
        {

        }

        public static Controller Instance(string address)
        {
            if (mInstance == null)
            {
                mInstance = new Controller(address);
            }
            return mInstance;
        }

        
        

        public static Controller Instance()
        {
            if (mInstance == null)
            {
                mInstance = new Controller();
            }
            return mInstance;
        }
        #endregion

        #region Proxy

        public void notify_login_loginScreen_was_pressed(object userControlLogin, string userName, string password, int userID)
        {
            var playerInfo = proxy.Login(userName, password);
            if (playerInfo != null)
            {
                User.setUser(playerInfo.UserName, password, playerInfo.TotalMoney, playerInfo.ID);
            }
            else
            {
                throw (new Exception("Player doesn't exist!"));
            }
        }

        public List<Table> getTablesFromServer()
        {
            var returnTables = proxy.GetAllGamesTables();
            List<Table> tables = new List<Table>();
            foreach (var t in returnTables)
                tables.Add(new Table(t.TableID, t.Name, t.PlayersInTable, t.SmallBlind, t.BigBlind, t.MinAmount, t.MaxPlayers));
            return tables;
        }

        public int createTable(string tableName, int smallBlind, int bigBlind, int minAmmount, int adminID, int buyIn)
        {
            try
            {
                int res = proxy.CreateNewTable(tableName, minAmmount, smallBlind, bigBlind, adminID, buyIn);
                return res;
            }
            catch (Exception ex)
            {
                throw (new Exception("Could not create table"));
            }
        }

        public void placeBet(int tableID, int userID, int sumToBet, int currentBet)
        {
            Thread t;
            try
            {
                if (currentBet == sumToBet)
                {
                    t = new Thread(() => proxy.Call(tableID, userID, sumToBet));
                }
                else
                {
                    t = new Thread(() => proxy.Raise(tableID, userID, sumToBet));
                }
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void check(int tableID, int userID)
        {
            try
            {
                Thread t = new Thread(() => proxy.Check(tableID, userID));
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void fold(int userID)
        {
            Thread t = new Thread(() => proxy.Fold(User.Table.TableID, userID));
            t.Start();
        }

        public void notify_leave_table(int userID, int TableID, int userMoney)
        {
            try
            {
                proxy.LeaveTable(TableID, userID, false);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ConnectionLost);
            }
        }

        public List<Player> RequestJoinTable(int userID, int tableID, int amountToEnter)
        {
            /*debug*/
             List<Player> p;
            if(User.Table != null)
                 p = User.Table.Players;
            List<Player> playersToReturn = new List<Player>();
            try
            {
                var playersInTable = proxy.JoinIntoTable(tableID, userID, amountToEnter);
                foreach (var player in playersInTable)
                {
                    playersToReturn.Add(new Player(player.UserName, player.BalanceInTable, player.PoistionInTable, player.PlayerId, player.IsPlaying));

                }

                return playersToReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void KickPlayer(int PlayerID, int tableID)
        {
            proxy.KickPlayer(tableID, PlayerID);
        }

        public void SendChatText(int TableID, string chat)
        {
            Thread t = new Thread(() =>proxy.SendChatMessage(TableID, User.UserName, chat));
            t.Start();
        }

        public void notifyPlayerIsReady(int userID, int tableID)
        {
            Thread t = new Thread(() => proxy.PlayerIsReadyToPlay(tableID));
            t.Start();
        }

        public void NotifyPlayerKicked()
        {

            Thread t = new Thread(() => proxy.LeaveTable(User.Table.TableID, User.UserID, true));
            t.Start();

            if (NotifyToCloseTableAfterKick_Handler != null)
                NotifyToCloseTableAfterKick_Handler(this, User.Table);

        }

        private void getBalance(int userID)
        {
            balance = proxy.GetCurrentBalance(userID);
        }

        public int getCurrentBalance(int userID)
        {
            //int balance = 0;
            Thread t = new Thread(() => getBalance(userID));
            t.Start();
            return balance;
        }

        public void CreateChannelToGameService(string address)
        {
            proxy = new GameServerServices.GameServerServicesClient(new InstanceContext(this), "TCPBinding", address);
            proxy.Open();
            if (proxy == null)
                throw (new Exception("Could not connect to server..."));

        }

        #endregion

        #region CallBacks

        public Client.Card cardConverter(GameServerServices.Card card)
        {
            return new Client.Card((int)card.Facek__BackingField, (int)card.Suitk__BackingField, card.showk__BackingField);
        }

        public void NotifyBlindsChanged(int smallBlind, int bigBlind)
        {
            /*debug*/
            List<Player> p;
            if(User.Table != null)
                p = User.Table.Players;
            if (ReceivedGameEvents_Handler != null)
            {
                string small = string.Format("New {0} blind: {1}", "small", smallBlind);
                string big = string.Format("New {0} blind: {1}", "big", bigBlind);
                ReceivedGameEvents_Handler(this, User.Table, small);
                ReceivedGameEvents_Handler(this, User.Table, big);
            }
        }

        public void NotifyPlayersRules(int smallBlindID, int bigBlindID)
        {
            /*debug*/
            List<Player> p;
            if (User.Table != null)
                p = User.Table.Players;
            if (NotifyPlayersRoles_Handler != null)
                NotifyPlayersRoles_Handler(this, User.Table, smallBlindID, bigBlindID);
            
        }

        public void NotifyNextPlayerToPlay(string PlayerToPlay, int minAmountToBet, int currentPlayerBalance, int pot)
        {
            /*debug*/
            List<Player> p;
            if (User.Table != null)
                p = User.Table.Players;

            if (ReceiveCurrentPlayer_Handler != null)
                ReceiveCurrentPlayer_Handler(this, User.Table, PlayerToPlay, minAmountToBet, currentPlayerBalance);

            if (ReceivePot_Handler != null)
                ReceivePot_Handler(this, User.Table, pot);


        }

        public void NotifyPlayerFold(string UserName)
        {
            if (NotifyPlayerHasFolded_Handler != null)
                NotifyPlayerHasFolded_Handler(this, User.Table, UserName);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="card"></param>
        /// <param name="cardToPlayer"></param>
        /// <param name="position">Player location on the server 0-3</param>
        public void SendCardToPlayer(GameServerServices.Card card, GameServerServices.Card.CardToPlayer cardToPlayer, int position)
        {

            Card c = cardConverter(card);
            int location = position;

            if (cardToPlayer == GameServerServices.Card.CardToPlayer.HAND)
            {
                if (SetSharedCard_Handler != null)
                    SetSharedCard_Handler(this, c, User.Table, 0, location);
            }
            else if (cardToPlayer == GameServerServices.Card.CardToPlayer.TABLE)
            {
                if (SetSharedCard_Handler != null)
                    SetSharedCard_Handler(this, c, User.Table, 1, -1);
            }

            else throw new Exception("Coulden't get cards!");

        }

        public void UpdateMessageInChat(string msg)
        {
            if (ReceiveChatText_Handler != null)
                ReceiveChatText_Handler(this, User.Table, msg);
        }

        public void NotifyPlayerJoinedTable(PlayersInTable player)
        {
            /*debug*/
            List<Player> p;
            if (User.Table != null)
                p = User.Table.Players;
            Player tempPlayer = new Player(player.UserName, player.BalanceInTable, player.PoistionInTable, player.PlayerId,player.IsPlaying);
            tempPlayer.TableID = User.Table.TableID;
            if(NotifyPlayerJoinedTable_Handler != null)
                NotifyPlayerJoinedTable_Handler(this, User.Table, tempPlayer);

            if (ReceivedGameEvents_Handler != null)
            {
                string temp = string.Format("{0} has joined the table", tempPlayer.UserName);
                ReceivedGameEvents_Handler(this, User.Table, temp);
            }
        }

        public void NotifyPlayerLeftTable(string playerName, int position, bool wasKicked)
        {
            /*debug*/
            List<Player> p;
            if (User.Table != null)
                p = User.Table.Players;
            string t;
            if (!wasKicked)
                t = "has left the table";
            else t = "was kicked from the table";

                string temp = string.Format("{0} {1}", playerName,t);

            if (NotifyDeletePlayerFromTable_Handler != null)
                NotifyDeletePlayerFromTable_Handler(this, User.Table, playerName);

            if (playerName != User.UserName && ReceivedGameEvents_Handler != null)
                ReceivedGameEvents_Handler(this, User.Table, temp);
        }

        public void RevealCardsOnTable(PlayersInTable player)
        {
            Player tempPlayer = new Player(player.UserName, player.BalanceInTable, player.PoistionInTable, player.PlayerId,player.IsPlaying);            
            tempPlayer.cards[0] = cardConverter(player.PlayerHand.Cardsk__BackingField[0]);
            tempPlayer.cards[1] = cardConverter(player.PlayerHand.Cardsk__BackingField[1]);
            if (SetAllPlayersCards_Handler != null)
                SetAllPlayersCards_Handler(this, User.Table, tempPlayer);
        }

        public void NotifyWinners(PlayersInTable[] winners)
        {

            /*debug*/
            List<Player> p;
            if (User.Table != null)
                p = User.Table.Players;
            for (int i = 0; i < winners.Length; i++)
            {
                string temp = string.Format("{0} has won with!\n{1} {2}", winners[i].UserName,winners[i].PlayerHand.Rankk__BackingField ,winners[i].PlayerHand._sequence);
                if (ReceivedGameEvents_Handler != null)
                    ReceivedGameEvents_Handler(this, User.Table, temp);

            }

            Thread.Sleep(CardDelay);

            if (NotifyFinishedRound_Handler != null)
                NotifyFinishedRound_Handler(this, User.Table);

            notifyPlayerIsReady(User.UserID, User.Table.TableID);
            
        }

        public void NotifyPlayerAction(string userName, PlayerPlayerAction action, int amount)
        {
            string amountString = amount.ToString();
            if (action == PlayerPlayerAction.Check || action == PlayerPlayerAction.Fold)
                amountString = "";
            string temp = string.Format("{0} has {1} {2}", userName, action, amountString);
            if (ReceivedGameEvents_Handler != null)
                ReceivedGameEvents_Handler(this, User.Table, temp);
        }

        #endregion



    }
}
