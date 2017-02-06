using System;

namespace Game_Server
{
    [Serializable]
    public class Dealer
    {
        public Deck Deck { get; set; }


        public Dealer(Deck deckOfCards)
        {
            Deck = deckOfCards;
        }

        public Card DealCard()
        {
            return Deck.GetCard();
        }

        public void BurnCard()
        {
            Deck.BurnCard();
        }

        public void ReOrderDeck()
        {
            Deck = new Deck();
        }





    }
}
