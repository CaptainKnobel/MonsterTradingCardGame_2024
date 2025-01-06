using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Data_Access
{
    public interface IDeckRepository
    {
        List<Card> GetDeckByUserId(Guid userId);
        bool UpdateDeck(Guid userId, IEnumerable<Guid> cardIds);
        bool RemoveCardsFromDeck(Guid userId, IEnumerable<Guid> cardIds);
    }
}
