using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace Client
{
    public interface IClientListener
    {
        void subscribe_login_loginScreen(UserControlLogin userControlLogin, string userName, string password, int userID);
        void notify_login_loginScreen(UserControlLogin userControlLogin, string userName, string password, int userID);

        void subscribe_loadTables_chooseTableScreen(UserControlChooseTable userControlChooseTable);
        List<Table> notify_loadTables_chooseTableScreen(UserControlChooseTable userControlChooseTable);

        void subscribe_create_tables(UserControlCreateTable userControlCreateTable, string tableName, int smallBlind, int bigBlind, int minAmmount, int adminID, int buyIn);
        int notify_create_tables(UserControlCreateTable userControlCreateTable, string tableName, int smallBlind, int bigBlind, int minAmmount, int adminID, int buyIn);

        void subscribe_place_bet(UserControlBets UserControlBets, int tableID, int userID, int sumToBet, int currentBet);
        void notify_place_bet(UserControlBets UserControlBets, int tableID, int userID, int sumToBet, int currentBet);

        void subscribe_check_handler(UserControlTable userControlTable, int tableID, int userID);
        void notify_check_handler(UserControlTable userControlTable, int tableID, int userID);

        //void subscribe_get_current_bet(UserControlTable userControlTable, int tableID);
        //int notify_get_current_bet(UserControlTable userControlTable, int tableID);

        void subscribe_notify_player_folded(UserControlTable userControlTable, int userID);
        void notify_notify_player_folded(UserControlTable userControlTable, int userID);

        void subscribe_notify_player_left_table(UserControlTable userControlTable, int userID, int tableID, int userMoney);
        void notify_notify_player_left_table(UserControlTable userControlTable, int userID, int tableID, int userMoney);

        void subscribe_request_join_table(UserControlChooseMinAmount userControlChooseMinAmount, int tableID, int userID, int minAmount);
        List<Player> notify_request_join_table(UserControlChooseMinAmount userControlChooseMinAmount, int tableID, int userID, int minAmount);

        void subscribe_kick_player(UserControlPlayersToKick userControlPlayersToKick, int playerID, int tableID);
        void notify_kick_player(UserControlPlayersToKick userControlPlayersToKick, int playerID, int tableID);

        void subscribe_send_chat_talk(Table table, string chat);
        void notify_send_chat_talk(Table table, string chat);

        void subscribe_send_card_to_user_control_table(Table table, Card card, int toWho, int location);
        void notify_send_card_to_user_control_table(Table table, Card card, int toWho, int location);

        void subscribe_notify_player_ready(UserControlChooseMinAmount userControlChooseMinAmount, int userID, int tableID);
        void notify_notify_player_ready(UserControlChooseMinAmount userControlChooseMinAmount, int userID, int tableID);

        void subscribe_is_player_closed_game(MainWindow mainWindow);
        void notify_is_player_closed_game(MainWindow mainWindow);

        void subscribe_notify_ready_after_player_folded(Table table, int userID, int tableID);
        void notify_notify_ready_after_player_folded(Table table, int userID, int tableID);

        void subscribe_get_current_balance_from_server(UserControlTable userControlTable, int userID);
        int notify_get_current_balance_from_server(UserControlTable userControlTable, int userID);

    }
}
