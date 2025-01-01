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
        IEnumerable<Card> GetCardsByUserId(int userId);
        Card? GetCardById(Guid cardId);
        bool AddCard(Card card);
    }
}
