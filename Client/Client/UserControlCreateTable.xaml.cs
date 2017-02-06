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
    /// Interaction logic for UserControlCreateTable.xaml
    /// </summary>
    public partial class UserControlCreateTable : UserControl
    {
        private string tableName;
        private int smallBlind;
        private int bigBlind;
        private int minAmount;
        private int buyIn;
        private bool result = false;
        private int tableIDReturned;
        private string invalid = "Is invalid, please insert a valid number";
        private string minAmountException = "Not enough cash!, insert lower min amount...";
        

        public Table createdTable;

        private UserControlTable userControlTable;
        private UserControlUserDetails userControlUserDetails;

        public delegate int CreateTableHandler(UserControlCreateTable userControlCreateTable, string tableName, int smallBlind, int bigBlind, int minAmmount, int adminID, int buyIn);
        public event CreateTableHandler CreateTable_Handler;

        public IClientListener clientListener; 

        public UserControlCreateTable()
        {
            InitializeComponent();
            clientListener = new ClientListener();

            clientListener.subscribe_create_tables(this, null, 0, 0, 0, 0, 0);
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            CreateTable();            
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CreateTable();
            }
        }

        private void CreateTable()
        {
            try
            {
                if (TableNameText.Text == null || TableNameText.Text.Equals(""))
                    throw new ArgumentException("Table name is empty!");

                tableName = TableNameText.Text;

                result = Int32.TryParse(SmallBlindText.Text, out smallBlind);
                if (!result || smallBlind <= 0)
                {
                    throw new ArgumentException("\"" + SmallBlindText.Text + "\"" + invalid);
                }

                result = Int32.TryParse(BigBlindText.Text, out bigBlind);
                if (!result || bigBlind <= 0 || bigBlind <= smallBlind )
                {
                    throw new ArgumentException("\"" + BigBlindText.Text + "\"" + invalid);
                }

                result = Int32.TryParse(MimAmount.Text, out minAmount);
                if (!result || minAmount <= 0)
                {
                    throw new ArgumentException("\"" + MimAmount.Text + "\"" + invalid);
                }
                result = Int32.TryParse(BuyIn.Text, out buyIn);
                if (!result || buyIn <= 0 || buyIn < minAmount)
                {
                    throw new ArgumentException("\"" + BuyIn.Text + "\"" + invalid);
                }
                if (minAmount > User.Money || buyIn > User.Money)
                {
                    throw new Exception(minAmountException);
                }

                if (CreateTable_Handler != null)
                {
                    User.MoneyToPlay = buyIn;
                    tableIDReturned = CreateTable_Handler(this, tableName, smallBlind, bigBlind, minAmount, User.UserID, User.MoneyToPlay);
                    createdTable = new Table(tableIDReturned, tableName, 1, smallBlind, bigBlind, minAmount, 4);
                }
                if (tableIDReturned <= 0)
                {
                    throw new Exception("Could not create table, Try again...");
                }
                User.TableID = tableIDReturned;
                User.Table = createdTable;
                moveToTable();
                ((Panel)this.Parent).Children.Remove(this);

            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }  
        }

        private void Cancle_Click(object sender, RoutedEventArgs e)
        {
            ((Panel)this.Parent).Children.Remove(this);
        }

        private void moveToTable()
        {
            User.player.Position = enumPosition.player1;
            createdTable.addPlayer(User.player);            
            User.BuyInMoney = buyIn;
            User.MoneyToPlay = buyIn;
            User.player.Money = User.MoneyToPlay;
            User.player.cards = new List<Card>();
            userControlTable = new UserControlTable(createdTable, 0);
            User.IsAdmin = true;
            ((Panel)this.Parent).Children.Add(userControlTable);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utils.StateMachine = StateMachine.CreateTable;
        }

    }
}

