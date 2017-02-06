using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace Client
{
    public interface IController
    {
        void notify_login_loginScreen_was_pressed(Object userControlLogin, string userName, string password, int userID);
        List<Table> getTablesFromServer();
        int createTable(string tableName, int smallBlind, int bigBlind, int minAmmount, int adminID, int buyIn);
        void placeBet(int tableID, int userID, int sumToBet, int currentBet);
        void check(int tableID, int userID);
        void fold(int userID);
        //int getCurrentBet(int tableName);
        void notify_leave_table(int userID, int TableID, int userMoney);
        List<Player> RequestJoinTable(int userID, int tableID, int amountToEnter);
        void KickPlayer(int playerID, int tableID);
        void SendChatText(int TableID, string chat);
        void notifyPlayerIsReady(int userID, int tableID);
        int getCurrentBalance(int userID);
        
    }
}
