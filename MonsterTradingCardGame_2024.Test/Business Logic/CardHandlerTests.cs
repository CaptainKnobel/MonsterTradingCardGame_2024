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
    public class CardHandlerTests
    {
        private ICardRepository _cardRepository;
        private CardHandler _cardHandler;

        [SetUp]
        public void Setup()
        {
            _cardRepository = Substitute.For<ICardRepository>();
            _cardHandler = new CardHandler(_cardRepository);
        }

        [Test]
        public void GetUserCards_ValidUserId_ReturnsCards()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedCards = new List<Card>
            {
                new MonsterCard("Dragon", 50, Enums.Element.Fire, Enums.Species.Dragon, userId),
                new SpellCard("Fireball", 40, Enums.Element.Fire, userId)
            };

            _cardRepository.GetCardsByUserId(userId).Returns(expectedCards);

            // Act
            var result = _cardHandler.GetUserCards(userId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedCards));
        }

        [Test]
        public void GetUserCards_InvalidUserId_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _cardRepository.GetCardsByUserId(userId).Returns(new List<Card>());

            // Act
            var result = _cardHandler.GetUserCards(userId);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetCard_ValidCardId_ReturnsCard()
        {
            // Arrange
            var cardId = Guid.NewGuid();
            var expectedCard = new MonsterCard("Dragon", 50, Enums.Element.Fire, Enums.Species.Dragon, Guid.NewGuid());

            _cardRepository.GetCardById(cardId).Returns(expectedCard);

            // Act
            var result = _cardHandler.GetCard(cardId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedCard));
        }

        [Test]
        public void GetCard_InvalidCardId_ReturnsNull()
        {
            // Arrange
            var cardId = Guid.NewGuid();
            _cardRepository.GetCardById(cardId).Returns((Card)null!);

            // Act
            var result = _cardHandler.GetCard(cardId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void AddCard_ValidCard_ReturnsTrue()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var card = new SpellCard("Fireball", 40, Enums.Element.Fire, ownerId);

            _cardRepository.AddCard(card).Returns(true);

            // Act
            var result = _cardHandler.AddCard(card);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void AddCard_InvalidCard_ReturnsFalse()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var card = new SpellCard("Fireball", 40, Enums.Element.Fire, ownerId);

            _cardRepository.AddCard(card).Returns(false);

            // Act
            var result = _cardHandler.AddCard(card);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
