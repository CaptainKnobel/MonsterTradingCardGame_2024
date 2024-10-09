using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Http;
using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Business_Logic
{
    internal static class UserHandler
    {
        // Register a new user through the repository
        public static bool RegisterUser(string username, string password)
        {
            return UserRepository.Register(username, password);
        }

        // Validate user credentials for login
        public static string? LoginUser(string username, string password)
        {
            var user = UserRepository.Login(username, password);
            return user?.Token;
        }

        // Find a user by their token
        public static User? FindUserByToken(string token)
        {
            return UserRepository.GetUserByToken(token);
        }

        // Returns the user's deck based on the token
        public static List<Card>? GetDeckByToken(string token)
        {
            var user = FindUserByToken(token);
            return user?.Deck.Cards;  // Returns null if user is not found
        }

        // Returns the user's statistics (ELO, wins, losses)
        public static UserStats? GetStatsByToken(string token)
        {
            var user = FindUserByToken(token);
            return user?.Stats;  // Returns null if user is not found
        }

        // Fetches the scoreboard (list of users ordered by ELO)
        public static List<UserStats> GetScoreboard()
        {
            return UserRepository.GetAllUsers()
                .OrderByDescending(u => u.Stats.Elo)
                .Select(u => u.Stats)
                .ToList();
        }

        // Update a user's ELO score after a battle, along with win/loss records
        public static void UpdateUserElo(string token, int points, bool won)
        {
            var user = FindUserByToken(token);
            if (user != null)
            {
                user.UpdateElo(points, won);
            }
        }
        
    } // <- End of UserHandler class
} // <- End of MonsterTradingCardGame_2024.Business_Logic namesspace
