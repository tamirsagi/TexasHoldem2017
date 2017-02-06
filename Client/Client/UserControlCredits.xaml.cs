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
    /// Interaction logic for UserControlCredits.xaml
    /// </summary>
    public partial class UserControlCredits : UserControl
    {
        

        public UserControlCredits()
        {
            InitializeComponent();
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            ((Panel)this.Parent).Children.Remove(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utils.StateMachine = StateMachine.Credits;
        }
    }
}
