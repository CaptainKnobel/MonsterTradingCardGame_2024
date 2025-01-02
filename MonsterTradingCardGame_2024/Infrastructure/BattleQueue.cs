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
        private readonly ConcurrentQueue<string> _playerQueue = new();

        public bool TryPairPlayers(string token, out string? opponent)
        {
            if (_playerQueue.TryDequeue(out opponent))
            {
                return true;    // Opponent found
            }

            _playerQueue.Enqueue(token);
            opponent = null;    // No opponent available
            return false;
        }
    }
}
