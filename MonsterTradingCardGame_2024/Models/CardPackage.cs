using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Models
{
    internal class CardPackage
    {
        public List<Card> Cards { get; set; }

        public CardPackage(List<Card> cards)
        {
            if (cards.Count == 5)
            {
                Cards = cards;
            }
            else
            {
                throw new ArgumentException("A package must contain exactly 5 cards.");
            }
        }
    }
}
