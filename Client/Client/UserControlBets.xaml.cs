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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControlBets : UserControl
    {
        private int sum = 0;
        private int minBet = 100;
        private int maxBet = User.BuyInMoney;
        private readonly int chip100 = 100;
        private readonly int chip50 = 50;
        private readonly int chip20 = 20;
        private readonly int chip10 = 10;
        private readonly int chip5 = 5;
        public int Raise { get; set; }
        public int TableID { get; set; }
        public int CurrentBet { get; set; }


        public IClientListener clientListener;

        public delegate void PlaceBetHandler(UserControlBets UserControlBets, int tablID, int userID, int sumToBet, int currentBet);
        public event PlaceBetHandler PlaceBet_Handler;

        public UserControlBets(int raise, int tableID, int currentBet)
        {
            InitializeComponent();
            Raise = raise;
            clientListener = new ClientListener();
            clientListener.subscribe_place_bet(this, -1, User.UserID, -1, -1);            
            sumToBet.Content = sum;
            TableID = tableID;
            CurrentBet = currentBet;
        }

        private void Plus100_Click(object sender, RoutedEventArgs e)
        {
            sum += chip100;
            if (sum >= maxBet)
            {
                sum = maxBet;
            }
            sumToBet.Content = sum;
        }

        private void Plus50_Click(object sender, RoutedEventArgs e)
        {
            sum += chip50;
            if (sum >= maxBet)
            {
                sum = maxBet;
            }
            sumToBet.Content = sum;
        }

        private void Plus20_Click(object sender, RoutedEventArgs e)
        {
            sum += chip20;
            if (sum >= maxBet)
            {
                sum = maxBet;
            }
            sumToBet.Content = sum;
        }

        private void Plus10_Click(object sender, RoutedEventArgs e)
        {
            sum += chip10;
            if (sum >= maxBet)
            {
                sum = maxBet;
            }
            sumToBet.Content = sum;
        }

        private void Plus5_Click(object sender, RoutedEventArgs e)
        {
            sum += chip5;
            if (sum >= maxBet)
            {
                sum = maxBet;
            }
            sumToBet.Content = sum;
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            sum -= minBet;
            if (sum <= 0)
            {
                sum = 0;                
            }
            sumToBet.Content = sum;
        }


        private void PlaceBet_Click(object sender, RoutedEventArgs e)
        {
            //send sum to server with delegate/event
            Bet(sum);
            ((Panel)this.Parent).Children.Remove(this);
        }

        private void AllIn_Click(object sender, RoutedEventArgs e)
        {
            sum = maxBet;
            sumToBet.Content = sum;
        }

        private void ClearBet_Click(object sender, RoutedEventArgs e)
        {
            sum = 0;
            sumToBet.Content = sum;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ((Panel)this.Parent).Children.Remove(this);
        }

        public void Bet(int sum)
        {
            if (PlaceBet_Handler != null)
            {
                PlaceBet_Handler(this, TableID, User.UserID, sum, CurrentBet);
                User.MoneyToPlay -= sum;
            }            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utils.StateMachine = StateMachine.Bets;
        }
    }
}
