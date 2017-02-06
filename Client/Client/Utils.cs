using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    static class Utils
    {
        private static string IP = "";
        public static string GameServerAddress;
        public static StateMachine StateMachine { get; set; }
        public static bool IsPlayerClosedWindow = false;
        public static void setIP(string ip)
        {
            IP = ip;
            GameServerAddress = string.Format(@"net.tcp://{0}:8988/Game_Server/GameServerServices/",IP);
        }
    }
}
