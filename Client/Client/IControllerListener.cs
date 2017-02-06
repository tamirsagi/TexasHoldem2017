using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public interface IControllerListener
    {
        void subscribe_set_shared_card(Controller controller, Card card, Table tabl, int toWhoe, int location);
        void notify_set_shared_card(Controller controller, Card card, Table table, int toWho, int location);

        void subscribe_receive_chat_text(Controller controller, Table table, string toDisplay);
        void notify_receive_chat_text(Controller controller, Table table, string toDisplay);

        void subscribe_receive_pot(Controller controller, Table table, int pot);
        void notify_receive_pot(Controller controller, Table table, int pot);

        void subscribe_receive_current_player(Controller controller, Table table, string currentPlayer, int minAmountToBet, int currentPlayerBalance);
        void notify_receive_current_player(Controller controller, Table table, string currentPlayer, int minAmountToBet, int currentPlayerBalance);
        
        void subscribe_received_game_events(Controller controller, Table table, string toDisplay);
        void notify_received_game_events(Controller controller, Table table, string toDisplay);

        void subscribe_notify_player_joined_table(Controller controller, Table table, Player player);
        void notify_notify_player_joined_table(Controller controller, Table table, Player player);

        void subscribe_notify_players_roles(Controller controller, Table table, int smallBlindID, int bigBlindID);
        void notify_notify_players_roles(Controller controller, Table table, int smallBlindID, int bigBlindID);

        void subscribe_notify_delete_player_from_table(Controller controller, Table table, string playerName);
        void notify_notify_delete_player_from_table(Controller controller, Table table, string playerName);

        void subscribe_notify_to_close_table_after_kick(Controller controller, Table table);
        void notify_notify_to_close_table_after_kick(Controller controller, Table table);

        void subscribe_set_all_players_cards(Controller controller, Table table, Player player);
        void notify_set_all_players_cards(Controller controller, Table table, Player player);

        void subscribe_notify_finished_round(Controller controller, Table table);
        void notify_subscribe_notify_finished_round(Controller controller, Table table);

        void subscribe_notify_player_has_folded(Controller controller, Table table, string playerName);
        void notify_notify_player_has_folded(Controller controller, Table table, string playerName);

        void subscribe_notify_player_is_admin(Controller controller, Table table, string playerName);
        void notify_notify_player_is_admin(Controller controller, Table table, string playerName);
    }
}
