using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public class User
    {
        private static User mInstance;

        public static string    UserName    { set; get; }
        public static string    Password    { set; get; }
        public static int       Money       { set; get; }
        public static int       BuyInMoney  { set; get; }
        public static int       MoneyToPlay { set; get; }
        public static int       UserID      { set; get; }
        public static int       TableID     { set; get; }
        public static bool      IsAdmin     { set; get; }
        public static Player    player      { set; get; }
        public static Table     Table       { set; get; }
        

        private User(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
        public static User Instance(string userName, string password)
        {
            if (mInstance == null)
            {
                mInstance = new User(userName, password);                
            }
            return mInstance;
        }
        public static void setUser(string userName, string password, int money, int userID)
        {
            User.UserName = userName;
            User.Password = password;
            User.Money = money;
            User.UserID = userID;
            player = new Player(UserName, 0);
            player.PlayerID = userID;
                
        }
        public static void unsetUser()
        {
            User.UserName = null;
            User.Password = null;
            User.Money = 0;
            User.UserID = -1;
            User.BuyInMoney = 0;
            User.MoneyToPlay = 0;            
            User.TableID = -1;
            User.IsAdmin = false;
            User.player = new Player();
            User.Table = null;
      }

    }
}