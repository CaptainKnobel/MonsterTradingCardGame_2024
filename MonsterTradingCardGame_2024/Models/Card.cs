using MonsterTradingCardGame_2024.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Models
{
    internal abstract class Card
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Damage { get; set; }
        public CardType CardType { get; set; }

        protected Card(string name, double damage, CardType cardType)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Damage = damage;
            this.CardType = cardType;
        }
    }
}
