using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Documents;
using System.Windows.Forms;


namespace Client
{
    public class ClientListener : IClientListener
    {

        IController controller;
        
        public ClientListener()
        {
            controller = Controller.Instance();
        }

        public void subscribe_login_loginScreen(UserControlLogin userControlLogin, string username, string password, int userID)
        {
            userControlLogin.loginButtonPressed_Handler += new UserControlLogin.LoginButtonHandler(notify_login_loginScreen);
        }

        public void notify_login_loginScreen(UserControlLogin userControlLogin, string userName, string password, int userID)
        {            
            controller.notify_login_loginScreen_was_pressed(userControlLogin, userName, password,userID);            
        }

        public void subscribe_loadTables_chooseTableScreen(UserControlChooseTable userControlChooseTable)
        {
            userControlChooseTable.GetTables_Handler += new UserControlChooseTable.GetTablesHandler(notify_loadTables_chooseTableScreen);
        }

        public List<Table> notify_loadTables_chooseTableScreen(UserControlChooseTable userControlChooseTable)
        {
            return controller.getTablesFromServer();
        }

        public void subscribe_create_tables(UserControlCreateTable userControlCreateTable, string tableName, int smallBlind, int bigBlind, int minAmmount, int adminID, int buyIn)
        {
            userControlCreateTable.CreateTable_Handler += new UserControlCreateTable.CreateTableHandler(notify_create_tables);
        }

        public int notify_create_tables(UserControlCreateTable userControlCreateTable, string tableName, int smallBlind, int bigBlind, int minAmmount, int adminID, int buyIn)
        {
            return controller.createTable(tableName, smallBlind, bigBlind, minAmmount, adminID, buyIn);
            
        }

        public void subscribe_place_bet(UserControlBets UserControlBets, int tableID, int userID, int sumToBet, int currentBet)
        {
            UserControlBets.PlaceBet_Handler += new UserControlBets.PlaceBetHandler(notify_place_bet);
        }

        public void notify_place_bet(UserControlBets UserControlBets, int tableID, int userID, int sumToBet, int currentBet)
        {
            controller.placeBet( tableID, userID, sumToBet, currentBet);
        }


        //public void subscribe_get_current_bet(UserControlTable userControlTable, int tableName)
        //{
        //    userControlTable.getCurrentBet_Handler += new UserControlTable.getCurrentBet(notify_get_current_bet);
        //}

        //public int notify_get_current_bet(UserControlTable userControlTable, int tableID)
        //{
        //    return controller.getCurrentBet(tableID);
        //}


        public void subscribe_notify_player_folded(UserControlTable userControlTable, int userID)
        {
            userControlTable.notifyPlayerFolded_Handler += new UserControlTable.notifyPlayerFolded(notify_notify_player_folded);
        }

        public void notify_notify_player_folded(UserControlTable userControlTable, int userID)
        {
            controller.fold(userID);
        }


        public void subscribe_notify_player_left_table(UserControlTable userControlTable, int userID, int tableID, int userMoney)
        {
            userControlTable.notifyPlayerLeftTable_Handler += new UserControlTable.notifyPlayerLeftTable(notify_notify_player_left_table);
        }

        public void notify_notify_player_left_table(UserControlTable userControlTable, int userID, int tableID, int userMoney)
        {
            controller.notify_leave_table(userID, tableID, userMoney);
        }




        public void subscribe_request_join_table(UserControlChooseMinAmount userControlChooseMinAmount, int tableID, int userID, int amountToEnter)
        {
            userControlChooseMinAmount.RequestJoinTable_Handler += new UserControlChooseMinAmount.RequestJoinTable(notify_request_join_table);
        }

        public List<Player> notify_request_join_table(UserControlChooseMinAmount userControlChooseMinAmount, int tableID, int userID, int amountToEnter)
        {
            return controller.RequestJoinTable(userID, tableID, amountToEnter);

        }


        //public void subscribe_request_join_table_created(UserControlCreateTable userControlCreateTable, int tableID, int userID)
        //{
        //    userControlCreateTable.RequestJoinTableCreated_Handler += new UserControlCreateTable.RequestJoinTableCreated(notify_request_join_table_created);
        //}

        //public bool notify_request_join_table_created(UserControlCreateTable userControlCreateTable, int tableID, int userID)
        //{
        //    return false;//controller.RequestJoinTable(userID, tableID);
        //}


        public void subscribe_kick_player(UserControlPlayersToKick userControlPlayersToKick, int playerID, int tableID)
        {
            userControlPlayersToKick.KickPlayer_Handler += new UserControlPlayersToKick.KickPlayer(notify_kick_player);
        }

        public void notify_kick_player(UserControlPlayersToKick userControlPlayersToKick, int playerID, int tableID)
        {
            controller.KickPlayer(playerID, tableID);
        }


        public void subscribe_send_chat_talk(Table table, string chat)
        {
            table.SendChatTalk_Handler += new Table.SendChatTalk(notify_send_chat_talk);
        }

        public void notify_send_chat_talk(Table table ,string chat)
        {
            controller.SendChatText(table.TableID, chat);
        }


        public void subscribe_send_card_to_user_control_table(Table table, Card card, int toWho, int location)
        {
            table.SendCardToUserControlTable_Handler += new Table.SendCardToUserControlTable(notify_send_card_to_user_control_table);
        }

        public void notify_send_card_to_user_control_table(Table table, Card card, int toWho, int location)
        {
            if (toWho == 1)
                table.UserControlTable.getFlopFromTable(card);
            else if (toWho == 0)
            {
                table.UserControlTable.getCardsToUserFromTable(card, location);                
            }
        }



        public void subscribe_notify_player_ready(UserControlChooseMinAmount userControlChooseMinAmount, int userID, int tableID)
        {
            userControlChooseMinAmount.NotifyPlayerReady_Handler += new UserControlChooseMinAmount.NotifyPlayerReady(notify_notify_player_ready);
        }

        public void notify_notify_player_ready(UserControlChooseMinAmount userControlChooseMinAmount, int userID, int tableID)
        {
            controller.notifyPlayerIsReady(userID, tableID);
        }


        public void subscribe_is_player_closed_game(MainWindow mainWindow)
        {
            mainWindow.IsPlayerClosedGame_Handler += new MainWindow.IsPlayerClosedGame(notify_is_player_closed_game);
        }

        public void notify_is_player_closed_game(MainWindow mainWindow)
        {
            //Thread t = new Thread(()=> controller.checkConnection());
            //t.Start();
        }



        public void subscribe_check_handler(UserControlTable userControlTable, int tableID, int userID)
        {
            userControlTable.CheckHandler_Handler += new UserControlTable.CheckHandler(notify_check_handler);
        }

        public void notify_check_handler(UserControlTable userControlTable, int tableID, int userID)
        {
            controller.check(tableID, userID);
        }


        public void subscribe_notify_ready_after_player_folded(Table table, int userID, int tableID)
        {
            table.NotifyReadyAfterPlayerFolded_Handler += new Table.NotifyReadyAfterPlayerFolded(notify_notify_ready_after_player_folded);
        }

        public void notify_notify_ready_after_player_folded(Table table, int userID, int tableID)
        {
            controller.notifyPlayerIsReady(userID, tableID);
            table.setAllPlayersToActive();
        }


        public void subscribe_get_current_balance_from_server(UserControlTable userControlTable, int userID)
        {
            userControlTable.GetCurrentBalanceFromServer_Handler += new UserControlTable.GetCurrentBalanceFromServer(notify_get_current_balance_from_server);
        }

        public int notify_get_current_balance_from_server(UserControlTable userControlTable, int userID)
        {
            int temp = controller.getCurrentBalance(userID);
            User.Money = temp;
            return temp;
        }

    }
}
