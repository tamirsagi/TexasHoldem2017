using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using TexasHoldemDBWCF;


///  Texas Holdem Client-Server!! WCF and more  \\\\\\\\\\\\\\\\


/// **** Small Pots Are not implemented
/// **** All in Button is not implemented
/// ***when raise turns are changed wrong way
/// //In Client the

namespace Game_Server
{
    public class GameServer
    {
        private static Deck deck;

        private static void Main(string[] args)
        {

            RegisterToNotifiers();

            using (ServiceHost gameServer = new ServiceHost(typeof (GameServerServices))) //define host
            {
                gameServer.Open();
                Console.WriteLine("Game server started press any key to terminate");
               CheckDbFunctions();
                //Console.ReadLine();
                gameServer.Close();
                Console.WriteLine("Game server stopped");
            }

            DeRegisterToNotifiers();

        }

        public static void RegisterToNotifiers()
        {
            GameServerServices.GameServerServicesMessagesNotifier += GameServerMessagesHandler;
            Table.TableMessagesNotifier += GameServerMessagesHandler;
            Player.PlayerMessagesNotifier += GameServerMessagesHandler;
            Deck.DeckMessagesNotifier += GameServerMessagesHandler;
            DBServicesHandler.DbServicesNotifier += GameServerMessagesHandler;
        }

        public static void DeRegisterToNotifiers()
        {
            GameServerServices.GameServerServicesMessagesNotifier -= GameServerMessagesHandler;
            Table.TableMessagesNotifier -= GameServerMessagesHandler;
            Player.PlayerMessagesNotifier -= GameServerMessagesHandler;
            Deck.DeckMessagesNotifier -= GameServerMessagesHandler;
            DBServicesHandler.DbServicesNotifier -= GameServerMessagesHandler;
        }

        public static void GameServerMessagesHandler(string msg)
        {
            Console.WriteLine(msg);
        }

        //    /// <summary>
        //    /// testing DB & WCF functionallity
        //    /// </summary>

        public static void CheckDbFunctions()
        {
            string input;
            while ((input = Console.ReadLine()) != "e")
            {
                switch (input)
                {
                    case "l":
                      //  Login("TamirS", "1245");
                        break;
                    case "t":
                       // PrintTables();
                        break;
                    case "i":
                       // AddPlayerToTable(1, 6);
                        break;
                    case "g":
                       // GetId(TablesInDb.GameTable.ToString(), "a");
                        break;
                    case "k":
                        //PrintallTablesfromDic();
                        break;
                    case "c":
                        //SendCard();
                        break;
                    case "s":
                        //sendChat("test");
                        break;
                    case "q":
                        //SortHand();
                        break;
                }
            }
        }

        //private static void SortHand()
        //{
        //    Hand hand = new Hand();
        //    hand.Cards.Add(new Card(Card.FACE.ACE, Card.SUIT.HEARTS, false));
        //    hand.Cards.Add(new Card(Card.FACE.KING, Card.SUIT.CLUBS, false));
        //    hand.Cards.Add(new Card(Card.FACE.KING, Card.SUIT.HEARTS, false));
        //    hand.Cards.Add(new Card(Card.FACE.KING, Card.SUIT.CLUBS, false));
        //    hand.Cards.Add(new Card(Card.FACE.ACE, Card.SUIT.HEARTS, false));
        //    hand.Cards.Add(new Card(Card.FACE.ACE, Card.SUIT.CLUBS, false));
        //    hand.Cards.Add(new Card(Card.FACE.NINE, Card.SUIT.DIAMONDS, false));
        //    Console.WriteLine(hand.Sequence);
        //    Console.WriteLine(Hand.IsOnePair(hand));
        //    var orderHand = hand.Cards.OrderBy(card => card.Face).Reverse();        //decending order
        //    foreach (var card in orderHand)
        //    {
        //        Console.WriteLine(card);
        //    }
        //    Console.WriteLine();
        //    var PossibleHands = Hand.GetAllPossibleHands(orderHand);
        //    var BestHand = Hand.GetBestHand(PossibleHands);
        //    string[] s = {"tell", "arr", "more", "ill", "robus"};
        //    string first = new string(s.Where(str=>!string.IsNullOrEmpty(str)).Select(str=>str[0]).ToArray());
        //    Console.WriteLine(  first);
        //    foreach (var card in BestHand.Cards)
        //    {
        //        Console.WriteLine(card);
        //    }
        //    Console.WriteLine(BestHand.Sequence + "\n" + BestHand.Rank + "\n" + BestHand.Value);
        //    var possibleHAnds = Hand.GetAllPossibleHands(orderHand);
        //    Console.WriteLine( "end");
        //}



        //    public static void Login(string un,string pwd)
        //    {
        //        int answer = DBManager.Manager.Login(un, pwd);
        //        System.Data.DataRow p = DBManager.Manager.GetPlayer(answer);

        //    }
        //    public static void PrintTables()
        //    {
        //        int j = 1;
        //        List<TexasHoldemDBWCF.Table> ls = DBServicesHandler.DbHandler.GetAllGamesTables();
        //        foreach (TexasHoldemDBWCF.Table t in ls)
        //            Console.WriteLine("Table #" + (j++) + " ID = {0} Name = {1} max = {2} smallBlind = {3}", t.TableID, t.Name, t.MaxPlayers, t.SmallBlind);
        //    }

        //    public static void AddPlayerToTable(int id,int pid)
        //    {
        //        bool r = DBServicesHandler.DbHandler.JoinToTable(id, pid);
        //        Console.WriteLine("result is : {0}",r);

        //    }

        //    public static void GetId(string table, string search)
        //    {
        //        int id = DBServicesHandler.DbHandler.GetId(table, search);
        //        Console.WriteLine("ID : {0}",id);

        //    }

        //    public static void PrintallTablesfromDic()
        //    {
        //        foreach(int id in ServerManager.Tables.Keys)
        //            Console.WriteLine(ServerManager.Tables[id].ToString());
        //    }

        //    public static void SendCard()
        //    {
        //        deck = new Deck();
        //        deck.Shuffle();
        //        //foreach (var client in ServerManager.ClientsInServer)
        //        //{
        //        //    client.SendCardToPlayer(deck.GetCard(), Card.CardToPlayer.TABLE);
        //        //    client.SendCardToPlayer(deck.GetCard(), Card.CardToPlayer.TABLE);
        //        //    client.SendCardToPlayer(deck.GetCard(), Card.CardToPlayer.TABLE);
        //        //}
        //        foreach (var client in ServerManager.ClientsInServer)
        //        {
        //            client.SendCardToPlayer(deck.GetCard(), Card.CardToPlayer.HAND,0);
        //           // client.SendCardToPlayer(deck.GetCard(), Card.CardToPlayer.HAND,0);
        //        }

        //        //foreach (var client in ServerManager.ClientsInServer)
        //        //{
        //        //    client.SendCardToPlayer(deck.GetCard(), Card.CardToPlayer.TABLE);
        //        //    client.SendCardToPlayer(deck.GetCard(), Card.CardToPlayer.TABLE);
        //        //}
        //    }

        //    public static void sendChat(string msg)
        //    {
        //        try
        //        {
        //            foreach (var client in ServerManager.ClientsInServer)
        //                client.UpdateMessageInChat(msg);
        //        }catch(Exception e)
        //        {
        //            Console.WriteLine(e.Message);
        //        }
        //    }
        //}
    }
}
