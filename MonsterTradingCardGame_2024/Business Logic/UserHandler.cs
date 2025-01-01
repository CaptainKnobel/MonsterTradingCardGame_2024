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
    internal class UserHandler
    {
        private readonly IUserRepository _userRepository;
        public UserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Register a new user through the repository
        public bool RegisterUser(string username, string password)
        {
            return _userRepository.Register(username, password);
        }

        // Validate user credentials for login
        public string? LoginUser(string username, string password)
        {
            var user = _userRepository.Login(username, password);
            return user?.Token;
        }

        // Find a user by their token
        public User? FindUserByToken(string token)
        {
            return _userRepository.GetUserByToken(token);
        }

        // Returns the user's deck based on the token
        public List<Card> GetDeckByToken(string token)
        {
            if(token != null)
            {
                var user = FindUserByToken(token);
                if (user != null)
                {
                    // Check if the user's deck exists
                    if (user.Deck != null)
                    {
                        // Return the user's cards
                        return user.Deck.Cards;
                    }
                }
            }
            // Returns an empty Deck if anything went wrong above.
            return new List<Card>();
        }

        // Returns the user's statistics (ELO, wins, losses)
        public UserStats? GetStatsByToken(string token)
        {
            var user = FindUserByToken(token);
            return user?.Stats;  // Returns null if user is not found
        }

        // Fetches the scoreboard (list of users ordered by ELO)
        public List<UserStats> GetScoreboard()
        {
            return _userRepository.GetAllUsers()
                .OrderByDescending(u => u.Stats.Elo)
                .Select(u => u.Stats)
                .ToList();
        }

        // Update a user's ELO score after a battle, along with win/loss records
        public void UpdateUserElo(string token, int points, bool won)
        {
            var user = FindUserByToken(token);
            if (user != null)
            {
                user.Stats.Elo += points;
                if (won)
                {
                    user.Stats.Wins++;
                }
                else
                {
                    user.Stats.Losses++;
                }

                // TODO: save in database
            }
        }

        public void AddCardToDeck(User user, Card card)
        {
            if (user.Deck.Cards.Count < 4)
            {
                user.Deck.AddCard(card);
            }
            else
            {
                throw new InvalidOperationException("The deck can only contain 4 cards.");
            }
        }

        public void RemoveCardFromDeck(User user, Card card)
        {
            user.Deck.RemoveCard(card);
        }

        public void AddCardToStack(User user, Card card)
        {
            user.Stack.AddCard(card);
        }

        public Card DrawCardFromStack(User user)
        {
            return user.Stack.DrawRandomCard();
        }

        public bool SpendCoins(User user, int amount)
        {
            if(user.Coins < 5) // Check if user has enough coins. A package costs 5 coins.
            {
                if (user.Coins >= amount)
                {
                    user.Coins -= amount;
                    return true;
                }
            }
            return false;
        }

    } // <- End of UserHandler class
} // <- End of MonsterTradingCardGame_2024.Business_Logic namesspace
