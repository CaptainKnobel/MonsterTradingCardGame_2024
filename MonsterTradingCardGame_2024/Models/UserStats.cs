using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Models
{
    public class UserStats
    {
        // Constructor for new User's Stats
        public UserStats()
        {
            Elo = 100;  // Start ELO is 100 points
            Wins = 0;
            Losses = 0;
        }
        // Constructor for User's Stats loaded from Database
        public UserStats(int Elo, int Wins, int Losses)
        {
            this.Elo = Elo;
            this.Wins = Wins;
            this.Losses = Losses;
        }
        // ELO score for matchmaking and leaderboard
        public int Elo { get; set; }
        // Number of won matches
        public int Wins { get; set; }
        // Number of lost matches
        public int Losses { get; set; }
    } // <- End of UserStats class
} // <- End of MonsterTradingCardGame_2024.Models namesspace
