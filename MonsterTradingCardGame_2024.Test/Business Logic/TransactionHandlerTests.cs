using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame_2024.Enums;
using MonsterTradingCardGame_2024.Models;
using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Business_Logic;
using NSubstitute;
using NUnit;
using NUnit.Framework;

namespace MonsterTradingCardGame_2024.Test.Business_Logic
{
    [TestFixture]
    public class TransactionHandlerTests
    {
        private IUserRepository _userRepository;
        private IPackageRepository _packageRepository;
        private TransactionHandler _transactionHandler;

        [SetUp]
        public void Setup()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _packageRepository = Substitute.For<IPackageRepository>();
            var userHandler = new UserHandler(_userRepository); // No mocking for UserHandler
            _transactionHandler = new TransactionHandler(userHandler, _packageRepository);
        }

        [Test]
        public void BuyPackage_UserNotFound_ReturnsError()
        {
            // Arrange
            var token = "invalid-token";
            _userRepository.GetUserByToken(token).Returns((User)null!);

            // Act
            var result = _transactionHandler.BuyPackage(token);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid authentication token"));
        }

        [Test]
        public void BuyPackage_NotEnoughCoins_ReturnsError()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Coins = 3 };
            var token = "valid-token";
            _userRepository.GetUserByToken(token).Returns(user);

            // Act
            var result = _transactionHandler.BuyPackage(token);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Not enough money to buy the package"));
        }

        [Test]
        public void BuyPackage_NoPackagesAvailable_ReturnsError()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Coins = 10 };
            var token = "valid-token";
            _userRepository.GetUserByToken(token).Returns(user);
            _packageRepository.GetAvailablePackage().Returns((CardPackage)null!);

            // Act
            var result = _transactionHandler.BuyPackage(token);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("No packages available for purchase"));
        }

        [Test]
        public void BuyPackage_SuccessfullyBuysPackage_ReturnsPackage()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Coins = 10, Stack = new CardStack() };
            var token = "valid-token";
            var cards = new List<Card>
            {
                new MonsterCard("Dragon", 50, Element.Fire, Species.Dragon, user.Id),
                new MonsterCard("Goblin", 20, Element.Normal, Species.Goblin, user.Id),
                new SpellCard("Fireball", 40, Element.Fire, user.Id),
                new SpellCard("WaterSplash", 30, Element.Water, user.Id),
                new MonsterCard("Elf", 25, Element.Normal, Species.Elf, user.Id)
            };
            var package = new CardPackage(cards);

            _userRepository.GetUserByToken(token).Returns(user);
            _packageRepository.GetAvailablePackage().Returns(package);

            // Act
            var result = _transactionHandler.BuyPackage(token);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Package, Is.EqualTo(package));
            Assert.That(user.Stack.Cards, Is.EquivalentTo(cards));
        }
    }
}
