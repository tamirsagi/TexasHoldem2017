using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for UserControlHelp.xaml
    /// </summary>
    public partial class UserControlHelp : UserControl
    {
        public string HelpString { get; set; }
        

        public UserControlHelp()
        {
            InitializeComponent();
            HelpString = "To start playing you need first to login and then sit at a table.\nyou may join an open table or create your own.";

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ((Panel)this.Parent).Children.Remove(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utils.StateMachine = StateMachine.Help;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }


    }
}
