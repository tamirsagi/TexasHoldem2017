using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game_Server
{
    [Serializable]
    public class Hand:IComparable
    {
        public const int CardsInHand = 5;

        public List<Card> Cards { get; set; }
        public HandRanking Rank { get; set; }
        
        private string _sequence;
        private string _value;

        public Hand() 
        {
            Cards = new List<Card>();
            Rank = HandRanking.Default;
        }

        public Hand(List<Card> cards)
        {
            Cards = cards;
            SetSequenceAndValue();
        }

        public enum HandRanking
        {
            RoyalFlush = 1000,
            StraightFlush = 900,
            FourOfAKind = 800,
            FullHouse = 700,
            Flush = 600,
            Straight = 500,
            ThreeOfAKind = 400,
            TwoPair = 300,
            OnePair = 200,
            HigheCard = 100,
            Default = 0
        };

        public override string ToString()
        {
            string hand = "";
            foreach (var card in Cards)
                hand += card + "\n";
            return hand;
        }

        public void SetSequenceAndValue()
        {
            StringBuilder seq = new StringBuilder();
            StringBuilder val = new StringBuilder();
            foreach (var card in Cards)
            {
                if ((int) card.Face < (int) Card.FACE.JACK)
                    seq.Append((int) card.Face);
                else
                    seq.Append(card.Face.ToString()[0]);
               
                val.Append((int) card.Face + ",");
               seq.Append(/*card.Suit.ToString()[0] +*/ ",");
            }
            _sequence = seq.Remove(seq.Length - 1, 1).ToString();
            _value = val.Remove(val.Length - 1, 1).ToString();
        }

        public string Sequence
        {
            get
            {
                if (string.IsNullOrEmpty(_sequence))
                    SetSequenceAndValue();
                return _sequence; 
            }
        }

        public string Value
        {
            get { return _value; }
        }


        public static bool IsRoyalFlush(Hand hand)
        {
            List<Card> cards = hand.Cards;
            if (cards[4].Face != Card.FACE.TEN)
                return false;
            for(int i = 0; i < cards.Count - 1; i++)
                if ((int) cards[i].Face - (int) cards[i + 1].Face != 1 || cards[i].Suit != cards[i + 1].Suit)
                    return false;
            return true;
        }

        public static bool IsStraightFlush(Hand hand)
        {
            List<Card> cards = hand.Cards;
            if ((int)cards[2].Face - (int)cards[3].Face != 1 || cards[0].Suit != cards[1].Suit)
                return false;

            if (cards[4].Face == Card.FACE.TWO && cards[0].Face == Card.FACE.ACE)
            {
                Card temp = cards[0];
                temp.Face = Card.FACE.ACEFirst;
                for (int i = 1; i < hand.Cards.Count; i++)
                    cards[i - 1] = cards[i];
                cards[cards.Count - 1] = temp;
            }
            for (int i = 0; i < cards.Count - 1; i++)
                if ((int)cards[i].Face - (int)cards[i + 1].Face != 1 || cards[i].Suit != cards[i + 1].Suit)
                    return false;
            return true;
        }

        public static bool IsFourOfAKind(Hand hand)
        {
            List<Card> cards = hand.Cards;
            if (cards[2].Face != cards[3].Face)
                return false;
            int counter = 1;
            for (int i = 0; i < cards.Count - 1; i++)
                if (cards[i].Face == cards[i + 1].Face)
                    counter++;
                else if( i != 0 && i != cards.Count - 1)
                    counter = 0;
            return counter == 4;
        }

        public static bool IsFullHouse(Hand hand)
        {
            List<Card> cards = hand.Cards;
            if (cards[0].Face == cards[1].Face)
                if(cards[1].Face == cards[2].Face || cards[2].Face == cards[3].Face)
                    return cards[3].Face == cards[4].Face;
            return false;
        }

        public static bool IsFlush(Hand hand)
        {
            List<Card> cards = hand.Cards;
            int counter = 1;
            for(int i = 0; i < cards.Count - 1; i++)
                if (cards[i].Suit == cards[i + 1].Suit)
                    counter++;
            return counter == 5;
        }

        public static bool IsStraight(Hand hand)
        {
            List<Card> cards = hand.Cards;
            if ((int)cards[2].Face - (int)cards[3].Face != 1)
                return false;
            if (cards[4].Face == Card.FACE.TWO && cards[0].Face == Card.FACE.ACE)
            {
                Card temp = cards[0];
                temp.Face = Card.FACE.ACEFirst;
                for (int i = 1; i < hand.Cards.Count; i++)
                    cards[i - 1] = cards[i];
                cards[cards.Count - 1] = temp;
            }
            int counter = 1;
            for (int i = 0; i < cards.Count - 1; i++)
                if ((int)cards[i].Face - (int)cards[i + 1].Face == 1)
                    counter++;
            return counter == 5;
        }

        public static bool IsThreeOfAKind(Hand hand)
        {
            List<Card> cards = hand.Cards;
            if ((int) cards[0].Face == (int) cards[1].Face)
                if((int) cards[1].Face == (int) cards[2].Face)
                    return true;
            if ((int)cards[1].Face == (int)cards[2].Face)
                if ((int)cards[2].Face == (int)cards[3].Face)
                    return true;
            if ((int)cards[2].Face == (int)cards[3].Face)
                if ((int)cards[3].Face == (int)cards[4].Face)
                    return true;
            return false;
        }

        public static bool IsTwoPair(Hand hand)
        {
            List<Card> cards = hand.Cards;
            int counter = 0;
            for(int i = 0, j = i + 1; j < cards.Count; j++,i++)
                if (cards[j].Face == cards[i].Face)
                    counter++;
            return counter == 2;
        }

        public static bool IsOnePair(Hand hand)
        {
            List<Card> cards = hand.Cards;
            int counter = 0;
            for (int i = 0, j = i + 1; j < cards.Count; j++, i++)
                if (cards[j].Face == cards[i].Face)
                    counter++;
            return counter == 1;
        }

        public static List<Hand> GetAllPossibleHands(IEnumerable<Card> cards)
        {
            List<Hand> possibleHands = new List<Hand>();
            List<Card> allCards = new List<Card>();
            List<Card> cardsCombination;
            foreach (Card card in cards)
                allCards.Add(card);
            for (int j = 0, i = 0; i < allCards.Count && (j = i + 1) < allCards.Count; i++)
            {
                cardsCombination = new List<Card>();
                for (int p = 0; p < allCards.Count || j < allCards.Count; p++)
                {
                    if (p != i && p != j)
                        cardsCombination.Add(allCards[p]);
                    if (cardsCombination.Count == CardsInHand)
                    {
                        possibleHands.Add(new Hand(cardsCombination));
                        if (++j < allCards.Count)
                        {
                            p = -1;
                            cardsCombination = new List<Card>();
                        }
                        else
                            break;
                    }
                }
            }
            return possibleHands;
        }

        public static Hand GetBestHand(List<Hand> possibleHands)
        {
            Hand bestHand = possibleHands[0], currentHand;
            foreach (var hand in possibleHands)
            {
                currentHand = hand;
                if (IsRoyalFlush(currentHand))
                    currentHand.Rank = HandRanking.RoyalFlush;
                else if (IsStraightFlush(currentHand))
                {
                    currentHand.Rank = HandRanking.StraightFlush;
                    ReOrderStraight(ref currentHand);
                }
                else if (IsFourOfAKind(currentHand))
                {
                    currentHand.Rank = HandRanking.FourOfAKind;
                    ReOrderFourOfAKind(ref currentHand);
                }
                else if (IsFullHouse(currentHand))
                {
                    currentHand.Rank = HandRanking.FullHouse;
                    ReOrderFullHouse(ref currentHand);
                }
                else if (IsFlush(currentHand))
                    currentHand.Rank = HandRanking.Flush;
                else if (IsStraight(currentHand))
                {
                    currentHand.Rank = HandRanking.Straight;
                    ReOrderStraight(ref currentHand);
                }
                else if (IsThreeOfAKind(currentHand))
                {
                    currentHand.Rank = HandRanking.ThreeOfAKind;
                    ReOrderThreeOfAKind(ref currentHand);
                }
                else if (IsTwoPair(currentHand))
                {
                    currentHand.Rank = HandRanking.TwoPair;
                    ReOrderTwoPair(ref currentHand);
                }
                else if (IsOnePair(currentHand))
                {
                    currentHand.Rank = HandRanking.OnePair;
                    ReOrderOnePair(ref currentHand);
                }
                else
                    currentHand.Rank = HandRanking.HigheCard;

                if (currentHand.CompareTo(bestHand) > 0)
                    bestHand = currentHand;
            }

            if (bestHand.Rank == HandRanking.Straight || 
                bestHand.Rank == HandRanking.StraightFlush || bestHand.Rank == HandRanking.RoyalFlush)
            {
                //when we check hands we set higest to lowest by card's face and Rank, here we change the order
                bestHand.Cards.Reverse();
                //we set the current sequence and value
                bestHand.SetSequenceAndValue(); 
            }
            return bestHand;
        }

        private static void ReOrderStraight(ref Hand hand)
        {
            List<Card> cards = hand.Cards;
            if (cards[4].Face == Card.FACE.TWO && cards[0].Face == Card.FACE.ACE)
            {
                Card temp = cards[0];
                temp.Face = Card.FACE.ACEFirst;
                for (int i = 1; i < hand.Cards.Count; i++)
                    cards[i - 1] = cards[i];
                cards[cards.Count - 1] = temp;
                hand.Cards = cards;
                hand.SetSequenceAndValue();
            }
        }

        private static void ReOrderFourOfAKind(ref Hand hand)
        {
            Card temp = null;
            if (hand.Cards[0].Face != hand.Cards[1].Face)
            {
                temp = hand.Cards[0];
                hand.Cards[0] = hand.Cards[4];
                hand.Cards[4] = temp;
                hand.SetSequenceAndValue();
            }
        }

        private static void ReOrderFullHouse(ref Hand hand)
        {
            if (hand.Cards[1].Face != hand.Cards[2].Face)
            {
                hand.Cards.Reverse();
                hand.SetSequenceAndValue();
            }
        }

        private static void ReOrderThreeOfAKind(ref Hand hand)
        {
            List<Card> newHand = new List<Card>();
            if (hand.Cards[1].Face == hand.Cards[2].Face && hand.Cards[2].Face == hand.Cards[3].Face)
            {
                for(int i = 0; i < hand.Cards.Count - 2; i++)
                    newHand.Add(hand.Cards[2]);
                newHand.Add(hand.Cards[0]);
                newHand.Add(hand.Cards[4]);
                hand.Cards = newHand;
                hand.SetSequenceAndValue();
            }
            else if (hand.Cards[0].Face != hand.Cards[1].Face)
            {
                hand.Cards.Reverse();
                hand.SetSequenceAndValue();
            }
        }

        private static void ReOrderTwoPair(ref Hand hand)
        {
            Card temp = null;
            int j = hand.Cards.Count;
            if (hand.Cards[0].Face != hand.Cards[1].Face)
            {
                j = 0;
                temp = hand.Cards[j];
            }
            else if (hand.Cards[1].Face != hand.Cards[2].Face && hand.Cards[2].Face != hand.Cards[3].Face)
            {
                j = 2;
                temp = hand.Cards[j = 2];
            }

            for (int i = j + 1; i < hand.Cards.Count; i++)
                hand.Cards[i - 1] = hand.Cards[i];
            
            if(j != hand.Cards.Count)
            {
                hand.Cards[4] = temp;
                hand.SetSequenceAndValue();
            }
        }

        private static void ReOrderOnePair(ref Hand hand)
        {
            if (hand.Cards[0] != hand.Cards[1])
            {
                for (int i = 0; i < hand.Cards.Count - 1; i++)
                    if (hand.Cards[i + 1].Face == hand.Cards[i].Face)
                    {
                        Card temp = hand.Cards[i];
                        for (int j = i + 1; j > 0 && i > 0; j--,i--)
                            hand.Cards[j] = hand.Cards[i - 1];

                        hand.Cards[0] = temp;
                        hand.Cards[1] = temp;
                        hand.SetSequenceAndValue();
                        break;
                    }
            }
        }

        public int CompareTo(object obj)
        {
            if (Rank > ((Hand) obj).Rank)
                return 1;
            else if (Rank < ((Hand) obj).Rank)
                return -1;

            else
            {
                string[] firstHand = _value.Split(',');
                string[] secondHand = ((Hand) obj)._value.Split(',');
                for (int i = 0; i < firstHand.Length; i++)
                {
                    int firstHandVal = int.Parse(firstHand[i]), secondHandVal = int.Parse(secondHand[i]);
                    if (firstHandVal > secondHandVal)
                      return 1;
                    else if (firstHandVal < secondHandVal)
                        return -1;
                }
            }
            return 0;
        }
    }
}
