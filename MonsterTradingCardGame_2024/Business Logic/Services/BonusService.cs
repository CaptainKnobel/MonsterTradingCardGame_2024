using MonsterTradingCardGame_2024.Enums;
using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Business_Logic.Services
{
    public class BonusService
    {
        private static readonly HashSet<string> ValidBonuses = new() { "FireBoost", "MonsterBoost", "SpellBoost" };
        public void ApplyBonus(string bonusType, CardDeck deck)
        {
            try
            {
                if (string.IsNullOrEmpty(bonusType) || !ValidBonuses.Contains(bonusType))
                {
                    throw new InvalidOperationException($"Invalid bonus type: {bonusType}");
                }

                switch (bonusType)
                {
                    case "FireBoost":
                        ApplyElementBoost(deck, Element.Fire, 0.1); // 10% damage boost
                        break;

                    case "MonsterBoost":
                        ApplyCardTypeBoost(deck, CardType.Monster, 5); // +5 flat damage
                        break;

                    case "SpellBoost":
                        ApplyCardTypeBoost(deck, CardType.Spell, 0.15); // 15% damage boost
                        break;

                    default:
                        throw new InvalidOperationException("Invalid bonus type specified.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error applying bonus: {ex.Message}");
                throw; // Re-throw
            }
        }

        private void ApplyElementBoost(CardDeck deck, Element element, double percentage)
        {
            foreach (var card in deck.Cards)
            {
                if (card.ElementType == element)
                {
                    card.Damage *= (1 + percentage);
                }
            }
        }

        private void ApplyCardTypeBoost(CardDeck deck, CardType type, double value)
        {
            foreach (var card in deck.Cards)
            {
                if (card.CardType == type)
                {
                    if (value < 1)
                        card.Damage *= (1 + value);
                    else
                        card.Damage += value;
                }
            }
        }
    }
}
