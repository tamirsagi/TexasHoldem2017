using System;
using System.Collections.Generic;

namespace Game_Server
{
    [Serializable]
    public class Card
    {
        public FACE Face { get; set; }
        public SUIT Suit { get; set; }
        public bool show { get; set; }

        public enum FACE
        {
            ACEFirst = 1,
            TWO = 2,
            THREE = 3,
            FOUR = 4,
            FIVE = 5,
            SIX = 6,
            SEVEN = 7,
            EIGHT = 8,
            NINE = 9,
            TEN = 10,
            JACK = 11,
            QUEEN = 12,
            KING = 13,
            ACE = 14
        };

        public enum SUIT
        {
            CLUBS,
            DIAMONDS,
            HEARTS,
            SPADES
        };

        public Card(FACE cardFace, SUIT cardSuit, bool isCardShown)
        {
            Suit = cardSuit;
            Face = cardFace;
            show = isCardShown;
        }

        [Serializable]
        public enum CardToPlayer
        {
            HAND,
            TABLE
        };

        public override string ToString()
        {
            return Face + " Of " + Suit;
        }

    }

    ///// <summary>
    ///// Comparator to Compare cards
    ///// </summary>
    //public class CardComparator : IComparer<Card>
    //{
    //    public int Compare(Card cardA, Card cardB)
    //    {
    //        if (cardA.Face > cardB.Face)
    //            return -1;
    //        else if (cardA.Face < cardB.Face)
    //            return 1;
    //        return 0;
    //    }
    //}
}
