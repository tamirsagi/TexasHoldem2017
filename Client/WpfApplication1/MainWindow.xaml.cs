using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += Button_Click_1;
            Button1.Focusable = false;
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = 10;
            da.Duration = new Duration(TimeSpan.FromSeconds(0.2));            
            TranslateTransform rt = new TranslateTransform();
            rectangle1.RenderTransform = rt;
            rt.BeginAnimation(TranslateTransform.XProperty, da);
            rt.BeginAnimation(TranslateTransform.YProperty, da);
        }
    }
}
