using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;

namespace MonsterTradingCardGame_2024.Models
{
    public class User
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
            Bio = string.Empty;
            Image = string.Empty;
        }
        public User(string username, string password)
        {
            this.Username = username;
            this.Password = password;
            this.Coins = 20;                // Every user starts with 20 coins
            this.Stack = new CardStack();   // Collection of all the user's cards (stack)
            this.Deck = new CardDeck();     // Deck (the user's best 4 cards for battle)
            this.Stats = new UserStats();   // Default stats with initial values
            this.Bio = string.Empty;
            this.Image = string.Empty;
        }
        // Constructor for loading a user from the database (with all attributes except image and bio)
        public User(Guid id, string username, string password, int coins, string token, int elo, int wins, int losses)
        {
            this.Id = id;
            this.Username = username;
            this.Password = password;
            this.Coins = coins;
            this.Token = token;
            this.Stack = new CardStack();   // Stack of all cards the user owns
            this.Deck = new CardDeck();     // Best 4 cards selected by the user for battles
            this.Stats = new UserStats(elo, wins, losses);
            this.Bio = string.Empty;
            this.Image = string.Empty;
        }
        // Constructor for loading a user from the database (with all attributes)
        public User(Guid id, string username, string password, int coins, string token, int elo, int wins, int losses, string? bio, string? image)
        {
            this.Id = id;
            this.Username = username;
            this.Password = password;
            this.Coins = coins;
            this.Token = token;
            this.Stack = new CardStack();   // Stack of all cards the user owns
            this.Deck = new CardDeck();     // Best 4 cards selected by the user for battles
            this.Stats = new UserStats(elo, wins, losses);
            this.Bio = bio ?? string.Empty;
            this.Image = image ?? string.Empty;
        }

        // ----------========== [Properties] ==========----------
        // Unique user ID
        public Guid Id { get; set; }

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
        
        // Short biography
        public string? Bio { get; set; }
        
        // Profile image
        public string? Image { get; set; }

    } // <- End of User class
} // <- End of MonsterTradingCardGame_2024.Models namesspace
