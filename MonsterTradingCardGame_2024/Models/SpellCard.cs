using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Models
{
    internal class SpellCard : Card
    {
        public string ElementType { get; set; } // fire, water, normal

        public SpellCard(string name, double damage, string elementType)
            : base(name, damage)
        {
            ElementType = elementType;
        }
    }
}
