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
    /// Interaction logic for UserControlChooseTable.xaml
    /// </summary>
    public partial class UserControlChooseTable : UserControl
    {
        public static List<Table> tablesList;
        private UserControlCreateTable userControlCreateTable;
        private Table selectedTable;
        //public UserControlUserDetails userControlUserDetails;
        public UserControlChooseMinAmount userControlChooseMinAmount;
        
        public IClientListener clientListener;

        public event GetTablesHandler GetTables_Handler;
        public delegate List<Table> GetTablesHandler(UserControlChooseTable userControlChooseTable);



        public UserControlChooseTable(/*UserControlUserDetails userControlUserDetails*/)
        {
            InitializeComponent();
            //this.userControlUserDetails = userControlUserDetails;
            clientListener = new ClientListener();
            clientListener.subscribe_loadTables_chooseTableScreen(this);
            tablesList = new List<Table>();


            getTables();

        }

        private void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {            
            var grid = sender as DataGrid;
            grid.ItemsSource = tablesList;            
        }

        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                foreach (Table table in e.AddedItems)
                {
                    selectedTable = table;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("No table was selected");
            }
            
        }

        private void RowDoubleClick(object sender, RoutedEventArgs e)
        {            
            joinTable();
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back)
            {
                Back_Exit();
            }
            if (e.Key == Key.Enter)
            {
                joinTable();
            }
        }

        private void JoinTable_Click(object sender, RoutedEventArgs e)
        {
            joinTable();            
        }

        private void joinTable()
        {
            if (selectedTable != null && checkTableAccessibility(selectedTable))
            {
                userControlChooseMinAmount = new UserControlChooseMinAmount(selectedTable);
                mainGrid.Children.Add(userControlChooseMinAmount);
                getTables();
            }
        }

        private bool checkTableAccessibility(Table table)
        {
            if(table != null)
                return table.MinAmmount <= User.Money;
            return false;
        }

        private void CreateTable_Click(object sender, RoutedEventArgs e)
        {
            userControlCreateTable = new UserControlCreateTable();

            mainGrid.Children.Add(userControlCreateTable);
            
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Back_Exit();
        }

        private void Back_Exit()
        {
            ((Panel)this.Parent).Children.Remove(this);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            getTables();                
        }

        private void getTables()
        {
            tablesList = new List<Table>();

            if (GetTables_Handler != null)
            {
                try
                {
                    tablesList = GetTables_Handler(this);
                    TablesGrid.ItemsSource = tablesList;
                    TablesGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            }    
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utils.StateMachine = StateMachine.ChooseTable;
            
        }

    }
}
