using MonsterTradingCardGame_2024.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Models
{
    public class SpellCard : Card
    {

        public SpellCard(string name, double damage, Element elementType, int ownerId = 0)
            : base(name, damage, elementType, CardType.Spell, ownerId)
        {

        }
    }
}
