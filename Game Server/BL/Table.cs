using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;


namespace Game_Server
{
    /// <summary>
    /// that class takes care for the game logic.
    /// each table has and id, list of players dealer and functions to maintain the table for each game 
    /// </summary>

    [Serializable]
    public class Table
    {

        #region events

        public delegate void TableMessages(string msg);

        public static event TableMessages TableMessagesNotifier;

        #endregion

        public static readonly int EmptySeat = -1;
        public static readonly int CardsOnTablePosition = -1;
        public static readonly int MinPlayersToRunTable = 1;
        public static readonly int MinPlayersToStart = 2;
        public static readonly int NumberOfInitialCards = 2;
        public static readonly int NumberOfFlopCards = 3;
        public static readonly int MaxPlayersInTable = 4;
        public static readonly int MaxCardsOnTable = 5;

        public int TableId { get; set; }
        public string TableName { get; set; }
        public int MinAmount { get; set; }
        public int MaxPlayers { get; set; }
        public Dictionary<int, Player> Players { get; set; }
        public List<int> PlayersIdByOrder { get; set; }
        public List<Card> CardsOnTable { get; set; }
        public int Pot { get; set; }
        public int CurrentMaxBet { get; set; }
        public Dealer Dealer { get; set; }
        public int SmallBlind { get; set; }
        public int BigBlind { get; set; }
        public int IdNextPlayer { get; set; }
        public int IdSmallBlind { get; set; }
        public int IdBigBlind { get; set; }
        public int Round { get; set; }
        public TableSate GameState { get; set; }
        public int NumberOfBets { get; set; }
        public int ActivePlayers { get; set; }
        public int AdminId { get; set; }
        public bool IsTableFull { get; set; }
        public int PlayersAreReady { get; set; }
        public Message ChatMessage { get; set; }

        public enum TableSate
        {
            NotStarted,
            Started,
            Finished
        };

        public Table(int id, string name, int minAmount, int smallBlind, int bigBlind, int maxPlayers, int adminId)
        {
            TableId = id;
            AdminId = adminId;
            TableName = name;
            MinAmount = minAmount;
            SmallBlind = smallBlind;
            BigBlind = bigBlind;
            MaxPlayers = maxPlayers;
            Dealer = new Dealer(new Deck());
            Players = new Dictionary<int, Player>();
            PlayersIdByOrder = new List<int>();
            CardsOnTable = new List<Card>();
            GameState = TableSate.NotStarted;
            PlayersAreReady = 1;
        }

        public void RunTable()
        {
            bool chatThreadIsUp = false;
            while (Players.Count > 0)
            {
                if (!chatThreadIsUp)
                {
                    chatThreadIsUp = true;
                    ChatMessage = new Message("Admin", ServerManager.WelcomeMessage + TableName);
                    Thread tableChatThread = new Thread(() => ChatManager());
                    tableChatThread.Name = ServerManager.ThreadName + ServerManager.Chat;
                    ServerManager.ThreadsPerTable[TableId].Add(tableChatThread);
                    tableChatThread.Start();
                }

                if (Players.Count == MaxPlayersInTable)
                    IsTableFull = true;

                if (Players.Count == MinPlayersToRunTable && GameState != TableSate.NotStarted)
                    InitialTable();
                else if (Players.Count >= MinPlayersToStart)
                    if (GameState == TableSate.NotStarted && PlayersAreReady >= MinPlayersToStart)
                        StartRound();
                    else if (GameState == TableSate.Finished)
                        FinishRound();
            }

            if (TableMessagesNotifier != null)
                TableMessagesNotifier("Table " + TableId + " Removed");
            ServerManager.ThreadsPerTable.Remove(TableId); //remove all threads for that table
            if (TableMessagesNotifier != null)
                TableMessagesNotifier("thread " + ServerManager.ThreadName + TableName + " is done");
            ServerManager.Tables.Remove(TableId); //removes the table from local dictionary
            DBServicesHandler.DBproxy.DeleteTable(TableId); //remove table id DB
        }

        public void ChatManager()
        {
            while (Players.Count > 0)
            {
                if (!string.IsNullOrEmpty(ChatMessage.Sender) && !string.IsNullOrEmpty(ChatMessage.Msg))
                {
                    foreach (var client in ServerManager.ClientsInTables[TableId])
                        if (client != null)
                            client.UpdateMessageInChat(ChatMessage.Sender + " : " + ChatMessage.Msg);
                    if (TableMessagesNotifier != null)
                        TableMessagesNotifier("Chat was sent to all Players");
                    ChatMessage.Sender = "";
                    ChatMessage.Msg = "";
                }
            }
        }

        public void InitialTable()
        {
            GameState = TableSate.NotStarted;
            CardsOnTable.Clear();
            Dealer.ReOrderDeck();
            Round = 1;
            Pot = BigBlind;
            CurrentMaxBet = 0;
            IdSmallBlind = 0;
            IdBigBlind = 0;
            IdNextPlayer = 0;
        }

        public void StartRound()
        {
            GameState = TableSate.Started;
            SetBlinds();
            CurrentMaxBet = BigBlind;
            DealCards();
            NextPlayerToPlay();
        }

        public void FinishRound()
        {
            if (ActivePlayers == 1)
            {
                PlayersAreReady = 1;
                Round = 0;
                foreach (var player in Players.Values)
                    if (player.IsPlaying)
                    {
                        player.MoneyToPlay += Pot;
                        player.UpdateMoneyInDb();
                        break;
                    }
            }
            else
            {
                PlayersAreReady = 0;
                PlayersInTable playersInTable = new PlayersInTable();
                int  id = EmptySeat;
                for (int i = 0; i < PlayersIdByOrder.Count; i++)
                {
                    id = PlayersIdByOrder[i];
                    if (id != EmptySeat && Players[id].IsPlaying)
                    {
                        playersInTable = (new PlayersInTable()
                        {
                            PlayerId = Players[id].ID,
                            UserName = Players[id].UserName,
                            PoistionInTable = i,
                            BalanceInTable = Players[id].MoneyToPlay,
                            PlayerHand = Players[id].PlayerHand
                        });
                        foreach (var client in ServerManager.ClientsInTables[TableId])
                            if (client != null)
                                client.RevealCardsOnTable(playersInTable);
                    }
                }
                var winners = CheckWinner();
                int cash = (int)Pot / winners.Count;
                foreach (var player in winners)
                {
                    Players[player.PlayerId].MoneyToPlay += cash;
                    Players[player.PlayerId].UpdateMoneyInDb();
                }
                foreach (var client in ServerManager.ClientsInTables[TableId])
                    if (client != null)
                        client.NotifyWinners(winners);
                
            }
            PrepareTable();
            Round++;
            
        }

        public void RegisterToPlayerEvents(int playerId)
        {
            Players[playerId].PlayerBetNotifier += PlayerBetHandler;
            Players[playerId].PlayerFoldedNotifier += PlayerFoldedHandler;
        }

        public void UnRegisterToPlayerEvents(int playerId)
        {
            Players[playerId].PlayerBetNotifier -= PlayerBetHandler;
            Players[playerId].PlayerFoldedNotifier -= PlayerFoldedHandler;
        }

        public int JoinToTable(Player player)
        {

            bool setInsteadNull = false;
            int seat = EmptySeat;
            int nullIndex = PlayersIdByOrder.FindIndex(index => index == EmptySeat);
            if (nullIndex >= 0 && nullIndex < PlayersIdByOrder.Count)
            {
                PlayersIdByOrder[nullIndex] = player.ID;
                seat = nullIndex;
                setInsteadNull = true;
            }
            if (!setInsteadNull)
            {
                PlayersIdByOrder.Add(player.ID);
                seat = Players.Count;
            }
            if (GameState != TableSate.Started && player.IsPlaying)
                ActivePlayers++;

            Players.Add(player.ID, player);
            RegisterToPlayerEvents(player.ID);

            return seat;
        }

        public int LeaveTable(Player player)
        {
            int positionInTable = PlayersIdByOrder.FindIndex(id => id == player.ID);
            ActivePlayers--;
            PlayersAreReady--;
            IsTableFull = false;
            UnRegisterToPlayerEvents(player.ID);
            Players.Remove(player.ID);
            PlayersIdByOrder[positionInTable] = EmptySeat;
            if (Players.Count == MinPlayersToRunTable)
            {
                int lastInTable = PlayersIdByOrder.FindIndex(id => id != EmptySeat);
                Players[PlayersIdByOrder[lastInTable]].MoneyToPlay += Pot;
                Players[PlayersIdByOrder[lastInTable]].UpdateMoneyInDb();
                GameState = TableSate.NotStarted;
            }
            return positionInTable;
        }

        public void SetBlinds()
        {
            int smallIndex = 0, bigIndex = 1;
            if (IdSmallBlind == 0 && IdBigBlind == 0)
            {
                while (PlayersIdByOrder[smallIndex] == EmptySeat) smallIndex++;
                IdSmallBlind = PlayersIdByOrder[smallIndex];
                while (PlayersIdByOrder[bigIndex] == EmptySeat) bigIndex++;
                IdBigBlind = PlayersIdByOrder[bigIndex];
            }
            else
            {
                smallIndex = PlayersIdByOrder.FindIndex(id => id == IdSmallBlind);
                    //get last player who was the Small Blind
                bigIndex = PlayersIdByOrder.FindIndex(id => id == IdBigBlind); //get last player who was the big Blind
                if (++smallIndex >= Players.Count) //in case the next small should be in index 0
                {
                    smallIndex = 0;
                    bigIndex = smallIndex + 1;
                }
                else if (++bigIndex >= PlayersIdByOrder.Count) //in case the next big should be in index 0
                    bigIndex = 0;
            }
            IdSmallBlind = PlayersIdByOrder[smallIndex]; //get the relevant ID
            IdBigBlind = PlayersIdByOrder[bigIndex]; //get the relevant ID
            Players[IdSmallBlind].SetPlayerRole(Player.PlayerRoleOnTable.SmallBlind, SmallBlind);
                //set player rule and charge him
            Players[IdBigBlind].SetPlayerRole(Player.PlayerRoleOnTable.BigBlind, BigBlind);
                //set player rule and charge him
            
            Pot = SmallBlind + BigBlind;        //initial Pot

            foreach (var client in ServerManager.ClientsInTables[TableId])
                //update each client in the table about the small/big/next player in current round
                if (client != null)
                    client.NotifyPlayersRules(IdSmallBlind, IdBigBlind);
        }

        public void DealCards()
        {
            bool allGotCard = false;
            int gotCard = 0;
            int smallBlindIndex = PlayersIdByOrder.FindIndex(id => id == IdSmallBlind);
            int beginDeal;
            var clientsInTable = ServerManager.ClientsInTables[TableId];
            for (int i = 0; i < NumberOfInitialCards; i++)
            {
                beginDeal = smallBlindIndex;
                while (!allGotCard)
                {
                    if (PlayersIdByOrder[beginDeal] != EmptySeat)
                    {
                        var c = Dealer.DealCard();
                        Players[PlayersIdByOrder[beginDeal]].AddCard(c);
                        clientsInTable[beginDeal].SendCardToPlayer(c, Card.CardToPlayer.HAND, beginDeal);
                        gotCard++;
                        beginDeal++;
                        if (beginDeal >= Players.Count)
                            beginDeal = 0;
                        allGotCard = (gotCard >= Players.Count);
                    }
                }
                allGotCard = false;
                gotCard = 0;
            }
        }

        public void PlayerBetHandler(int playerId, int amount)
        {
            bool raised = false;
            NumberOfBets++;
            Pot += amount;
            if (amount > CurrentMaxBet)
            {
                raised = true;
                CurrentMaxBet = amount;
                NextPlayerToPlay();
            }
            
            if (!raised && NumberOfBets == ActivePlayers)
            {
                CurrentMaxBet = BigBlind;
                foreach (var player in Players.Values)
                    player.CurrentBet = 0;

                if (CardsOnTable.Count >= 0 && CardsOnTable.Count < NumberOfFlopCards)
                    Flop();
                else if (CardsOnTable.Count < MaxCardsOnTable)
                    RiverOrTurn();
                else
                    GameState = TableSate.Finished;

                NumberOfBets = 0;
                CurrentMaxBet = Players[IdNextPlayer].CurrentBet;
                if (GameState != TableSate.Finished)
                    NextPlayerToPlay();
            }
            else
                NextPlayerToPlay();
        }

        public void PlayerFoldedHandler(int playerId)
        {
            ActivePlayers--;
            Players[playerId].IsPlaying = false;
            foreach (var client in ServerManager.ClientsInTables[TableId])
                if (client != null)
                    client.NotifyPlayerFold(Players[playerId].UserName);
            if (ActivePlayers == 1)
                GameState = TableSate.Finished;
            else
                NextPlayerToPlay();
        }

        public void NextPlayerToPlay()
        {
            int currentPlayer = 0;
            int indexSmallBlind = PlayersIdByOrder.FindIndex(id => id == IdSmallBlind);
            int indexBigBlind = PlayersIdByOrder.FindIndex(id => id == IdBigBlind);
            if (IdNextPlayer <= 0)
            {
                if (Players.Count == MinPlayersToStart)
                {
                    IdNextPlayer = IdSmallBlind;
                    currentPlayer = indexSmallBlind;
                }
                else
                {
                    IdNextPlayer = PlayersIdByOrder[indexBigBlind + 1];
                    currentPlayer = indexBigBlind + 1;
                }
            }
            else
            {
                currentPlayer = PlayersIdByOrder.FindIndex(id => id == IdNextPlayer);
                if (currentPlayer + 1 >= Players.Count)
                    currentPlayer = 0;
                else
                    currentPlayer++;
                IdNextPlayer = PlayersIdByOrder[currentPlayer];
            }


            while (Players[IdNextPlayer].MoneyToPlay <= 0 || !Players[IdNextPlayer].IsPlaying)
            {
                currentPlayer++;
                if (currentPlayer >= PlayersIdByOrder.Count)
                    currentPlayer = 0;
                IdNextPlayer = PlayersIdByOrder[currentPlayer];
            }
            foreach (var pId in PlayersIdByOrder)
            {
                if (pId != EmptySeat && Players[pId].IsPlaying)
                {
                    if (pId == IdNextPlayer)
                        Players[pId].CanPlay = true;
                    else
                        Players[pId].CanPlay = false;
                }
            }
            for (int i = 0; i < ServerManager.ClientsInTables[TableId].Count; i++)
            {
                var client = ServerManager.ClientsInTables[TableId][i];
                if (client != null)
                {
                    int playerId = ServerManager.Tables[TableId].PlayersIdByOrder[i];
                    int currentPlayerBalance = ServerManager.Tables[TableId].Players[playerId].MoneyToPlay;
                    client.NotifyNextPlayerToPlay(Players[IdNextPlayer].UserName,
                        CurrentMaxBet - Players[IdNextPlayer].CurrentBet, currentPlayerBalance,
                        Pot);
                }
            }
                    
        }

    public void Flop()
        {
            Dealer.BurnCard();
            Card c = null;
            for (int i = 0; i < NumberOfFlopCards; i++)
            {
                c = Dealer.DealCard();
                c.show = true;
                CardsOnTable.Add(c);
                foreach (var client in ServerManager.ClientsInTables[TableId])
                    if(client != null)
                        client.SendCardToPlayer(c,Card.CardToPlayer.TABLE);
            }
        }

        public void RiverOrTurn()
        {
            Dealer.BurnCard();
            Card c = Dealer.DealCard();
            c.show = true;
            CardsOnTable.Add(c);
            foreach (var client in ServerManager.ClientsInTables[TableId])
                if(client != null)
                    client.SendCardToPlayer(c,Card.CardToPlayer.TABLE);
        }


        public List<PlayersInTable> CheckWinner()
        {
            Hand besHand = new Hand();
            int winnerID = -1;
            List<PlayersInTable> winners = new List<PlayersInTable>();
            PlayersInTable playerInTable;
            foreach (var player in Players.Values)
            {
                if (player.IsPlaying)
                {
                    var playerHand = player.PlayerHand.Cards;
                    CardsOnTable.Add(playerHand[playerHand.Count - 1]); //add player inital hand
                    CardsOnTable.Add(playerHand[playerHand.Count - 2]); //add player inital hand
                    var cards = CardsOnTable.OrderBy(card => card.Face).ThenBy(card => card.Suit).Reverse();
                    //sort by face then by suit decending order
                    player.FindBestHand(cards);
                    CardsOnTable.RemoveAt(CardsOnTable.Count - 1); //remove player inital Hand
                    CardsOnTable.RemoveAt(CardsOnTable.Count - 1); //remove player inital Hand
                    playerInTable = new PlayersInTable()
                    {
                        PlayerId = player.ID,
                        UserName = player.UserName,
                        PoistionInTable = PlayersIdByOrder.FindIndex(id => id == player.ID),
                        BalanceInTable = player.MoneyToPlay,
                        PlayerHand = player.BestHand
                    };
                    if (winners.Count == 0)
                        winners.Add(playerInTable);
                    else
                    {
                        int score;
                        if ((score = player.BestHand.CompareTo(winners[0].PlayerHand)) > 0)
                        {
                            winners.Clear(); //player has better hand so remove entire list
                            winners.Add(playerInTable);
                        }
                        else if (score == 0)
                            winners.Add(playerInTable); //best hands are  equal so add to list
                    }
                }
            }
            return winners;
        }

        public void PrepareTable()
        {
            GameState = TableSate.NotStarted;
            CardsOnTable.Clear();
            foreach (var player in Players.Values)
            {
                if (player.MoneyToPlay == 0)
                {
                    int playerPos = PlayersIdByOrder.FindIndex(id => id == player.ID);
                    ServerManager.ClientsInTables[TableId][playerPos].NotifyPlayerKicked();
                }
                else
                {
                    player.ClearHand();
                    player.IsPlaying = true;
                    player.CanPlay = false;
                    player.PlayerRole = Player.PlayerRoleOnTable.Normal;
                }
            }
            Pot = SmallBlind + BigBlind;
            CurrentMaxBet = BigBlind;
            NumberOfBets = 0;
            ActivePlayers = Players.Count;
            Dealer.ReOrderDeck();
        }

        public override string ToString()
        {
            return "[Table Id : = " + TableId + " Table Name = " + TableName + " MaxPlayers = " + MaxPlayers + "\n" +
                   "Min Amount = " + MinAmount + " SmallBlind = " + SmallBlind + " Big Blind = " + BigBlind +
                   " Number Of Players = " + ActivePlayers
                   + " AdminId = " + AdminId + "]";

        }



        

    }//end of class

}
