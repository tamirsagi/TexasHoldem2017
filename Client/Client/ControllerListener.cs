using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public class ControllerListener : IControllerListener
    {
        IController controller;

        public ControllerListener()
        {
            controller = Controller.Instance();
        }

        public void subscribe_set_shared_card(Controller controller, Card card, Table table, int toWho, int location)
        {
            controller.SetSharedCard_Handler += new Controller.SetSharedCard(notify_set_shared_card);
        }

        public void notify_set_shared_card(Controller controller, Card card, Table table, int toWho, int location)
        {

            if (toWho == 0)
            {
                table.setPlayerCards(card, location);
                table.UserControlTable.setDeckToOtherPlayers();
            }
            else if (toWho == 1)
            {
                table.setFlop(card);
            }


        }


        public void subscribe_receive_chat_text(Controller controller, Table table, string toDisplay)
        {
            controller.ReceiveChatText_Handler += new Controller.ReceiveChatText(notify_receive_chat_text);
        }

        public void notify_receive_chat_text(Controller controller, Table table, string toDisplay)
        {
            table.ReceiveChatText(toDisplay);
        }


        public void subscribe_receive_pot(Controller controller, Table table, int pot)
        {
            controller.ReceivePot_Handler += new Controller.ReceivePot(notify_receive_pot);
        }

        public void notify_receive_pot(Controller controller, Table table, int pot)
        {
            table.ReceivePotChange(pot);
        }


        public void subscribe_receive_current_player(Controller controller, Table table, string currentPlayer, int minAmountToBet, int currentPlayerBalance)
        {
            controller.ReceiveCurrentPlayer_Handler += new Controller.ReceiveCurrentPlayer(notify_receive_current_player);
        }

        public void notify_receive_current_player(Controller controller, Table table, string currentPlayer, int minAmountToBet, int currentPlayerBalance)
        {
            table.ReceiveCurrentPlayer(currentPlayer, minAmountToBet);
            table.recivePlayerCurrentBalance(currentPlayer, currentPlayerBalance);
        }


        public void subscribe_received_game_events(Controller controller, Table table, string toDisplay)
        {
            controller.ReceivedGameEvents_Handler += new Controller.ReceivedGameEvents(notify_received_game_events);
        }

        public void notify_received_game_events(Controller controller, Table table, string toDisplay)
        {

            table.ReceivedGameEvents(toDisplay);
        }

        public void subscribe_notify_player_joined_table(Controller controller, Table table, Player player)
        {
            controller.NotifyPlayerJoinedTable_Handler += new Controller.NotifyPlayerHasJoinedTable(notify_notify_player_joined_table);
        }

        public void notify_notify_player_joined_table(Controller controller, Table table, Player player)
        {
            table.addPlayer(player);
            table.UserControlTable.setPlayerList();
        }


        public void subscribe_notify_players_roles(Controller controller, Table table, int smallBlindID, int bigBlindID)
        {
            controller.NotifyPlayersRoles_Handler += new Controller.NotifyPlayersRoles(notify_notify_players_roles);
        }

        public void notify_notify_players_roles(Controller controller, Table table, int smallBlindID, int bigBlindID)
        {
            table.setPlayersRols(smallBlindID, bigBlindID);
            table.UserControlTable.setRoleToPlayer();
        }


        public void subscribe_notify_delete_player_from_table(Controller controller, Table table, string playerName)
        {
            controller.NotifyDeletePlayerFromTable_Handler += new Controller.NotifyDeletePlayerFromTable(notify_notify_delete_player_from_table);
        }

        public void notify_notify_delete_player_from_table(Controller controller, Table table, string playerName)
        {
            enumPosition p =  table.deletePlayer(playerName);
            table.UserControlTable.deletePlayerFromTable(p);
        }

        public void subscribe_notify_to_close_table_after_kick(Controller controller, Table table)
        {
            controller.NotifyToCloseTableAfterKick_Handler += new Controller.NotifyToCloseTableAfterKick(notify_notify_to_close_table_after_kick);
        }

        public void notify_notify_to_close_table_after_kick(Controller controller, Table table)
        {
            table.UserControlTable.closeTable();
        }


        public void subscribe_set_all_players_cards(Controller controller, Table table, Player player)
        {
            controller.SetAllPlayersCards_Handler += new Controller.SetAllPlayersCards(notify_set_all_players_cards);
        }

        public void notify_set_all_players_cards(Controller controller, Table table, Player player)
        {
            if(player.UserName != User.UserName)
            {
                foreach (var playerCard in player.cards)
                {
                    table.UserControlTable.getCardsToAllPlayersFromTable(playerCard, (int)player.Position, player);
                }                   
	        }           
        }


        public void subscribe_notify_finished_round(Controller controller, Table table)
        {
            controller.NotifyFinishedRound_Handler += new Controller.NotifyFinishedRound(notify_subscribe_notify_finished_round);
        }

        public void notify_subscribe_notify_finished_round(Controller controller, Table table)
        {
            table.clearTableAfterRound();
        }


        public void subscribe_notify_player_has_folded(Controller controller, Table table, string playerName)
        {
            controller.NotifyPlayerHasFolded_Handler += new Controller.NotifyPlayerHasFolded(notify_notify_player_has_folded);
        }

        public void notify_notify_player_has_folded(Controller controller, Table table, string playerName)
        {
            table.playerHasFolded(playerName);
        }


        public void subscribe_notify_player_is_admin(Controller controller, Table table, string playerName)
        {
            controller.NotifyPlayerIsAdmin_Handler += new Controller.NotifyPlayerIsAdmin(notify_notify_player_is_admin);
        }

        public void notify_notify_player_is_admin(Controller controller, Table table, string playerName)
        {
            User.IsAdmin = true;
            table.UserControlTable.setKickButton();
        }
    }
}
