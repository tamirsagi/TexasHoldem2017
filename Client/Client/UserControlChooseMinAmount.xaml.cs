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
    /// Interaction logic for UserControlChooseMinAmount.xaml
    /// </summary>
    public partial class UserControlChooseMinAmount : UserControl
    {
        public IClientListener clientListener;
        private UserControlTable userControlTable;
        public Table Table { get; set; }
        private string invalid = "Is invalid, please insert a valid number";
        public delegate List<Player> RequestJoinTable(UserControlChooseMinAmount userControlChooseMinAmount, int TableID, int userID, int amountToEnter);
        public event RequestJoinTable RequestJoinTable_Handler;

        public delegate void NotifyPlayerReady(UserControlChooseMinAmount userControlChooseMinAmount, int userID, int tableID);
        public event NotifyPlayerReady NotifyPlayerReady_Handler;

        public UserControlChooseMinAmount(Table table)
        {
            InitializeComponent();
            clientListener = new ClientListener();
            Table = table;
            clientListener.subscribe_request_join_table(this, -1, -1, -1);
            clientListener.subscribe_notify_player_ready(this, User.UserID, Table.TableID);
        }

        private void NotifyServerPlayerReady()
        {
            if (NotifyPlayerReady_Handler != null)
                NotifyPlayerReady_Handler(this, User.UserID, Table.TableID);
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            joinTable();
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                joinTable();
            }
        }

        private void joinTable()
        {
            int buyIn;
            bool result = false;
            int location = -1;
            List<Player> playersList = new List<Player>();
            try
            {
                result = Int32.TryParse(MinAmountText.Text, out buyIn);
                if (!result || buyIn <= 0 || User.Money < buyIn || Table.MinAmmount > buyIn)
                {
                    throw new ArgumentException("\"" + MinAmountText.Text + "\"" + invalid);
                }
                if (RequestJoinTable_Handler != null)
                {
                    playersList = RequestJoinTable_Handler(this, Table.TableID, User.UserID, buyIn);
                    Table.addPlayersFromServer(playersList);

                    User.TableID = Table.TableID;
                    User.Table = Table;
                    User.BuyInMoney = buyIn;
                    User.MoneyToPlay = User.BuyInMoney;
                    User.player.cards = new List<Card>();
                    User.IsAdmin = false;

                    //if (Table.Players.Count <= 1)
                    //{
                    //    Table.GameStatus = enumGameStatus.Finished;
                    //    User.player.IsPlaying = true;
                    //}
                    //else
                    //{
                    //    Table.GameStatus = enumGameStatus.Started;
                    //    User.player.IsPlaying = false;
                    //}

                    userControlTable = new UserControlTable(Table, location);
                    ((Panel)this.Parent).Children.Add(userControlTable);
                    ((Panel)this.Parent).Children.Remove(this);
                    //ready
                    NotifyServerPlayerReady();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ((Panel)this.Parent).Children.Remove(this);
        }


    }
}
