using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame_2024.Business_Logic;
using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Enums;
using MonsterTradingCardGame_2024.Models;
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
        private UserHandler _userHandler;
        private TransactionHandler _transactionHandler;

        [SetUp]
        public void Setup()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _packageRepository = Substitute.For<IPackageRepository>();
            _userHandler = new UserHandler(_userRepository); // Real UserHandler mit Mocked IUserRepository
            _transactionHandler = new TransactionHandler(_userHandler, _packageRepository);
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
            _packageRepository.GetAvailablePackage().Returns((List<Card>)null!);

            // Act
            var result = _transactionHandler.BuyPackage(token);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("No packages available for purchase"));
        }

        [Test]
        public void BuyPackage_FailedOwnershipTransfer_ReturnsError()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), Coins = 10 };
            var token = "valid-token";
            var cards = new List<Card>
            {
                new MonsterCard("Dragon", 50, Element.Fire, Species.Dragon, Guid.Empty),
                new MonsterCard("Goblin", 20, Element.Normal, Species.Goblin, Guid.Empty)
            };

            _userRepository.GetUserByToken(token).Returns(user);
            _packageRepository.GetAvailablePackage().Returns(cards);
            _packageRepository.TransferOwnership(cards, user.Id).Returns(false);

            // Act
            var result = _transactionHandler.BuyPackage(token);

            // Assert
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Failed to transfer ownership of the package"));
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
                new MonsterCard("Goblin", 20, Element.Normal, Species.Goblin, user.Id)
            };

            _userRepository.GetUserByToken(token).Returns(user);
            _packageRepository.GetAvailablePackage().Returns(cards);
            _packageRepository.TransferOwnership(cards, user.Id).Returns(true);

            // Act
            var result = _transactionHandler.BuyPackage(token);

            // Assert
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.PurchasedCards, Is.EqualTo(cards));
            Assert.That(user.Coins, Is.EqualTo(5)); // Verify coins deduction
        }
    }
}
