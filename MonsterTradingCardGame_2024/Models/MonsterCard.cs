using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Models
{
    internal class MonsterCard : Card
    {
        public string ElementType { get; set; } // fire, water, normal

        public MonsterCard(string name, double damage, string elementType)
            : base(name, damage)
        {
            ElementType = elementType;
        }
    }
}
