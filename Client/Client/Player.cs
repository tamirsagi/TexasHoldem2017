using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
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
    /// Position - from 0-3 defines the player positin in the table
    /// </summary>
    public class Player
    {
        public string           UserName        { set; get; }        
        public int              Money           { set; get; }        
        public int              TableID         { set; get; }
        public int              PlayerID        { set; get; }
        public enumPosition     Position        { set; get; }
        public List<Card>       cards           { set; get; }
        public List<Image>      cardsImage      { set; get; }
        public enumPlayerRole   Role            { set; get; }
        public bool             IsPlaying       { set; get; }


        public Player() { }

        public Player(string userName, int money)
        {
            UserName = userName;
            Money = money;
            cards = new List<Card>() {new Card(), new Card() };
            IsPlaying = true;
        }

        public Player(string userName, int money, int location, int playerID, bool isPlaying)
        {
            UserName = userName;
            Money = money;
            cards = new List<Card>() { new Card(), new Card() };
            Position = (enumPosition)location;
            PlayerID = playerID;
            IsPlaying = isPlaying;
        }
        
        public void setRole(enumPlayerRole role)
        {
            Role = role;
        }
    }
}
