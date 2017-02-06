using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{

    public class CardsConverter
    {
        private Dictionary<int, string> Clubs = new Dictionary<int, string>();
        private Dictionary<int, string> Dimonds = new Dictionary<int, string>();
        private Dictionary<int, string> Hearts = new Dictionary<int, string>();
        private Dictionary<int, string> Spades = new Dictionary<int, string>();
        private string[] clubsArray = new string[]
        {
         "TwoOfClubs",
        "ThreeOfClubs",
         "FourOfClubs",
         "FiveOfClubs",
          "SixOfClubs",
        "SevenOfClubs",
        "EightOfClubs",
         "NineOfClubs",
          "TenOfClubs",
         "JackOfClubs",
        "QueenOfClubs",
         "KingOfClubs",
          "AceOfClubs",
        };
        private string[] dimondsArray = new string[]
        {
          "TwoOfDimonds",
        "ThreeOfDimonds",
         "FourOfDimonds",
         "FiveOfDimonds",
          "SixOfDimonds",
        "SevenOfDimonds",
        "EightOfDimonds",
         "NineOfDimonds",
          "TenOfDimonds",
         "JackOfDimonds",
        "QueenOfDimonds",
         "KingOfDimonds",
          "AceOfDimonds",
        };
        private string[] heartsArray = new string[]
        {
         "TwoOfHearts",
        "ThreeOfHearts",
         "FourOfHearts",
         "FiveOfHearts",
          "SixOfHearts",
        "SevenOfHearts",
        "EightOfHearts",
         "NineOfHearts",
          "TenOfHearts",
         "JackOfHearts",
        "QueenOfHearts",
         "KingOfHearts",
          "AceOfHearts",
        };
        private string[] spadesArray = new string[]
        {
          "TwoOfSpades",
        "ThreeOfSpades",
         "FourOfSpades",
         "FiveOfSpades",
          "SixOfSpades",
        "SevenOfSpades",
        "EightOfSpades",
         "NineOfSpades",
          "TenOfSpades",
         "JackOfSpades",
        "QueenOfSpades",
         "KingOfSpades",
          "AceOfSpades",
        };
        private int length = 15;
        private string gif = @".gif";
        private string jpg = @".jpg";
        private string prePathClubs = @"Clubs\";
        private string prePathDimonds = @"Dimonds\";
        private string prePathHearts = @"Hearts\";
        private string prePathSpades = @"Spades\";
        private string prePath = @"Images\Cards\";
        public CardsConverter()
        {
            setDictionaries();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cardType"></param>
        /// <param name="num">from 2 to 14</param>
        /// <returns>Image path</returns>
        public string cardsConverter(enumCardType cardType, int num)
        {
            switch (cardType)
            {
                case enumCardType.Clubs:
                    return Clubs[num];
                case enumCardType.Dimonds:
                    return Dimonds[num];
                case enumCardType.Hearts:
                    return Hearts[num];
                case enumCardType.Spades:
                    return Spades[num];
                case enumCardType.Deck:
                    return prePath + "Deck" + jpg;
                default:
                    throw new ArgumentException("please insert a valid card type");
            }
        }

        private void setDictionaries()
        {
            setDictionary(Clubs, prePathClubs, clubsArray);
            setDictionary(Dimonds, prePathDimonds, dimondsArray);
            setDictionary(Hearts, prePathHearts, heartsArray);
            setDictionary(Spades, prePathSpades, spadesArray);
            
        }
        private void setDictionary(Dictionary<int, string> d, string type, string[] arr)
        {
            for (int i = 2; i < length; i++)
            {
                d.Add(i, prePath + type + arr[i - 2] + gif);

            }
        }


    }
}
