using MonsterTradingCardGame_2024.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Models
{
    public class MonsterCard : Card
    {
        public Species MonsterSpecies { get; set; }

        public MonsterCard(string name, double damage, Element elementType, Species monsterSpecies, int ownerId = 0)
            : base(name, damage, elementType, CardType.Monster, ownerId)
        {
            this.MonsterSpecies = monsterSpecies;
        }
    }
}
