using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Business_Logic
{
    internal class CardHandler
    {
        private readonly ICardRepository _cardRepository;

        public CardHandler(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public List<Card> GetUserCards(Guid userId)
        {
            return _cardRepository.GetCardsByUserId(userId);
        }

        public Card? GetCard(Guid cardId)
        {
            return _cardRepository.GetCardById(cardId);
        }

        public bool AddCard(Card card)
        {
            return _cardRepository.AddCard(card);
        }
    }
}
