using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Infrastructure
{
    public class BattleQueue
    {
        private readonly ConcurrentQueue<(string Token, string? Bonus)> _playerQueue = new();

        public bool TryPairPlayers(string token, string? bonus, out (string Token, string? Bonus)? opponent)
        {
            if (_playerQueue.TryDequeue(out var dequeuedOpponent))
            {
                opponent = dequeuedOpponent;
                return true;    // Opponent found
            }

            _playerQueue.Enqueue((token, bonus));
            opponent = null;    // No opponent available
            return false;
        }
    }
}
