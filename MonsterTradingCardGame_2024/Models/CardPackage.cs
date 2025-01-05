using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Models
{
    public class CardPackage
    {
        public List<Card> Cards { get; set; }

        public CardPackage(List<Card> cards)
        {
            Cards = cards;
        }
    }
}
