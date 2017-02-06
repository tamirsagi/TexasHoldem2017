using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
using System.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for UserControlTable.xaml
    /// </summary>
    public partial class UserControlTable : UserControl
    {
        private readonly string BigBlind = "Big";
        private readonly string SmallBlind = "Small";
        private UserControlBets bets;
        private Image[] playersCards = new Image[] { };
        public Table Table { set; get; }
        private int userLocation = 0;
        private readonly int maxPlayers = 4;
        
        private IClientListener     clientListener;
        private IControllerListener controllerListener;
        private CardsConverter      cardsConverter;
        private List<Label>         playersLabels;
        private List<Label>         PlayersRole;
        private List<Image>         playersCardsList;
        private List<Image>         deckCardsList;
        private List<Image>         SharedCards;
        private List<Button>        playersButtons;
        private List<Button>        playButtons;
        private static int []       playerLocationIncrement;
        private static int []       playerCardLocationIncrement;
        
        public Player playerToKick;
        //private Dictionary<int, Label> PlayersLabels;
        //private IController controller;

        //public delegate int getCurrentBet(UserControlTable userControlTable, int tableID);
        //public event getCurrentBet getCurrentBet_Handler;

        public delegate void notifyPlayerFolded(UserControlTable userControlTable, int userID);
        public event notifyPlayerFolded notifyPlayerFolded_Handler;

        public delegate void notifyPlayerLeftTable(UserControlTable userControlTable, int userID, int tableID, int userMoney);
        public event notifyPlayerLeftTable notifyPlayerLeftTable_Handler;

        public delegate void CheckHandler(UserControlTable userControlTable, int tablID, int userID);
        public event CheckHandler CheckHandler_Handler;

        public delegate int GetCurrentBalanceFromServer(UserControlTable userControlTable, int userID);
        public event GetCurrentBalanceFromServer GetCurrentBalanceFromServer_Handler;

        public UserControlTable( Table table, int location)
        {
            InitializeComponent();
            clientListener = new ClientListener();
            controllerListener = new ControllerListener();
            clientListener.subscribe_notify_player_folded(this, -1);
            clientListener.subscribe_notify_player_left_table(this, -1, -1, -1);
            clientListener.subscribe_check_handler(this, -1, -1);
            clientListener.subscribe_get_current_balance_from_server(this, -1);

            playersLabels = new List<Label>();
            PlayersRole = new List<Label>();
            playersCardsList = new List<Image>();
            deckCardsList = new List<Image>();
            SharedCards = new List<Image>();
            playersButtons = new List<Button>();
            playButtons = new List<Button>();
            Table = table;
            cardsConverter = new CardsConverter();
            playerLocationIncrement = new int[maxPlayers];
            playerCardLocationIncrement = new int[maxPlayers];
            try
            {
                setPlayerCardsList();
                setPlayersLabels();
                setPlayerRoleList();
                setPlayButtons();
                setPlayerList();                
                setDeck();
                setSharedCardsList();
                Table.SetUserControlTable(this);
                checkIfPlayerIsAlone();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void setPlayerList()
        {
            int i = 0;
            foreach (var player in Table.Players)
            {
                if (player != null)
                {
                    playersLabels[i].Content = player.UserName;                    
                }
                i++;
            }
            if (!User.player.IsPlaying)
                disableEnableAllPlayButtons(false);
        }

        private void checkIfPlayerIsAlone()
        {
            int counter = 0;
            foreach (var player in Table.Players)
            {
                if (player != null)
                    counter++;                
            }

            if (counter <= 1)
            {
                ReceivedGameEventsTextBox.Text = "Waiting for players to join...\n";
                disableEnableAllPlayButtons(false);
                Table.GameStatus = enumGameStatus.Finished;
                deletePlayerCards(User.player.Position);
                deleteSharedCards();
                User.player.cards = new List<Card>();
                Table.SharedCards = new List<Card>();
                playerLocationIncrement = new int[maxPlayers];
                playerCardLocationIncrement = new int[maxPlayers];
            }
            else
            {
                Table.GameStatus = enumGameStatus.Started;
            }
        }

        public void clearAfterRound()
        {
            Table.GameStatus = enumGameStatus.Finished;
            foreach (var player in Table.Players)
            {
                deletePlayerCards(player.Position);
                player.cards = new List<Card>();
            }
            User.player.cards = new List<Card>();
            PotLabel.Content = "";
            MinBetLabel.Content = "";
            deleteSharedCards();
            Table.SharedCards = new List<Card>();
            disableEnableAllPlayButtons(false);
            playerLocationIncrement = new int[maxPlayers];
            playerCardLocationIncrement = new int[maxPlayers];
        }

        public void deletePlayerFromTable(enumPosition p)
        {
            deletePlayerCards(p);         
            playersLabels[(int)p].Content = "";
            PlayersRole[(int)p].Content = "";
            Table.Players[(int)p] = null;
            checkIfPlayerIsAlone();
        }

        public void deletePlayerCards(enumPosition p)
        {
            SetImage(playersCardsList[(int)p], "");
            SetImage(playersCardsList[(int)p + maxPlayers], "");    
        }

        public void setPlayerRoleList()
        {
            PlayersRole.Add(Role1);
            PlayersRole.Add(Role2);
            PlayersRole.Add(Role3);
            PlayersRole.Add(Role4);
        }

        public void setRoleToPlayer()
        {
            int i = 0;
            foreach (var player in Table.Players)
            {
                if (player.Role == enumPlayerRole.BigBlind)
                    PlayersRole[i].Content = BigBlind;
                else if (player.Role == enumPlayerRole.SmallBlind)
                    PlayersRole[i].Content = SmallBlind;
                else 
                    PlayersRole[i].Content = "";
                i++;
            }           
        }

        public void setKickButton()
        {
            if (!User.IsAdmin)
                enableDisableButton(KickButton, false,0);
            else
                enableDisableButton(KickButton, true,0);
        }

        public void enableDisableButton(Button button, bool state, double precent)
        {
            if (!state)
            {
                button.Opacity = precent;
                button.IsEnabled = state;
            }
            else
            {
                button.Opacity = 1;
                button.IsEnabled = state;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="button"></param>
        /// <param name="state">true button is enabled, false is disabled!</param>
        public void enableDisableButtonNoOpacity(Button button, bool state)
        {
            button.IsEnabled = state;            
        }

        private void initPlayersButton()
        {
            playersButtons.Add(labelPlayer1Button);
            playersButtons.Add(labelPlayer2Button);
            playersButtons.Add(labelPlayer3Button);
            playersButtons.Add(labelPlayer4Button);
        }

        private void setPlayerCardsList()
        {
            playersCardsList.Add(Player1FirstCard);            
            playersCardsList.Add(Player2FirstCard);            
            playersCardsList.Add(Player3FirstCard);            
            playersCardsList.Add(Player4FirstCard);                        
            playersCardsList.Add(Player1SecondCard);
            playersCardsList.Add(Player2SecondCard);
            playersCardsList.Add(Player3SecondCard);
            playersCardsList.Add(Player4SecondCard);

            for (int i = 0; i < playerLocationIncrement.Length; i++)
            {
                playerLocationIncrement[i] = 0;
            }
        }

        private void setSharedCardsList()
        {
            SharedCards.Add(CardFlop1);
            SharedCards.Add(CardFlop2);
            SharedCards.Add(CardFlop3);
            SharedCards.Add(CardRiver);
            SharedCards.Add(CardTurn);
        }

        private void setDeck()
        {
            SetImage(Deck1, cardsConverter.cardsConverter(enumCardType.Deck, 0));
            SetImage(Deck2, cardsConverter.cardsConverter(enumCardType.Deck, 0));
        }

        private void setPlayButtons()
        {
            playButtons.Add(RaiseButton);
            playButtons.Add(CallButton);
            playButtons.Add(CheckButton);
            playButtons.Add(FoldButton);
        }

        public void disableEnableAllPlayButtons(bool state)
        {

            foreach (var button in playButtons)
	        {
                enableDisableButtonNoOpacity(button, state);
	        }
            
        }

        public void initUserCards()
        {

            User.player.cards[User.player.cards.Count - 1].Image = new Image();//get free space
            if (User.player.cards.Count == 1)
                User.player.cards[User.player.cards.Count - 1].Image = playersCardsList[(int)User.player.Position];
            else
                User.player.cards[User.player.cards.Count - 1].Image = playersCardsList[(int)User.player.Position + maxPlayers];
        }

        private void setCardToUser(List<Card> cards)
        {            
            SetImage(cards[0].Image, cardsConverter.cardsConverter(cards[0].Suit, (int)cards[0].Face));
            SetImage(cards[1].Image, cardsConverter.cardsConverter(cards[1].Suit, (int)cards[1].Face));            
        }

        /// <summary>
        /// set deck to all the rest
        /// </summary>
        public void setDeckToOtherPlayers()
        {
            
            int i = 0;
            foreach (var player in Table.Players)
            {
                if (player.UserName.Equals(User.UserName))
                {
                    i++;
                    continue;
                }
                if (player.cards.Count == 0)
                    player.cards = new List<Card> { new Card(), new Card() };
                player.cards[0].Image = playersCardsList[i];
                player.cards[1].Image = playersCardsList[i+4];
                SetImage(player.cards[0].Image, cardsConverter.cardsConverter(enumCardType.Deck, 0));
                SetImage(player.cards[1].Image, cardsConverter.cardsConverter(enumCardType.Deck, 0));
                i++;
            }
        }

        public void getFlopFromTable(Card card)
        {
            //List<Card> flop = new List<Card>();
            //flop = Table.sendFlop();
            //setFlop(card);
            setSharedCardsOnTable(card, Table.SharedCards.Count-1);
            //if (sharedCardCounter < maxSharedCards)
              //  sharedCardCounter++;
        }
        
        public void getCardsToUserFromTable(Card card, int location)
        {
            userLocation = (int)User.player.Position;
            User.player.cards[playerCardLocationIncrement[(int)User.player.Position]].Image = playersCardsList[(int)User.player.Position + playerLocationIncrement[(int)User.player.Position]];
            SetImage(User.player.cards[playerCardLocationIncrement[(int)User.player.Position]].Image, cardsConverter.cardsConverter(card.Suit, (int)card.Face));
            playerLocationIncrement[(int)User.player.Position] += maxPlayers;
            playerCardLocationIncrement[(int)User.player.Position]++;
        }

        public void getCardsToAllPlayersFromTable(Card card, int location, Player player)
        {
            player.cards[playerCardLocationIncrement[location]].Image = playersCardsList[location + playerLocationIncrement[location]];
            SetImage(player.cards[playerCardLocationIncrement[location]].Image, cardsConverter.cardsConverter(card.Suit, (int)card.Face));
            playerLocationIncrement[location] += maxPlayers;
            playerCardLocationIncrement[location]++;
        }

        public void setSharedCardsOnTable(Card card, int count)
        {
            SetImage(SharedCards[count], cardsConverter.cardsConverter(card.Suit, (int)card.Face));
        }

        private void setPlayerName(Label label, string name)
        {
            label.Content = name;
        }

        private void setPlayersLabels()
        {
            playersLabels.Add(labelPlayer1);
            playersLabels.Add(labelPlayer2);
            playersLabels.Add(labelPlayer3);
            playersLabels.Add(labelPlayer4);
        }

        private void Raise_Click(object sender, RoutedEventArgs e)
        {
            ShowBets(Table.CurrentBet);
        }

        private void Call_Click(object sender, RoutedEventArgs e)
        {
            if(User.MoneyToPlay < Table.CurrentBet)
                CallCheck(User.MoneyToPlay);
            else
                CallCheck(Table.CurrentBet);
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            if (CheckHandler_Handler != null)
                CheckHandler_Handler(this, Table.TableID, User.UserID);
        }

        private void CallCheck(int sumToBet)
        {
            bets = new UserControlBets(sumToBet, Table.TableID, Table.CurrentBet);
            bets.Bet(sumToBet);
        }

        private void Fold_Click(object sender, RoutedEventArgs e)
        {
            if (notifyPlayerFolded_Handler != null)
            {
                notifyPlayerFolded_Handler(this, User.UserID);
            }            
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (notifyPlayerLeftTable_Handler != null)
            {
                notifyPlayerLeftTable_Handler(this, User.UserID, User.TableID, User.BuyInMoney - User.MoneyToPlay);
                closeTable();
            }
            
        }

        public void closeTable()
        {
            User.IsAdmin = false;

            //if (GetCurrentBalanceFromServer_Handler != null)
            //    GetCurrentBalanceFromServer_Handler(this, User.UserID);

            ((Panel)this.Parent).Children.Remove(this);
        }

        //public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        //{
        //    //get parent item
        //    DependencyObject parentObject = VisualTreeHelper.GetParent(child);

        //    //we've reached the end of the tree
        //    if (parentObject == null) return null;

        //    //check if the parent matches the type we're looking for
        //    T parent = parentObject as T;
        //    if (parent != null)
        //        return parent;
        //    else
        //        return FindParent<T>(parentObject);
        //}

        private void SetImage(Image image, string path)
        {
            image.BeginInit();
            image.Source = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
            image.EndInit();
        }

        public Image returnImage(string path)
        {
            Image image = new Image();
            image.BeginInit();
            image.Source = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
            image.EndInit();
            return image;
        }

        void ShowBets(int raise)
        {
            bets = new UserControlBets(raise,Table.TableID, Table.CurrentBet);
            mainGrid.Children.Add(bets);

        }

        public void MoveTo(Image source, Image target)
        {
            double newX = target.Margin.Top, newY = target.Margin.Left;
            var top = Canvas.GetTop(source);
            var left = Canvas.GetLeft(source);

            TranslateTransform trans = new TranslateTransform();
            source.RenderTransform = trans;
            DoubleAnimation anim1 = new DoubleAnimation(top, top+15/*newY - top*/, TimeSpan.FromSeconds(2));
            DoubleAnimation anim2 = new DoubleAnimation(left, left+15/*newX - left*/, TimeSpan.FromSeconds(2));
            trans.BeginAnimation(TranslateTransform.XProperty, anim1);
            trans.BeginAnimation(TranslateTransform.YProperty, anim2);
        }

        private void Kick_Click(object sender, RoutedEventArgs e)
        {
            if (User.IsAdmin)
            {
                if (Table.Players.Count > 1)
                {
                    UserControlPlayersToKick userControlPlayersToKick = new UserControlPlayersToKick(getPlayersWithOutUser(), Table.TableID);
                    Grid1.Children.Add(userControlPlayersToKick);                    
                }
            }
            else
            {
                MessageBox.Show("You are the only player!");
            }
            
        }

        private List<Player> getPlayersWithOutUser()
        {
            List<Player> playersWithOutUser = new List<Player>();
            foreach (var player in Table.Players)
            {
                if(!player.UserName.Equals(User.UserName))
                    playersWithOutUser.Add(player);
            }
            return playersWithOutUser;
        }

        private string playerInfo(Player player)
        {
            return string.Format("{0} Has {1}",player.UserName,player.Money);
        }

        private void labelPlayer1Button_Click(object sender, RoutedEventArgs e)
        {
            if (Table.Players[0] != null)
            {
                playerToKick = Table.Players[0];
                MessageBox.Show(playerInfo(Table.Players[0]));
            }
        }

        private void labelPlayer2Button_Click(object sender, RoutedEventArgs e)
        {
            //if (Table.Players[1] != null)
            if (Table.Players.Count > 1 && Table.Players[1] != null)
            {
                playerToKick = Table.Players[1];
                MessageBox.Show(playerInfo(Table.Players[1]));
            }
        }

        private void labelPlayer3Button_Click(object sender, RoutedEventArgs e)
        {
            if (Table.Players.Count > 2 && Table.Players[2] != null)
            {
                playerToKick = Table.Players[2];
                MessageBox.Show(playerInfo(Table.Players[2]));
            }
        }

        private void labelPlayer4Button_Click(object sender, RoutedEventArgs e)
        {
            if (Table.Players.Count > 3 && Table.Players[3] != null)
            {
                playerToKick = Table.Players[3];
                MessageBox.Show(playerInfo(Table.Players[3]));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utils.StateMachine = StateMachine.Table;
            setKickButton();
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendChatText();
            }
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            SendChatText();
        }

        private void SendChatText()
        {
            if (!string.IsNullOrEmpty(SendChatTextBox.Text))
            {
                Table.SendChatText(SendChatTextBox.Text);
                SendChatTextBox.Text = "";
            }
        }

        public void ReceiveChatText(string text)
        {
            ReceiveChatTextBox.IsReadOnly = false;
            ReceiveChatTextBox.Text += text;
            ReceiveChatTextBox.Text += "\n";
            ReceiveChatTextBox.ScrollToEnd();
            ReceiveChatTextBox.IsReadOnly = true;
        }

        public void ReceivePotChange(int pot)
        {
            PotLabel.Content = pot;
        }

        public void ReceiveCurrentPlayer(string currentPlayer, int minAmountToBet)
        {

            if (currentPlayer.Equals(User.UserName))
            {
                CurrentPlayerLabel.Content = "You";
                disableEnableAllPlayButtons(true);
                if (minAmountToBet == 0)
                    enableDisableButtonNoOpacity(CallButton, false);
                else
                {
                    if (User.MoneyToPlay < minAmountToBet)
                    {
                        enableDisableButtonNoOpacity(RaiseButton, false);
                    }
                        
                    enableDisableButtonNoOpacity(CheckButton, false);
                }


            }
            else
            {
                CurrentPlayerLabel.Content = currentPlayer;
                disableEnableAllPlayButtons(false);
            }

            MinBetLabel.Content = minAmountToBet;
            Table.CurrentBet = minAmountToBet;/*Added!*/
            
        }

        public void ReceiveGameEvents(string text)
        {
            ReceivedGameEventsTextBox.IsReadOnly = false;
            ReceivedGameEventsTextBox.Text += text;
            ReceivedGameEventsTextBox.Text += "\n";
            ReceivedGameEventsTextBox.ScrollToEnd();
            ReceivedGameEventsTextBox.IsReadOnly = true;
            checkIfPlayerIsAlone();
        }

        public void deleteAllCards()
        {
            deletePlayersCards();
            deleteSharedCards();
        }
        public void deletePlayersCards()
        {
            foreach (var card in playersCardsList)
            {
                SetImage(card, "");
            }
        }
        public void deleteSharedCards()
        {
            foreach (var card in SharedCards)
            {
                SetImage(card, "");
            }
        }

    }
}
