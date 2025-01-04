using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame_2024.Business_Logic;
using MonsterTradingCardGame_2024.Models;
using MonsterTradingCardGame_2024.Data_Access;
using NSubstitute;
using NUnit.Framework;

namespace MonsterTradingCardGame_2024.Test.Business_Logic
{
    [TestFixture]
    public class DeckHandlerTests
    {
        private IDeckRepository _deckRepository;
        private ICardRepository _cardRepository;
        private DeckHandler _deckHandler;

        [SetUp]
        public void Setup()
        {
            _deckRepository = Substitute.For<IDeckRepository>();
            _cardRepository = Substitute.For<ICardRepository>();
            _deckHandler = new DeckHandler(_deckRepository, _cardRepository);
        }

        [Test]
        public void GetDeckByUserId_ValidUserId_ReturnsDeck()
        {
            // Arrange
            var userId = 1;
            var expectedDeck = new List<Card>
            {
                new MonsterCard("Dragon", 50, Enums.Element.Fire, Enums.Species.Dragon, userId),
                new SpellCard("Fireball", 40, Enums.Element.Fire, userId),
                new MonsterCard("Elf", 25, Enums.Element.Normal, Enums.Species.Elf, userId),
                new SpellCard("WaterSplash", 30, Enums.Element.Water, userId)
            };

            _deckRepository.GetDeckByUserId(userId).Returns(expectedDeck);

            // Act
            var result = _deckHandler.GetDeckByUserId(userId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedDeck));
        }

        [Test]
        public void GetDeckByUserId_InvalidUserId_ReturnsEmptyDeck()
        {
            // Arrange
            var userId = 99;
            _deckRepository.GetDeckByUserId(userId).Returns(new List<Card>());

            // Act
            var result = _deckHandler.GetDeckByUserId(userId);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void ValidateDeck_ValidDeck_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var cardIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var userCards = new List<Card>
            {
                new MonsterCard("Dragon", 50, Enums.Element.Fire, Enums.Species.Dragon, userId) { Id = cardIds[0] },
                new SpellCard("Fireball", 40, Enums.Element.Fire, userId) { Id = cardIds[1] },
                new MonsterCard("Elf", 25, Enums.Element.Normal, Enums.Species.Elf, userId) { Id = cardIds[2] },
                new SpellCard("WaterSplash", 30, Enums.Element.Water, userId) { Id = cardIds[3] }
            };

            _cardRepository.GetCardsByUserId(userId).Returns(userCards);

            // Act
            var result = _deckHandler.ValidateDeck(cardIds, userId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void ValidateDeck_InvalidDeckSize_ReturnsFalse()
        {
            // Arrange
            var userId = 1;
            var cardIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }; // Only 2 cards

            // Act
            var result = _deckHandler.ValidateDeck(cardIds, userId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ValidateDeck_InvalidOwnership_ReturnsFalse()
        {
            // Arrange
            var userId = 1;
            var cardIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var userCards = new List<Card>
            {
                new MonsterCard("Dragon", 50, Enums.Element.Fire, Enums.Species.Dragon, userId) { Id = Guid.NewGuid() },
                new SpellCard("Fireball", 40, Enums.Element.Fire, userId) { Id = Guid.NewGuid() }
            };

            _cardRepository.GetCardsByUserId(userId).Returns(userCards);

            // Act
            var result = _deckHandler.ValidateDeck(cardIds, userId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void UpdateDeck_ValidDeck_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var cardIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            var userCards = new List<Card>
            {
                new MonsterCard("Dragon", 50, Enums.Element.Fire, Enums.Species.Dragon, userId) { Id = cardIds[0] },
                new SpellCard("Fireball", 40, Enums.Element.Fire, userId) { Id = cardIds[1] },
                new MonsterCard("Elf", 25, Enums.Element.Normal, Enums.Species.Elf, userId) { Id = cardIds[2] },
                new SpellCard("WaterSplash", 30, Enums.Element.Water, userId) { Id = cardIds[3] }
            };

            _cardRepository.GetCardsByUserId(userId).Returns(userCards);
            _deckRepository.UpdateDeck(userId, cardIds).Returns(true);

            // Act
            var result = _deckHandler.UpdateDeck(userId, cardIds);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void UpdateDeck_InvalidDeck_ThrowsException()
        {
            // Arrange
            var userId = 1;
            var cardIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            _deckRepository.UpdateDeck(userId, cardIds).Returns(false);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _deckHandler.UpdateDeck(userId, cardIds));
        }
    }
}
