﻿using MonsterTradingCardGame_2024.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Data_Access
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Generate a simple token for the user
        private string GenerateToken(string username)
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

        // Register a new user
        public bool Register(string username, string password)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Users (Id, Username, Password, Coins, Token, Elo, Wins, Losses)
                VALUES (@Id, @Username, @Password, 20, NULL, 100, 0, 0);
            ";
            var token = GenerateToken(username);
            command.Parameters.AddWithValue("@Id", Guid.NewGuid());
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);
            command.Parameters.AddWithValue("@Token", token);

            try
            {
                command.ExecuteNonQuery();
                return true;
            }
            catch (PostgresException)
            {
                return false;   // Catch duplicate username errors
            }
        }

        public User? Login(string username, string password)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT Id, Username, Password, Coins, Token, Elo, Wins, Losses
                        FROM Users
                        WHERE Username = @username AND Password = @password";
                    command.Parameters.AddWithValue("username", username);
                    command.Parameters.AddWithValue("password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var id = reader.GetGuid(0);
                            var coins = reader.GetInt32(3);
                            var token = reader.IsDBNull(4) ? null : reader.GetString(4);
                            var elo = reader.GetInt32(5);
                            var wins = reader.GetInt32(6);
                            var losses = reader.GetInt32(7);

                            if (token == null || token == string.Empty)
                            {
                                token = GenerateToken(username);

                                using (var updateCommand = connection.CreateCommand())
                                {
                                    updateCommand.CommandText = "UPDATE Users SET Token = @Token WHERE Id = @Id";
                                    updateCommand.Parameters.AddWithValue("Token", token);
                                    updateCommand.Parameters.AddWithValue("Id", id);
                                    updateCommand.ExecuteNonQuery();
                                }
                            }

                            return new User(id, username, password, coins, token ?? string.Empty, elo, wins, losses);
                        }
                    }
                }
            }
            return null;
        }

        // Find a user by their token
        public User? GetUserByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT Id, Username, Password, Coins, Token, Elo, Wins, Losses
                        FROM Users
                        WHERE Token = @token";
                    command.Parameters.AddWithValue("token", token);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var id = reader.GetGuid(0);
                            var username = reader.GetString(1);
                            var password = reader.GetString(2);
                            var coins = reader.GetInt32(3);
                            var elo = reader.GetInt32(5);
                            var wins = reader.GetInt32(6);
                            var losses = reader.GetInt32(7);

                            return new User(id, username, password, coins, token ?? string.Empty, elo, wins, losses);
                        }
                    }
                }
            }
            return null;
        }

        // Fetch all users (for scoreboard purposes)
        public List<User> GetAllUsers()
        {
            var users = new List<User>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT Id, Username, Password, Coins, Token, Elo, Wins, Losses
                        FROM Users
                        ORDER BY Elo DESC, Wins DESC, Losses ASC;";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User(
                                reader.GetGuid(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetInt32(3),
                                reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                                reader.GetInt32(5),
                                reader.GetInt32(6),
                                reader.GetInt32(7)
                            ));
                        }
                    }
                }
            }

            return users;
        }

        public IEnumerable<UserStats> GetScoreboardData()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            // Alle Benutzer abrufen und nach ELO sortieren
            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Elo, Wins, Losses
                FROM Users
                ORDER BY Elo DESC";

            using var reader = command.ExecuteReader();
            var userStats = new List<UserStats>();
            while (reader.Read())
            {
                userStats.Add(new UserStats(
                    reader.GetInt32(0),  // Elo
                    reader.GetInt32(1),  // Wins
                    reader.GetInt32(2)   // Losses
                ));
            }

            return userStats;
        }

        public void UpdateUser(User user)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Users
                SET 
                    Username = @Username,
                    Password = @Password,
                    Coins = @Coins,
                    Token = @Token,
                    Elo = @Elo,
                    Wins = @Wins,
                    Losses = @Losses
                WHERE Id = @Id
            ";
            command.Parameters.AddWithValue("Id", user.Id);
            command.Parameters.AddWithValue("Username", user.Username ?? string.Empty);
            command.Parameters.AddWithValue("Password", user.Password ?? string.Empty);
            command.Parameters.AddWithValue("Coins", Math.Max(0, user.Coins)); // Verhindert negative Coins
            command.Parameters.AddWithValue("Token", user.Token ?? string.Empty);
            command.Parameters.AddWithValue("Elo", user.Stats?.Elo ?? 100);  // Fallback für fehlende Stats
            command.Parameters.AddWithValue("Wins", user.Stats?.Wins ?? 0);
            command.Parameters.AddWithValue("Losses", user.Stats?.Losses ?? 0);

            command.ExecuteNonQuery();
        }
    } // <- End of UserRepository class
} // <- End of MonsterTradingCardGame_2024.Data_Access namesspace
