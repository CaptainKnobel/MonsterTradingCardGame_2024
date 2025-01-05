using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Business_Logic
{
    public class DeckHandler
    {
        private readonly IDeckRepository _deckRepository;
        private readonly ICardRepository _cardRepository;

        public DeckHandler(IDeckRepository deckRepository, ICardRepository cardRepository)
        {
            _deckRepository = deckRepository;
            _cardRepository = cardRepository;
        }

        public IEnumerable<Card> GetDeckByUserId(Guid userId)
        {
            return _deckRepository.GetDeckByUserId(userId);
        }

        public bool ValidateDeck(IEnumerable<Guid> cardIds, Guid userId)
        {
            // 1. Deck muss genau 4 Karten enthalten
            if (cardIds.Count() != 4)
            {
                return false;
            }

            // 2. Karten müssen dem Nutzer gehören
            var userCards = _cardRepository.GetCardsByUserId(userId);
            return cardIds.All(id => userCards.Any(card => card.Id == id));
        }

        public bool UpdateDeck(Guid userId, IEnumerable<Guid> cardIds)
        {
            if (!ValidateDeck(cardIds, userId))
            {
                throw new InvalidOperationException("Invalid deck configuration");
            }

            return _deckRepository.UpdateDeck(userId, cardIds);
        }
    }
}
