using System;
using System.Collections.Generic;
using System.ComponentModel;
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


namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserControlLogin login;
        private UserControlTable table;
        private UserControlChooseTable chooseTable;
        private UserControlHelp userControlHelp;
        private UserControlCredits userControlCredits;
        private UserControlEnterServerIP userControlEnterServerIP;

        public List<Button> Buttons;
        public UserControlUserDetails userControlUserDetails;
        public User user;
        public IClientListener clientListener;
        

        public delegate void IsPlayerClosedGame(MainWindow mainWindow);
        public event IsPlayerClosedGame IsPlayerClosedGame_Handler;

        public MainWindow()
        {
            InitializeComponent();
            Buttons = new List<Button>();
            initButtons();

            try
            {
                userControlEnterServerIP = new UserControlEnterServerIP(this);
                enableDisableAllButtons(false);
                mainGrid.Children.Add(userControlEnterServerIP);
                if (IsPlayerClosedGame_Handler != null)
                    IsPlayerClosedGame_Handler(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void initButtons()
        {
            Buttons.Add(LoginButton);
            Buttons.Add(LogOutButton);
            Buttons.Add(ChooseTable);
            Buttons.Add(Help);
            Buttons.Add(Credits);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            userControlUserDetails = new UserControlUserDetails();
            mainGrid.Children.Add(userControlUserDetails);
            Utils.StateMachine = StateMachine.MainWindow;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            login = new UserControlLogin(this);         
            mainGrid.Children.Add(login);
            
        }

        private void CooseTable_Click(object sender, RoutedEventArgs e)
        {
            chooseTable = new UserControlChooseTable(/*userControlUserDetails*/);
            mainGrid.Children.Add(chooseTable);
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            userControlHelp = new UserControlHelp();
            mainGrid.Children.Add(userControlHelp);
        }

        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            userControlCredits = new UserControlCredits();
            mainGrid.Children.Add(userControlCredits);
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            enableDisableChooseTable(false);
            enableDisableLogOut(false);
            enableDisableLogin(true);
            User.unsetUser();            
            userControlUserDetails.unSetDetailes();
        }

        public void enableDisableButton(Button button, bool state)
        {
            if (!state)
            {
                button.Opacity = 0.25;
                button.IsEnabled = state;
            }
            else
            {
                button.Opacity = 1;
                button.IsEnabled = state;
            }
        }
        public void enableDisableButtons(Button button, bool state)
        {
            if (!state)
            {
                button.Opacity = 0;
                button.IsEnabled = state;
            }
            else
            {
                button.Opacity = 1;
                button.IsEnabled = state;
            }
        }
        public void enableDisableLogOut(bool state)
        {
            enableDisableButton(LogOutButton, state);
        }

        public void enableDisableChooseTable(bool state)
        {
            enableDisableButton(ChooseTable, state);
        }


        public void enableDisableLogin(bool state)
        {
            enableDisableButton(LoginButton, state);
        }

        public void enableDisableAllButtons(bool state)
        {
            foreach (var button in Buttons)
            {
                enableDisableButtons(button, state);    
            }
            
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Utils.IsPlayerClosedWindow = true;
           
        }

    }
}
