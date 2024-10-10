using MonsterTradingCardGame_2024.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Models
{
    internal class TradingDeal
    {
        // Constructor for Serialization
        public TradingDeal()
        {
            Id = Guid.NewGuid().ToString();
            CardToTrade = null; // Null by default
            AcceptedElement = Element.Normal;
            AcceptedSpecies = Species.Goblin;
            MinimumDamage = 0;
        }
        // Constructor for creating a new trading deal
        public TradingDeal(string id, Card cardToTrade, Element acceptedElement, Species acceptedSpecies, float minimumDamage)
        {
            Id = id;
            CardToTrade = cardToTrade;
            AcceptedElement = acceptedElement;
            AcceptedSpecies = acceptedSpecies;
            MinimumDamage = minimumDamage;
        }

        // Unique identifier for the trading deal
        public string Id { get; set; }

        // The card being offered for trade
        public Card? CardToTrade { get; set; }

        // The element of the card the user is willing to accept
        public Element AcceptedElement { get; set; }

        // The species of the card the user is willing to accept
        public Species AcceptedSpecies { get; set; }

        // The minimum damage of the card the user is willing to accept
        public float MinimumDamage { get; set; }


    }
}
