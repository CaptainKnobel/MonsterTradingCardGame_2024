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
    public class TradingRepositoryTests : IDisposable
    {
        private TradingRepository _tradingRepository;
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
            _tradingRepository = new TradingRepository(_connectionString);
        }

        [TearDown]
        public void Teardown()
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _connection.Dispose();
        }

        [Test]
        public void AddTradingDeal_ValidDeal_SuccessfullyAddsDeal()
        {
            // Arrange
            var card = new MonsterCard("Dragon", 50, Element.Fire, Species.Dragon) { Id = Guid.NewGuid(), Locked = false };
            var deal = new TradingDeal(Guid.NewGuid().ToString(), card, Element.Water, Species.Elf, 30);

            // Act
            Assert.DoesNotThrow(() => _tradingRepository.AddTradingDeal(deal));

            // Assert
            var retrievedDeal = _tradingRepository.GetTradingDealById(deal.Id);
            Assert.That(retrievedDeal, Is.Not.Null);
            Assert.That(retrievedDeal?.CardToTrade?.Id, Is.EqualTo(card.Id));
        }

        [Test]
        public void GetTradingDealById_ValidId_ReturnsCorrectDeal()
        {
            // Arrange
            var card = new MonsterCard("Elf", 30, Element.Earth, Species.Elf) { Id = Guid.NewGuid(), Locked = false };
            var deal = new TradingDeal(Guid.NewGuid().ToString(), card, Element.Fire, Species.Dragon, 20);
            _tradingRepository.AddTradingDeal(deal);

            // Act
            var retrievedDeal = _tradingRepository.GetTradingDealById(deal.Id);

            // Assert
            Assert.That(retrievedDeal, Is.Not.Null);
            Assert.That(retrievedDeal?.Id, Is.EqualTo(deal.Id));
            Assert.That(retrievedDeal?.AcceptedElement, Is.EqualTo(deal.AcceptedElement));
        }

        [Test]
        public void RemoveTradingDeal_ValidId_RemovesDeal()
        {
            // Arrange
            var card = new MonsterCard("Goblin", 25, Element.Fire, Species.Goblin) { Id = Guid.NewGuid(), Locked = false };
            var deal = new TradingDeal(Guid.NewGuid().ToString(), card, Element.Water, Species.Dragon, 15);
            _tradingRepository.AddTradingDeal(deal);

            // Act
            _tradingRepository.RemoveTradingDeal(deal.Id);

            // Assert
            var retrievedDeal = _tradingRepository.GetTradingDealById(deal.Id);
            Assert.That(retrievedDeal, Is.Null);
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}