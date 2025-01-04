using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Models;
using MonsterTradingCardGame_2024.Enums;
using Npgsql;
using NUnit.Framework;

namespace MonsterTradingCardGame_2024.Test.Data_Access
{
    [TestFixture]
    public class DeckRepositoryTests : IDisposable
    {
        private DeckRepository _deckRepository;
        private string _connectionString;
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;

        [SetUp]
        public void Setup()
        {
            _connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=mtcgdb";
            _connection = new NpgsqlConnection(_connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
            _deckRepository = new DeckRepository(_connectionString);
        }

        [TearDown]
        public void Teardown()
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _connection.Dispose();
        }

        [Test]
        public void GetDeckByUserId_ValidUserId_ReturnsDeck()
        {
            // Arrange
            var userId = 1;
            var card1 = new MonsterCard("Dragon", 50, Element.Fire, Species.Dragon) { Id = Guid.NewGuid() };
            var card2 = new SpellCard("Fireball", 40, Element.Fire) { Id = Guid.NewGuid() };
            var deckCardIds = new List<Guid> { card1.Id, card2.Id };

            _deckRepository.UpdateDeck(userId, deckCardIds);

            // Act
            var deck = _deckRepository.GetDeckByUserId(userId);

            // Assert
            Assert.That(deck, Is.Not.Null);
            Assert.That(deck, Has.Exactly(1).Matches<Card>(c => c.Id == card1.Id));
            Assert.That(deck, Has.Exactly(1).Matches<Card>(c => c.Id == card2.Id));
        }

        [Test]
        public void UpdateDeck_ValidDeck_SuccessfullyUpdatesDeck()
        {
            // Arrange
            var userId = 2;
            var card1 = new MonsterCard("Elf", 30, Element.Earth, Species.Elf) { Id = Guid.NewGuid() };
            var card2 = new SpellCard("WaterSplash", 25, Element.Water) { Id = Guid.NewGuid() };
            var newDeckCardIds = new List<Guid> { card1.Id, card2.Id };

            // Act
            var updateResult = _deckRepository.UpdateDeck(userId, newDeckCardIds);

            // Assert
            Assert.That(updateResult, Is.True);
            var updatedDeck = _deckRepository.GetDeckByUserId(userId);
            Assert.That(updatedDeck, Has.Exactly(1).Matches<Card>(c => c.Id == card1.Id));
            Assert.That(updatedDeck, Has.Exactly(1).Matches<Card>(c => c.Id == card2.Id));
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}
