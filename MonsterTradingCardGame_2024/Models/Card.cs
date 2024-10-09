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

        protected Card(string name, double damage)
        {
            Id = Guid.NewGuid();
            Name = name;
            Damage = damage;
        }
    }
}
