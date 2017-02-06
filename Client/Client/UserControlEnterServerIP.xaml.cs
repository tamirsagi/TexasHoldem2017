using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for UserControlEnterServerIP.xaml
    /// </summary>
    public partial class UserControlEnterServerIP : UserControl
    {
        public IController controller;
        private MainWindow mainWindow;
        Regex ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
        private string errorMessage = "Expression does not match IP pattern or is not a valid IP";

        public UserControlEnterServerIP(MainWindow mainWindow)
        {
            InitializeComponent();
            
            
            this.mainWindow = mainWindow;
            ServerIPText.Focus();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            changeIP();
            
        }

        private void changeIP()
        {
            try
            {
                MatchCollection result = ip.Matches(ServerIPText.Text);
                Utils.setIP(result[0].ToString());
                controller = Controller.Instance(Utils.GameServerAddress);// TODO change to event + delegate
                mainWindow.enableDisableAllButtons(true);
                mainWindow.enableDisableChooseTable(false);
                mainWindow.enableDisableLogOut(false);
                ((Panel)this.Parent).Children.Remove(this);
                

            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(errorMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                changeIP();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utils.StateMachine = StateMachine.SetIP;
        }

    }
}
