﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame_2024.Infrastructure.Database;
using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Models;
using MonsterTradingCardGame_2024.Enums;
using Npgsql;
using NUnit.Framework;

namespace MonsterTradingCardGame_2024.Test.Data_Access
{
    [TestFixture]
    public class CardRepositoryTests
    {
        private CardRepository _cardRepository;
        private string _connectionString;

        [SetUp]
        public void Setup()
        {
            _connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=mtcgdb";

            // Ensure database is ready for testing
            DatabaseManager.CleanupTables(_connectionString);
            DatabaseManager.InitializeDatabase(_connectionString);

            _cardRepository = new CardRepository(_connectionString);
        }

        [TearDown]
        public void Teardown()
        {
            // Clean up tables after each test
            DatabaseManager.CleanupTables(_connectionString);
        }

        [Test]
        public void AddCard_ValidCard_SuccessfullyAddsCard()
        {
            // Arrange
            var card = new MonsterCard("Dragon", 50, Element.Fire, Species.Dragon) { Id = Guid.NewGuid() };

            // Act
            Assert.DoesNotThrow(() => _cardRepository.AddCard(card));

            // Assert
            var retrievedCard = _cardRepository.GetCardById(card.Id);
            Assert.That(retrievedCard, Is.Not.Null);
            Assert.That(retrievedCard?.Name, Is.EqualTo(card.Name));
        }

        [Test]
        public void GetCardById_ValidId_ReturnsCorrectCard()
        {
            // Arrange
            var card = new MonsterCard("Elf", 30, Element.Water, Species.Elf) { Id = Guid.NewGuid() };
            _cardRepository.AddCard(card);

            // Act
            var retrievedCard = _cardRepository.GetCardById(card.Id);

            // Assert
            Assert.That(retrievedCard, Is.Not.Null);
            Assert.That(retrievedCard?.Id, Is.EqualTo(card.Id));
            Assert.That(retrievedCard?.ElementType, Is.EqualTo(card.ElementType));
        }

        [Test]
        public void GetCardsByUserId_ValidUserId_ReturnsCards()
        {
            // Arrange
            var userId = 1;
            var card1 = new MonsterCard("Goblin", 20, Element.Earth, Species.Goblin) { Id = Guid.NewGuid(), OwnerId = userId };
            var card2 = new MonsterCard("Elf", 40, Element.Water, Species.Elf) { Id = Guid.NewGuid(), OwnerId = userId };
            _cardRepository.AddCard(card1);
            _cardRepository.AddCard(card2);

            // Act
            var userCards = _cardRepository.GetCardsByUserId(userId);

            // Assert
            Assert.That(userCards.Count, Is.EqualTo(2));
            Assert.That(userCards, Has.Exactly(1).Matches<Card>(c => c.Id == card1.Id));
            Assert.That(userCards, Has.Exactly(1).Matches<Card>(c => c.Id == card2.Id));
        }
    }
}
