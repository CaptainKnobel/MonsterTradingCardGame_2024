using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Models;
using Npgsql;
using NUnit.Framework;
using NSubstitute;
using MonsterTradingCardGame_2024.Infrastructure.Database;

namespace MonsterTradingCardGame_2024.Test.Data_Access
{
    [TestFixture]
    //[Parallelizable(ParallelScope.None)]
    public class UserRepositoryTests
    {
        private UserRepository _userRepository;
        private string _connectionString;
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;

        [SetUp]
        public void Setup()
        {
            _connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=mtcgdb";
            DatabaseManager.CleanupTables(_connectionString);
            DatabaseManager.InitializeDatabase(_connectionString);
            _connection = new NpgsqlConnection(_connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
            _userRepository = new UserRepository(_connectionString);
        }

        [TearDown]
        public void Teardown()
        {
            try
            {
                _transaction.Rollback();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Error during Transaction Rollback: {ex.Message}");
            }
            finally
            {
                DatabaseManager.CleanupTables(_connectionString);
                _transaction?.Dispose();
                _connection?.Dispose();
            }
        }

        [Test]
        public void Register_ValidUser_ReturnsTrue()
        {
            // Arrange
            var username = "testuser";
            var password = "password123";

            // Act
            var result = _userRepository.Register(username, password);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Register_DuplicateUser_ReturnsFalse()
        {
            // Arrange
            var username = "existinguser";
            var password = "password123";
            _userRepository.Register(username, password); // First registration

            // Act
            var result = _userRepository.Register(username, password); // Duplicate registration

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Login_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var username = "testuser";
            var password = "password123";
            _userRepository.Register(username, password);

            // Act
            var user = _userRepository.Login(username, password);

            // Assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user?.Username, Is.EqualTo(username));
        }

        [Test]
        public void Login_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            var username = "testuser";
            var password = "wrongpassword";

            // Act
            var user = _userRepository.Login(username, password);

            // Assert
            Assert.That(user, Is.Null);
        }

        [Test]
        public void GetUserByToken_ValidToken_ReturnsUser()
        {
            // Arrange
            var username = "testuser";
            var password = "password123";
            var token = "testuser-mtcgToken";
            _userRepository.Register(username, password);

            // Act
            var user = _userRepository.GetUserByToken(token);

            // Assert
            Assert.That(user, Is.Not.Null);
            Assert.That(user?.Token, Is.EqualTo(token));
        }

        [Test]
        public void GetUserByToken_InvalidToken_ReturnsNull()
        {
            // Arrange
            var token = "invalid-token";

            // Act
            var user = _userRepository.GetUserByToken(token);

            // Assert
            Assert.That(user, Is.Null);
        }

        [Test]
        public void GetAllUsers_ReturnsUserList()
        {
            // Arrange
            _userRepository.Register("user1", "pass1");
            _userRepository.Register("user2", "pass2");

            // Act
            var users = _userRepository.GetAllUsers();

            // Assert
            Assert.That(users, Has.Count.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void GetScoreboardData_ReturnsOrderedStats()
        {
            // Arrange
            var user1 = new UserStats { Elo = 1200, Wins = 10, Losses = 5 };
            var user2 = new UserStats { Elo = 1300, Wins = 15, Losses = 3 };
            var user3 = new UserStats { Elo = 1100, Wins = 8, Losses = 7 };

            // Act
            var scoreboard = _userRepository.GetScoreboardData();

            // Assert
            Assert.That(scoreboard, Is.Ordered.By(nameof(UserStats.Elo)).Descending);
        }

        [Test]
        public void UpdateUser_ValidUser_UpdatesSuccessfully()
        {
            // Arrange
            var username = "testuser";
            var password = "password123";
            _userRepository.Register(username, password);
            var user = _userRepository.Login(username, password);

            user!.Coins += 10;
            user.Stats.Elo += 100;

            // Act
            _userRepository.UpdateUser(user);
            var updatedUser = _userRepository.GetUserByToken(user.Token!);

            // Assert
            Assert.That(updatedUser?.Coins, Is.EqualTo(user.Coins));
            Assert.That(updatedUser?.Stats.Elo, Is.EqualTo(user.Stats.Elo));
        }
    }
}
