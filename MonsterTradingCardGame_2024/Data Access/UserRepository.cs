using MonsterTradingCardGame_2024.Models;
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
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO Users (Username, Password, Token)
                        VALUES (@username, @password, @token)";
                    command.Parameters.AddWithValue("username", username);
                    command.Parameters.AddWithValue("password", password);
                    command.Parameters.AddWithValue("token", GenerateToken(username));

                    try
                    {
                        command.ExecuteNonQuery();
                        return true;
                    }
                    catch (PostgresException)
                    {
                        return false;   // In case the User already exists, catch the error and return false.
                    }
                }
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
                            return new User(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetInt32(3),
                                reader.GetString(4),
                                reader.GetInt32(5),
                                reader.GetInt32(6),
                                reader.GetInt32(7)
                            );
                        }
                    }
                }
            }
            return null;
        }

        // Find a user by their token
        public User? GetUserByToken(string token)
        {
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
                            return new User(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetInt32(3),
                                reader.GetString(4),
                                reader.GetInt32(5),
                                reader.GetInt32(6),
                                reader.GetInt32(7)
                            );
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
                        FROM Users";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User(
                                reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetInt32(3),
                                reader.GetString(4),
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

    } // <- End of UserRepository class
} // <- End of MonsterTradingCardGame_2024.Data_Access namesspace
