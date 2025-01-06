using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame_2024.Infrastructure.Database;
using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Models;
using MonsterTradingCardGame_2024.Enums;
using Npgsql;
using NUnit;
using NUnit.Framework;
using NSubstitute;

namespace MonsterTradingCardGame_2024.Test.Data_Access
{
    [TestFixture]
    public class CardRepositoryTests
    {
        private CardRepository _cardRepository;
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
            _cardRepository = new CardRepository(_connectionString);
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

        private User CreateTestUser(string username, string password)
        {
            var isRegistered = _userRepository.Register(username, password);
            Assert.That(isRegistered, Is.True, "User registration failed");
            var user = _userRepository.Login(username, password);
            Assert.That(user, Is.Not.Null, "User login failed");
            return user!;
        }

        [Test]
        public void AddCard_ValidCard_SuccessfullyAddsCard()
        {
            var user = CreateTestUser("testuser", "password123");
            var card = new MonsterCard("Dragon", 50, Element.Fire, Species.Dragon, user.Id)
            {
                Id = Guid.NewGuid(),
                Locked = false
            };

            Assert.DoesNotThrow(() => _cardRepository.AddCard(card));

            var retrievedCard = _cardRepository.GetCardById(card.Id);
            Assert.That(retrievedCard, Is.Not.Null);
            Assert.That(retrievedCard?.Name, Is.EqualTo(card.Name));
            Assert.That(retrievedCard?.Locked, Is.False);
        }

        [Test]
        public void AddCard_SpellCard_SuccessfullyAddsCard()
        {
            var user = CreateTestUser("testuser4", "password123");
            var card = new SpellCard("Fireball", 40, Element.Fire, user.Id)
            {
                Id = Guid.NewGuid(),
                Locked = true
            };

            Assert.DoesNotThrow(() => _cardRepository.AddCard(card));

            var retrievedCard = _cardRepository.GetCardById(card.Id);
            Assert.That(retrievedCard, Is.Not.Null);
            Assert.That(retrievedCard?.Name, Is.EqualTo(card.Name));
            Assert.That(retrievedCard?.Locked, Is.True);
        }

        [Test]
        public void GetCardById_ValidId_ReturnsCorrectCard()
        {
            var user = CreateTestUser("testuser2", "password123");
            var card = new MonsterCard("Elf", 30, Element.Water, Species.Elf, user.Id)
            {
                Id = Guid.NewGuid(),
                Locked = false
            };
            _cardRepository.AddCard(card);

            var retrievedCard = _cardRepository.GetCardById(card.Id);

            Assert.That(retrievedCard, Is.Not.Null);
            Assert.That(retrievedCard?.Id, Is.EqualTo(card.Id));
            Assert.That(retrievedCard?.Name, Is.EqualTo(card.Name));
        }

        [Test]
        public void GetCardById_MonsterCard_ReturnsCorrectCard()
        {
            // Arrange
            var user = CreateTestUser("testuser5", "password123");
            var card = new MonsterCard("Ork", 60, Element.Earth, Species.Ork, user.Id)
            {
                Id = Guid.NewGuid(),
                Locked = true
            };
            _cardRepository.AddCard(card);

            // Act
            var retrievedCard = _cardRepository.GetCardById(card.Id);

            // Assert
            Assert.That(retrievedCard, Is.Not.Null, "Retrieved card should not be null");
            Assert.That(retrievedCard, Is.InstanceOf<MonsterCard>(), "Retrieved card should be a MonsterCard");
            Assert.That(retrievedCard?.Id, Is.EqualTo(card.Id), "Card ID should match");
            Assert.That(retrievedCard?.Name, Is.EqualTo(card.Name), "Card Name should match");
            Assert.That(retrievedCard?.Damage, Is.EqualTo(card.Damage), "Card Damage should match");
            Assert.That(retrievedCard?.ElementType, Is.EqualTo(card.ElementType), "Card ElementType should match");
            Assert.That(((MonsterCard)retrievedCard!).MonsterSpecies, Is.EqualTo(Species.Ork), "MonsterSpecies should match");
            Assert.That(retrievedCard?.Locked, Is.EqualTo(card.Locked), "Locked status should match");
        }

        [Test]
        public void GetCardsByUserId_ValidUserId_ReturnsCards()
        {
            var user = CreateTestUser("testuser3", "password123");
            var card1 = new MonsterCard("Goblin", 20, Element.Earth, Species.Goblin, user.Id)
            {
                Id = Guid.NewGuid(),
                Locked = false
            };
            var card2 = new MonsterCard("Elf", 40, Element.Water, Species.Elf, user.Id)
            {
                Id = Guid.NewGuid(),
                Locked = true
            };
            _cardRepository.AddCard(card1);
            _cardRepository.AddCard(card2);

            var userCards = _cardRepository.GetCardsByUserId(user.Id);

            Assert.That(userCards.Count, Is.EqualTo(2));
            Assert.That(userCards, Has.Exactly(1).Matches<Card>(c => c.Id == card1.Id));
            Assert.That(userCards, Has.Exactly(1).Matches<Card>(c => c.Id == card2.Id));
        }

        [Test]
        public void GetCardsByUserId_ReturnsAllCardTypes()
        {
            var user = CreateTestUser("testuser6", "password123");
            var card1 = new MonsterCard("Dragon", 50, Element.Fire, Species.Dragon, user.Id)
            {
                Id = Guid.NewGuid(),
                Locked = false
            };
            var card2 = new SpellCard("Fireball", 40, Element.Fire, user.Id)
            {
                Id = Guid.NewGuid(),
                Locked = true
            };
            _cardRepository.AddCard(card1);
            _cardRepository.AddCard(card2);

            var userCards = _cardRepository.GetCardsByUserId(user.Id);

            Assert.That(userCards.Count, Is.EqualTo(2));
            Assert.That(userCards, Has.Exactly(1).Matches<Card>(c => c is MonsterCard && c.Id == card1.Id));
            Assert.That(userCards, Has.Exactly(1).Matches<Card>(c => c is SpellCard && c.Id == card2.Id));
        }

        [Test]
        public void GetCardById_LockedCard_ReturnsCardWithCorrectLockStatus()
        {
            var user = CreateTestUser("testuser8", "password123");
            var card = new SpellCard("Shield", 25, Element.Water, user.Id)
            {
                Id = Guid.NewGuid(),
                Locked = true
            };
            _cardRepository.AddCard(card);

            var retrievedCard = _cardRepository.GetCardById(card.Id);

            Assert.That(retrievedCard, Is.Not.Null);
            Assert.That(retrievedCard?.Locked, Is.True);
        }
    }
}
