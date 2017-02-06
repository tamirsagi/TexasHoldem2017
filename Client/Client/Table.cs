using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace Client
{
    public class Table
    {
        public string           TableName           { set; get; }
        public int              TableID             { set; get; }        
        public int              NumOfPlayers        { set; get; }
        public int              SmallBlind          { set; get; }
        public int              BigBlind            { set; get; }
        public int              MinAmmount          { set; get; }
        public int              MaxPlayers          { set; get; }        
        public int              Pot                 { set; get; }
        public int              CurrentBet          { set; get; }
        public enumGameStatus   GameStatus          { set; get; }
        public List<Player>     Players             { set; get; }        
        public List<Card>       SharedCards         { set; get; }
        
        public UserControlTable UserControlTable    { set; get; }

        public delegate void SendChatTalk(Table table, string chat);
        public event SendChatTalk SendChatTalk_Handler;

        public delegate void SendCardToUserControlTable(Table table, Card card, int toWho, int location);
        public event SendCardToUserControlTable SendCardToUserControlTable_Handler;

        public delegate void NotifyReadyAfterPlayerFolded(Table table, int userID, int tableID);
        public event NotifyReadyAfterPlayerFolded NotifyReadyAfterPlayerFolded_Handler;

        public IClientListener clientListener;

        public Table()
        {

        }
        public Table(int id, string name, int numOfPlayers, int small, int big, int minAmmount, int maxPlayers)
        {
            TableID = id;
            TableName = name;
            NumOfPlayers = numOfPlayers;
            SmallBlind = small;
            BigBlind = big;
            MinAmmount = minAmmount;
            MaxPlayers = maxPlayers;
            GameStatus = enumGameStatus.Finished;
            
            SharedCards = new List<Card>();
            Players = new List<Player>();
            clientListener = new ClientListener();
            clientListener.subscribe_send_chat_talk(this, "");
            clientListener.subscribe_send_card_to_user_control_table(this, null, -1, -1);
            clientListener.subscribe_notify_ready_after_player_folded(this, User.player.PlayerID, TableID);
            
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}/{3} {4} {5}", TableName, NumOfPlayers, SmallBlind, BigBlind, MinAmmount, MaxPlayers);
        }

        //public void addPlayer(Player player)
        //{
        //    if(Players.Count <= (int)player.Position)
        //        Players.Add(player);
        //    else
        //        Players[(int)player.Position] = player;
        //    //
        //}

        public void addPlayer(Player player)
        {
            int nullIndex = Players.FindIndex(_player => _player == null);
            if (nullIndex != -1) Players[nullIndex] = player;
            else if (Players.Count < (int)player.Position+1)
                Players.Add(player);
        }

        public void addPlayersFromServer(List<Player> playersFromServer)
        {
            Players = playersFromServer;
            foreach (var player in Players)
            {
                if (player.UserName.Equals(User.UserName))
                {
                    User.player = player;
                    User.player.IsPlaying = Players.Count == 2;
                    GameStatus = enumGameStatus.Started;
                }
            }
        }

        public enumPosition deletePlayer(string playerName)
        {
            int i = 0;
            foreach (var player in Players)
            {
                if (player.UserName.Equals(playerName))
                {
                    enumPosition pos = player.Position;
                    Players[i] = null;
                    
                    return pos;
                }
                i++;
            }
            return enumPosition.size;
        }

        public void setFlop(Card flop)
        {            
            
            SharedCards.Add(flop);
            if (SendCardToUserControlTable_Handler != null)
                SendCardToUserControlTable_Handler(this, flop, 1, -1);
            
        }

        public void setPlayerCards(Card playerCard, int location)
        {

                if (User.player.cards.Count > 2)
                    User.player.cards = new List<Card>();
                User.player.cards.Add(playerCard);
                UserControlTable.initUserCards();
                if (SendCardToUserControlTable_Handler != null)
                    SendCardToUserControlTable_Handler(this, playerCard, 0, location);         
        }

        public List<Card> sendFlop()
        {
            List<Card> flop = new List<Card>();
            for (int i = 0; i < SharedCards.Count; i++)
            {
                flop.Add(SharedCards[i]);
            }
            return flop;
        }

        public List<Card> sendPlayerCards()
        {
            List<Card> cards = new List<Card>();

            for (int i = 0; i < 2; i++)
            {
                cards.Add( User.player.cards[i]);
            }
            return cards;
        }

        public void SendChatText(string text)
        {
            if (SendChatTalk_Handler != null)
                SendChatTalk_Handler(this, text);   
        }

        public void SetUserControlTable(UserControlTable userControlTable)
        {
            UserControlTable = userControlTable;
        }

        public void ReceiveChatText(string text)
        {
            if(UserControlTable != null)
                UserControlTable.ReceiveChatText(text);
        }

        public void ReceivedGameEvents(string text)
        {
            if (UserControlTable != null)
                UserControlTable.ReceiveGameEvents(text);
        }

        public void ReceivePotChange(int pot)
        {
            if (UserControlTable != null)
                UserControlTable.ReceivePotChange(pot);
        }

        public void ReceiveCurrentPlayer(string currentPlayer, int minAmountToBet)
        {
            CurrentBet = minAmountToBet;
            UserControlTable.ReceiveCurrentPlayer(currentPlayer, minAmountToBet);
        }

        public void setPlayersRols(int smallBlindID, int bigBlindID)
        {
            foreach (var player in Players)
            {
                if(player.PlayerID == smallBlindID)
                    player.Role = enumPlayerRole.SmallBlind;
                else if(player.PlayerID == bigBlindID)
                    player.Role = enumPlayerRole.BigBlind;
                else 
                    player.Role = enumPlayerRole.None;
            }

        }

        public void clearTableAfterRound()
        {
            if (UserControlTable != null)
                UserControlTable.clearAfterRound();
        }

        public enumPosition getPosition(string playerName)
        {
            foreach (var player in Players)
            {
                if (player.UserName.Equals(playerName))
                {
                    enumPosition pos = player.Position;                   
                    return pos;
                }
            }
            return enumPosition.size;
        }

        public int getActivePlayers()
        {
            int actives = 0;
            foreach (var player in Players)
            {
                if (player.IsPlaying)
                    actives++;
            }
            return actives;
        }

        public void setAllPlayersToActive()
        {
            foreach (var player in Players)
            {
                if (player != null)
                    player.IsPlaying = true;
                    
            }
        }

        public void playerHasFolded(string playerName)
        {
            if (getActivePlayers() <= 2)
            {
                clearTableAfterRound();

                if (NotifyReadyAfterPlayerFolded_Handler != null && playerName.Equals(User.UserName))
                    NotifyReadyAfterPlayerFolded_Handler(this, User.UserID, TableID);

                if (!playerName.Equals(User.UserName))
                    ReceivedGameEvents(User.UserName + "Has Won!");
                else
                    ReceivedGameEvents(playerName + "Has Won!");
            }
            else
            {
                enumPosition p = getPosition(playerName);
                UserControlTable.deletePlayerCards(p);

                if (UserControlTable != null)
                {
                    UserControlTable.deletePlayerCards(p);
                    UserControlTable.disableEnableAllPlayButtons(false);
                }
                Players[(int)p].IsPlaying = false;

                
            }
        }

        public void recivePlayerCurrentBalance(string playerName, int currentPlayerBalance)
        {
            foreach (var player in Players)
            {
                if (playerName.Equals(player.UserName))
                    player.Money = currentPlayerBalance;
            }
        }

    }
}
