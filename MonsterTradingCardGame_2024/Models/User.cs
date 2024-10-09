using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace MonsterTradingCardGame_2024.Models
{
    internal class User
    {
        // ----------========== [Constructors] ==========----------
        // Constructor for registration (username and password)
        public User(string username, string password)
        {
            this.Username = username;
            this.Password = password;
            this.Coins = 20;                // Every user starts with 20 coins
            this.Elo = 100;                 // Start ELO is 100 points
            this.Stack = new List<Card>();  // Collection of all the user's cards (stack)
            this.Deck = new List<Card>();   // Deck (the user's best 4 cards for battle)
        }
        // Constructor for loading a user from the database (with all attributes)
        public User(int id, string username, string password, int coins, int elo, string token)
        {
            this.Id = id;
            this.Username = username;
            this.Password = password;
            this.Coins = coins;
            this.Elo = elo;
            this.Token = token;
            this.Stack = new List<Card>();  // Stack of all cards the user owns
            this.Deck = new List<Card>();   // Best 4 cards selected by the user for battles
        }

        // ----------========== [Properties] ==========----------
        // Unique user ID
        public int Id { get; set; }

        // Username
        public string Username { get; set; }

        // Password (should be hashed)
        public string Password { get; set; }

        // Number of virtual coins the user possesses
        public int Coins { get; set; }

        // ELO score for matchmaking and leaderboard
        public int Elo { get; set; }

        // Collection of all cards the user owns (stack)
        public List<Card> Stack { get; set; }

        // The deck consisting of the 4 best cards for battles
        public List<Card> Deck { get; set; }

        // Token for authentication
        public string? Token { get; set; } = string.Empty;

        // ----------========== [Methods] ==========----------
        // Method to add a card to the deck
        public void AddCardToDeck(Card card)
        {
            if (Deck.Count < 4)
            {
                Deck.Add(card);
            }
            else
            {
                throw new InvalidOperationException("The deck can only contain 4 cards.");
            }
        }

        // Method to add a card to the stack
        public void AddCardToStack(Card card)
        {
            Stack.Add(card);
        }

        // Method to spend coins (e.g. to buy card packages)
        public bool SpendCoins(int amount)
        {
            if (Coins >= amount)
            {
                Coins -= amount;
                return true;  // Successfully spent coins
            }
            return false;  // Not enough coins
        }

        // Method to update the ELO score after a battle
        public void UpdateElo(int points)
        {
            Elo += points;
        }
    } // <- End of User class
} // <- End of MonsterTradingCardGame_2024.Models namesspace
