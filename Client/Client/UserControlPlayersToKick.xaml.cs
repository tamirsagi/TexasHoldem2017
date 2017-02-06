using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for UserControlPlayersToKick.xaml
    /// </summary>
    public partial class UserControlPlayersToKick : UserControl
    {
        public IClientListener clientListener;

        public List<Player> PlayersList { get; set; }
        public Player playerToKick { get; set; }
        public int TableID { get; set; }

        public delegate void KickPlayer(UserControlPlayersToKick userControlPlayersToKick, int playerID, int tableID);
        public event KickPlayer KickPlayer_Handler;

        public UserControlPlayersToKick(List<Player> playersList, int tableID)
        {
            InitializeComponent();
            PlayersList = playersList;
            TableID = tableID;
            clientListener = new ClientListener();


            clientListener.subscribe_kick_player(this, -1, -1);
        }

        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                foreach (Player player in e.AddedItems)
                {
                    playerToKick = player;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("No player was selected");
            }
        }
        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = sender as DataGrid;
            grid.ItemsSource = PlayersList;
        }
        private void RowDoubleClick(object sender, RoutedEventArgs e)
        {
            Kick();
        }
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                Back_Exit();
            }
        }
        private void Kick()
        {
            if (KickPlayer_Handler != null)
                KickPlayer_Handler(this, playerToKick.PlayerID, TableID);

            Back_Exit();
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Back_Exit();
        }
        private void Back_Exit()
        {
            ((Panel)this.Parent).Children.Remove(this);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utils.StateMachine = StateMachine.Kick;

        }

        private void Kick_Click(object sender, RoutedEventArgs e)
        {
            Kick();
        }
    }
}
