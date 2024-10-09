﻿using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonsterTradingCardGame_2024.Data_Access
{
    internal class UserRepository
    {

        // Dummy storage for users (in-memory list)
        private static List<User> users = new List<User>
        {
            //new User(1, "kienboec", "daniel", 20, 100, "kienboec-mtcgToken"),
            //new User(2, "altenhof", "markus", 20, 100, "altenhof-mtcgToken"),
            //new User(3, "admin", "istrator", 20, 100, "admin-mtcgToken")
        };

        // Register a new user
        public static bool Register(string username, string password)
        {
            // Check if the user already exists
            if (users.Any(u => u.Username == username))
            {
                return false;  // User already exists
            }

            // Create new user with a new ID and generate a token
            int newId = users.Max(u => u.Id) + 1; // Explanation:  users ... the list of users // .Max() ... a LINQ (Language Integrated Query) Method to find the largest value // u => u.Id ... a Lambda-expression, here for every user, we call "u", the Id is chosen, which we use for comparison // +1 ... then we add 1 to get a higher Id (so we're counting upwards with every new user) // and then we save that as int newId
            User newUser = new User(newId, username, password, 20, 100, GenerateToken(username));
            users.Add(newUser);
            return true;  // Registration successful
        }

        // Login an existing user
        public static User? Login(string username, string password)
        {
            return users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        // Generate a simple token for the user
        private static string GenerateToken(string username)
        {
            return $"{username}-mtcgToken";
        }

        /* // Alternate version of GenerateToken() that uses a SHA256 zu create a unique token. Doesn't work with the default CURL script.
        private string GenerateToken()
        {
            // Using SHA256 to generate a random token based on the username and a timestamp
            using (var sha256 = SHA256.Create())
            {
                var tokenSource = Username + DateTime.Now.Ticks;
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(tokenSource));
                var token = BitConverter.ToString(bytes).Replace("-", "").ToLower();
                return $"{Username}-mtcgToken-{token.Substring(0, 10)}"; // Example: kienboec-mtcgToken-abcdef1234
            }
        }
        */

    } // <- End of UserRepository class
} // <- End of MonsterTradingCardGame_2024.Data_Access namesspace