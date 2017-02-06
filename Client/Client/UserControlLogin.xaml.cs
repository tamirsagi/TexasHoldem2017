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
    /// Interaction logic for UserControlLogin.xaml
    /// </summary>
    public partial class UserControlLogin : UserControl
    {
        public User user;
        public UserControlUserDetails userControlUserDetails;   
     
        public event LoginButtonHandler loginButtonPressed_Handler;
        public delegate void LoginButtonHandler(UserControlLogin m, string userName, string password, int userID);

        public IClientListener clientListener;
        public UserControlLoading userControlLoading;
        public SplashScreen splashScreen;
        private MainWindow mainWindow;

        

        public UserControlLogin(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            clientListener = new ClientListener();
            clientListener.subscribe_login_loginScreen(this, "","",-1);
        }

        public UserControlUserDetails getUserControlUserDetails()
        {
            return userControlUserDetails;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Login();            
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Back();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utils.StateMachine = StateMachine.Login;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
            if (e.Key == Key.Back)
            {
                Back();
            }
        }

        private void Login()
        {
            string password = Password.Password;
            string userName = UserName.Text;

            user = User.Instance(userName, password);

            if (loginButtonPressed_Handler != null)
            {
                try
                {
                    loginButtonPressed_Handler(this, userName, password, User.UserID);
                    mainWindow.enableDisableLogOut(true);
                    mainWindow.enableDisableChooseTable(true);
                    mainWindow.enableDisableLogin(false);                
                    mainWindow.userControlUserDetails.SetDetailes(User.UserName, User.Money.ToString());
                    ((Panel)this.Parent).Children.Remove(this);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Back()
        {
            ((Panel)this.Parent).Children.Remove(this);
        }

        public void removeDetails()
        {
            ((Panel)this.Parent).Children.Remove(userControlUserDetails);
        }
    }
}
