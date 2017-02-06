using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class Card
    {
        public FACE         Face    { get; set; }
        public enumCardType Suit    { get; set; }
        public bool         show    { get; set; }
        public Image        Image   { get; set; }

        public enum FACE
        {
            TWO = 2, THREE = 3, FOUR = 4, FIVE = 5, SIX = 6, SEVEN = 7, EIGHT = 8,
            NINE = 9, TEN = 10, JACK = 11, QUEEN = 12, KING = 13, ACE = 14
        };

        public enum SUIT
        {
            CLUBS, DIAMONDS, HEARTS, SPADES
        };

        public Card()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardFace" from 2 to 14></param>
        /// <param name="cardSuit" from 0 to 3></param>
        /// <param name="isCardShown"></param>
        public Card(FACE cardFace, enumCardType cardSuit, bool isCardShown)
        {
            Suit = cardSuit;
            Face = cardFace;
            show = isCardShown;
        }
        public Card(int cardFace, int cardSuit, bool isCardShown)
        {
            Suit = (enumCardType)cardSuit;
            Face = (FACE)cardFace;
            show = isCardShown;
        }
        public string toString()
        {
            return Face.ToString() + " Of " + Suit.ToString();
        }
    }
}
