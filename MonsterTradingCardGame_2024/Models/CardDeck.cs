using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Models
{
    public class CardDeck
    {
        public List<Card> Cards { get; set; } = new List<Card>();

        // Method to add a card to the deck
        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        // Method to remove a card from the deck
        public void RemoveCard(Card card)
        {
            Cards.Remove(card);
        }

        // Method to randomly pull a card
        public Card DrawRandomCard()
        {
            if (Cards.Count == 0)
            {
                throw new InvalidOperationException("No cards in the deck.");
            }
            Random random = new Random();
            int index = random.Next(Cards.Count);
            Card drawnCard = Cards[index];
            Cards.RemoveAt(index);
            return drawnCard;
        }
    }
}
