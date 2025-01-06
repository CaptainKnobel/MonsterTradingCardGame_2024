using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Business_Logic
{
    public class ScoreboardHandler
    {
        private readonly IUserRepository _userRepository;

        public ScoreboardHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IEnumerable<(string Username, UserStats Stats)> GetScoreboard()
        {
            return _userRepository.GetScoreboardData();
        }
    }
}
