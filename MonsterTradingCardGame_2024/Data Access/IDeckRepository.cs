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
        IEnumerable<Card> GetDeckByUserId(int userId);
        bool UpdateDeck(int userId, IEnumerable<Guid> cardIds);
    }
}
