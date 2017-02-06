using System;
using System.Collections.Generic;

namespace Game_Server
{
    [Serializable]
    public class Player
    {
        #region Events
        public delegate void PlayerBet(int playerId,int bet);
        public event PlayerBet PlayerBetNotifier;

        public delegate void PlayerFold(int playerId);
        public event PlayerFold PlayerFoldedNotifier;

        public delegate void PlayerMessages(string msg);
        public static event PlayerMessages PlayerMessagesNotifier;
        #endregion

        ///////Params //////////
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public int TotalMoney { get; set; }
        public Hand PlayerHand { get; set; }
        public bool IsPlaying { get; set; }
        public bool CanPlay { get; set; }
        public PlayerAction PlayerState { get; set; }
        public int CurrentBet { get; set; }
        public int TotalBet { get; set; }
        public PlayerRoleOnTable PlayerRole { get; set; }
       
        public List<Hand> PossibleHands { get; set; }
        public Hand BestHand { get; set; }

        private bool _reduceAmountWhenJoinTable;
        
        private int _moneyToPlay;
      
        public enum PlayerAction
        {
            Check, Call, Raise, AllIn, Fold
        };

        public enum PlayerRoleOnTable
        {
            Normal, SmallBlind, BigBlind
        };

        public Player() { }

        public Player(int id,string firstName,string lastName,string userName,int totalMoney)
        {
            ID = id;
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            TotalMoney = totalMoney;
            PlayerHand = new Hand();
            IsPlaying = true ;
            CanPlay = false;
            PlayerRole = PlayerRoleOnTable.Normal;
        }

        public void AddCard(Card card)
        {
            if (PlayerMessagesNotifier != null)
                PlayerMessagesNotifier("Player - Add Card");
            try
            {
                PlayerHand.Cards.Add(card);             //add card to player's hand
            }
            catch(Exception e)
            {
                if (PlayerMessagesNotifier != null)
                    PlayerMessagesNotifier(e.Message);
                throw new Exception("Could not add card,Player Hand is not inisialized");
            }
        }

        public void Fold()
        {
            PlayerState = PlayerAction.Fold;
            if (PlayerMessagesNotifier != null)
                PlayerMessagesNotifier(UserName +" Fold");
            if (PlayerFoldedNotifier != null)
                PlayerFoldedNotifier(ID);
        }

        public void Bet(int betAmount)
        {
            if (betAmount > 0)
            {
                _moneyToPlay -= betAmount;
                CurrentBet = betAmount;
                TotalBet += CurrentBet;
             //   UpdateMoneyInDb();
                PlayerState = PlayerAction.Call;;
            }
            else
                PlayerState = PlayerAction.Check;

            if (PlayerBetNotifier != null)
                PlayerBetNotifier(ID,CurrentBet);
        }

        public void Raise(int betAmount)
        {
            PlayerState = PlayerAction.Raise;;
            _moneyToPlay -= (betAmount - CurrentBet);
            //UpdateMoneyInDb();
            CurrentBet = betAmount;
            TotalBet += CurrentBet;
            if (PlayerBetNotifier != null)
                PlayerBetNotifier(ID,CurrentBet);
        }

        public PlayerRoleOnTable PlayerSateOnTable
        {
            get { return PlayerRole; }
        }

        public void SetPlayerRole(PlayerRoleOnTable role,int currentBlind)
        {
            PlayerRole = role;
            _moneyToPlay -= currentBlind;
            CurrentBet = currentBlind;
            TotalBet += CurrentBet;
            UpdateMoneyInDb();
        }

        public void UpdateMoneyInDb()
        {
            DBServicesHandler.DbHandler.UpdateMoney(ID, TotalMoney + _moneyToPlay);
        }

        public override string ToString()
        {
            return "[Player ID : " + ID + " user name : " + UserName + " Amount : " + TotalMoney + "]";
        }

        public void ClearHand()
        {
            PlayerHand.Cards.Clear();
        }

        public int MoneyToPlay
        {
            get { return _moneyToPlay; }
            set
            {
                _moneyToPlay = value;
                if (!_reduceAmountWhenJoinTable)
                {
                    _reduceAmountWhenJoinTable = true;
                    TotalMoney -= _moneyToPlay;
                }
            }
        }

        /// <summary>
        /// function finds Player Best Hand Per Round
        /// </summary>
        /// <param name="allCards"></param>
        public void FindBestHand(IEnumerable<Card> allCards)
        {
            PossibleHands = Hand.GetAllPossibleHands(allCards);
            BestHand = Hand.GetBestHand(PossibleHands);
        }
    }
}
