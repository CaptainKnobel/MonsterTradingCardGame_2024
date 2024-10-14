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
        public User()
        {
            // Standardwerte setzen, um mögliche Null-Referenz-Probleme zu vermeiden
            Username = string.Empty;
            Password = string.Empty;
            Token = string.Empty;
            Coins = 20;
            Stack = new CardStack();
            Deck = new CardDeck();
            Stats = new UserStats();
        }
        public User(string username, string password)
        {
            this.Username = username;
            this.Password = password;
            this.Coins = 20;                // Every user starts with 20 coins
            this.Stack = new CardStack();   // Collection of all the user's cards (stack)
            this.Deck = new CardDeck();     // Deck (the user's best 4 cards for battle)
            this.Stats = new UserStats();   // Default stats with initial values
        }
        // Constructor for loading a user from the database (with all attributes)
        public User(int id, string username, string password, int coins, string token, int elo, int wins, int losses)
        {
            this.Id = id;
            this.Username = username;
            this.Password = password;
            this.Coins = coins;
            this.Token = token;
            this.Stack = new CardStack();   // Stack of all cards the user owns
            this.Deck = new CardDeck();     // Best 4 cards selected by the user for battles
            this.Stats = new UserStats(elo, wins, losses);
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

        // Token for authentication
        public string? Token { get; set; } = string.Empty;

        // Collection of all cards the user owns (stack)
        public CardStack Stack { get; set; } = new CardStack();

        // The deck consisting of the 4 best cards for battles
        public CardDeck Deck { get; set; } = new CardDeck();

        // Statistics of the player
        public UserStats Stats { get; set; } = new UserStats();

        // ----------========== [Methods] ==========---------- // TODO: funktionen in business logik verschieben
        // Method to add a card to the deck
        public void AddCardToDeck(Card card)
        {
            if (Deck.Cards.Count < 4)
            {
                Deck.AddCard(card);
            }
            else
            {
                throw new InvalidOperationException("The deck can only contain 4 cards.");
            }
        }

        public void RemoveCardFromDeck(Card card)
        {
            Deck.RemoveCard(card);
        }

        // Method to add a card to the stack
        public void AddCardToStack(Card card)
        {
            Stack.AddCard(card);
        }

        public void DrawCardFromStack()
        {
            Card card = Stack.DrawRandomCard();
            // TODO: Mach was mit der gezogenen Karte...
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
        public void UpdateElo(int points, bool won)
        {
            Stats.Elo += points;
            if (won)
            {
                Stats.Wins += 1;
            }
            else
            {
                Stats.Losses += 1;
            }
        }
    } // <- End of User class
} // <- End of MonsterTradingCardGame_2024.Models namesspace
