using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
//using Game_Server.DBHandler;


namespace Game_Server
{
    /// <summary>
    /// that class keeps all the necessary varibles for the game server
    /// </summary>

    public class ServerManager
    {
        #region events
       // public delegate void GameServerMessages(string msg);
        
        #endregion


        public const string NameSpace = "net.tcp://10.0.0.1:8988/Game_Server/GameServerServices/";
        public static Dictionary<int, List<Thread>> ThreadsPerTable = new Dictionary<int, List<Thread>>();
        public static Dictionary<int, Table> Tables = new Dictionary<int, Table>();
        public static Dictionary<int, List<IGameServerServicesCallbacks>> ClientsInTables = new Dictionary<int, List<IGameServerServicesCallbacks>>();
        public static List<IGameServerServicesCallbacks> ClientsInServer = new List<IGameServerServicesCallbacks>(); //list of clients who are connected to the game server
        public const string ThreadName = "Thread_";
        public const string Chat = "CHAT";
        public const string WelcomeMessage = "Welcome to Table ";
        public static Random Rand = new Random();
        public static int Seat = -1;

        public enum Status
        {
            Default = 0, Success = 1, Failed = -1
        };


    }

    public class Message
    {
        public string Sender { get; set; }
        public string Msg { get; set; }

        public Message(string sender, string msg)
        {
            Sender = sender.Trim();
            Msg = msg.Trim();
        }
    }


    public class ListCombination
    {
        
    }
    
}
