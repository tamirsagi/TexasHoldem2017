using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Game_Server
{
    public interface IGameServerServicesCallbacks
    {
        /// Notifies the client that a player has joined the table
        /// <param name="playerName">Name of the player</param>
        /// /// <param name="amount">user initial amount</param>
        /// /// /// <param name="positionInTable">position In Table</param>
        [OperationContract(IsOneWay = true)]
        void NotifyPlayerJoinedTable(PlayersInTable player);

        /// Notifies the client that blinds have changed
        /// <param name="smallBlind">new Small blind </param>
        /// /// <param name="bigBlind">new big blind </param>
        [OperationContract(IsOneWay = true)]
        void NotifyBlindsChanged(int smallBlind, int bigBlind);

        /// Notifies the client that blinds have changed
        /// <param name="smallBlindId">playerID who is the small Blind in current round</param>
        /// <param name="bigBlindId">playerID who is the big Blind in current round </param>
        [OperationContract(IsOneWay = true)]
        void NotifyPlayersRules(int smallBlindId,int bigBlindId);

        /// <summary>
        /// Notifies the client that a player has left the table
        /// </summary>
        /// <param name="playerName">Player's Name</param>
        /// <param name="position">position in table</param>
        /// <param name="wasKicked">indicates whether user was kicked or not</param>
        [OperationContract(IsOneWay = true)]
        void NotifyPlayerLeftTable(string playerName,int position,bool wasKicked);

        /// <summary>
        /// Notifies the client about next player
        /// </summary>
        /// <param name="playerToPlay">Player's ser name</param>
        /// <param name="minAmountToBet">min Amount To Bet</param>
        /// /// <param name="currentPlayerBalance">eace player in table will get his updated balance</param>
        /// /// <param name="pot">current</param>
        [OperationContract(IsOneWay = true)]
        void NotifyNextPlayerToPlay(string playerToPlay,int minAmountToBet,int currentPlayerBalance,int pot);

        /// <summary>
        /// Notifies players that a player fold
        /// </summary>
        /// <param name="userName">userName</param>
        [OperationContract(IsOneWay = true)]
        void NotifyPlayerFold(string userName);

        /// <summary>
        /// send Card to Player
        /// </summary>
        /// <param name="card">spesific card</param>
        /// <param name="cardToPlayer">Reveal Card on Table or Hand</param>
        /// /// <param name="position">player position in table</param>
        [OperationContract(IsOneWay = true)]
        void SendCardToPlayer(Card card, Card.CardToPlayer cardToPlayer,int position = -1);

        /// <summary>
        /// update the messages in Clients' Chat
        /// </summary>
        /// <param name="msg">msg to send to other clients</param>
        [OperationContract(IsOneWay = true)]
        void UpdateMessageInChat(string msg);

        /// <summary>
        /// update player of being kicked
        /// </summary>
        /// <param name="msg">msg to send to other clients</param>
        [OperationContract(IsOneWay = true)]
        void NotifyPlayerKicked();

        /// <summary>
        /// Notify Player Action
        /// </summary>
        /// <param name="userName">userName</param>
        /// <param name="action">player's action(Check,Call,Fold,Raise,AllIn)</param>
        ///<param name="amount">amount</param>
        [OperationContract(IsOneWay = true)]
        void NotifyPlayerAction(string userName,Player.PlayerAction action,int amount);

        /// <summary>
        /// reveal players cards
        /// </summary>
        /// <param name="player">player in Table clients</param>
        [OperationContract(IsOneWay = true)]
        void RevealCardsOnTable(PlayersInTable player);

        /// <summary>
        /// Notify winners per round
        /// </summary>
        /// <param name="winners">winners in Table per round</param>
        [OperationContract(IsOneWay = true)]
        void NotifyWinners(List<PlayersInTable> winners );
    }
}
