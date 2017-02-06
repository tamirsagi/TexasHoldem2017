using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Client
{


    /// <summary>
    /// Interaction logic for UserControlUserDetails.xaml
    /// </summary>    
    public partial class UserControlUserDetails : UserControl
    {
        private string ammout = "Ammount:";
        private string pleaseLogin = "Please login...";

        public UserControlUserDetails(string playerName, string playerTotalMoney)
        {
            InitializeComponent();

            PlayerName.Content = playerName;
            Ammount.Content = ammout;
            PlayerTotalMony.Content = playerTotalMoney;
        }
        public UserControlUserDetails()
        {
            InitializeComponent();
            unSetDetailes();
            PlayerName.Content = pleaseLogin;

        }
        public void unSetDetailes()
        {
            PlayerName.Content = pleaseLogin;
            PlayerTotalMony.Content = "";
            Ammount.Content = "";
        }
        public void SetDetailes(string playerName, string playerTotalMoney)
        {
            PlayerName.Content = playerName;
            Ammount.Content = ammout;
            PlayerTotalMony.Content = playerTotalMoney;
        }
    } 

}
