﻿using MonsterTradingCardGame_2024.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Models
{
    internal class MonsterCard : Card
    {
        public Element ElementType { get; set; }
        public Species MonsterSpecies { get; set; }

        public MonsterCard(string name, double damage, Element elementType, Species species)
            : base(name, damage, CardType.Monster)
        {
            ElementType = elementType;
            MonsterSpecies = species;
        }
    }
}
