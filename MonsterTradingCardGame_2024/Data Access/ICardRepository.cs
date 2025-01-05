using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Data_Access
{
    public interface ICardRepository
    {
        List<Card> GetCardsByUserId(Guid userId);
        Card? GetCardById(Guid cardId);
        bool AddCard(Card card);
        void UpdateCard(Card card);
    }
}
