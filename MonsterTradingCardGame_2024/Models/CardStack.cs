using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Models
{
    internal class CardStack
    {
        public List<Card> Cards { get; set; } = new List<Card>();

        // Methode zum Hinzufügen einer Karte zum Stack
        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        // Methode zum Entfernen einer Karte aus dem Stack
        public void RemoveCard(Card card)
        {
            Cards.Remove(card);
        }

        // Methode zum zufälligen Ziehen einer Karte
        public Card DrawRandomCard()
        {
            if (Cards.Count == 0)
            {
                throw new InvalidOperationException("No cards in the stack.");
            }
            Random random = new Random();
            int index = random.Next(Cards.Count);
            Card drawnCard = Cards[index];
            Cards.RemoveAt(index); // Entfernt die gezogene Karte aus dem Stapel
            return drawnCard;
        }

        // Methode zum Sortieren des Kartenstapels (optional)
        public void SortStackByDamage()
        {
            Cards = Cards.OrderByDescending(card => card.Damage).ToList();
        }
    }
}
