using System;
using System.Collections.Generic;

namespace Game_Server
{
    [Serializable]
    public class Deck
    {
        #region events

        public delegate void DeckMessages(string msg);
        public static event DeckMessages DeckMessagesNotifier;
        #endregion

        public readonly int TopCardIndex = 0;
        public readonly int MaxCards = 52;
        private List<Card> DeckOfCards { get; set; }

        public Deck()
        {
            DeckOfCards = new List<Card>();
            SetDeck();
            Shuffle();
        }
        
        public void SetDeck()
        {
            foreach (Card.FACE face in Enum.GetValues(typeof (Card.FACE)))
            {
                if (face != Card.FACE.ACEFirst)
                {
                    foreach (Card.SUIT suit in Enum.GetValues(typeof (Card.SUIT)))
                        DeckOfCards.Add(new Card(face, suit, false));
                }
            }
        }

        public void Shuffle()
        {
            int j;
            for(int i = 0; i < DeckOfCards.Count; i++)
            {
                j = ServerManager.Rand.Next(0,MaxCards);     //1-52
                Card temp = DeckOfCards[i];
                DeckOfCards[i] = DeckOfCards[j];
                DeckOfCards[j] = temp;
            }
        }

        public Card GetCard()
        {
            try
            {
                if (DeckMessagesNotifier != null)
                    DeckMessagesNotifier("getCard function");

                Card card = DeckOfCards[TopCardIndex];    //get top card
                DeckOfCards.RemoveAt(TopCardIndex);      //remove top card
                return card;                            //return top card
            }
            catch(Exception e)
            {
                if (DeckMessagesNotifier != null)
                    DeckMessagesNotifier("getCard function exception " + e.Message);
                throw (new Exception("deck is empty"));
            }
        }

        public void BurnCard()
        {
            try
            {
                if (DeckMessagesNotifier != null)
                    DeckMessagesNotifier("burnCard function");
                DeckOfCards.RemoveAt(TopCardIndex);     //burn top card
            }
            catch (Exception e)
            {
                if(DeckMessagesNotifier != null)
                    DeckMessagesNotifier("burnCard function exception : " + e.Message);
                throw (new Exception("deck is empty"));
            }
        }

    }
}
