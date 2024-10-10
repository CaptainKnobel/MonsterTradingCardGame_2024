using MonsterTradingCardGame_2024.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Models
{
    internal class SpellCard : Card
    {

        public SpellCard(string name, double damage, Element elementType)
            : base(name, damage, elementType, CardType.Spell)
        {

        }
    }
}
