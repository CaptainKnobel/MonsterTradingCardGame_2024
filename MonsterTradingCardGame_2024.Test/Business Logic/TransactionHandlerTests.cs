using NUnit;
using NUnit.Framework;
using MonsterTradingCardGame_2024.Business_Logic;
using MonsterTradingCardGame_2024.Models;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame_2024.Enums;

namespace MonsterTradingCardGame_2024.Test.Business_Logic
{/*
    [TestFixture]
    public class TransactionHandlerTests
    {
        private IUserRepository _userRepositoryMock;
        private IPackageRepository _packageRepositoryMock;

        [SetUp]
        public void Setup()
        {
            // Mocks erstellen
            _userRepositoryMock = Substitute.For<IUserRepository>();
            _packageRepositoryMock = Substitute.For<IPackageRepository>();

            // Repositories in Handler injizieren (falls nötig)
            UserHandler.SetUserRepository(_userRepositoryMock);
            PackageRepository.SetPackageRepository(_packageRepositoryMock);
        }

        [Test]
        public void BuyPackage_Success_ReturnsPackage()
        {
            // Arrange
            string validToken = "valid-token";
            var user = new User { Token = validToken, Coins = 10 };

            // Setup Mock für Benutzer
            _userRepositoryMock.GetUserByToken(validToken).Returns(user);

            // Setup Mock für verfügbares Paket
            var mockPackage = new CardPackage(new List<Card>
            {
                new Card("Card 1", 10, Element.Fire, CardType.Monster),
                new Card("Card 2", 20, Element.Water, CardType.Monster),
                new Card("Card 3", 30, Element.Normal, CardType.Spell),
                new Card("Card 4", 40, Element.Fire, CardType.Spell),
                new Card("Card 5", 50, Element.Water, CardType.Monster)
            });
            _packageRepositoryMock.GetAvailablePackage().Returns(mockPackage);

            // Act
            var (isSuccess, package, errorMessage) = TransactionHandler.BuyPackage(validToken);

            // Assert
            Assert.IsTrue(isSuccess, "Transaction should be successful.");
            Assert.NotNull(package, "Package should not be null.");
            Assert.AreEqual(mockPackage, package, "Returned package should match the mock package.");
            Assert.IsNull(errorMessage, "Error message should be null.");
        }

        [Test]
        public void BuyPackage_InsufficientFunds_ReturnsError()
        {
            // Arrange
            string token = "low-funds-token";
            var user = new User { Token = token, Coins = 1 };

            // Setup Mock für Benutzer
            _userRepositoryMock.GetUserByToken(token).Returns(user);

            // Act
            var (isSuccess, package, errorMessage) = TransactionHandler.BuyPackage(token);

            // Assert
            Assert.IsFalse(isSuccess, "Transaction should fail due to insufficient funds.");
            Assert.IsNull(package, "Package should be null.");
            Assert.AreEqual("Not enough money to buy the package", errorMessage, "Error message should indicate insufficient funds.");
        }

        [Test]
        public void BuyPackage_NoPackagesAvailable_ReturnsError()
        {
            // Arrange
            string validToken = "valid-token";
            var user = new User { Token = validToken, Coins = 10 };

            // Setup Mock für Benutzer
            _userRepositoryMock.GetUserByToken(validToken).Returns(user);

            // Setup Mock für kein verfügbares Paket
            _packageRepositoryMock.GetAvailablePackage().Returns((CardPackage?)null);

            // Act
            var (isSuccess, package, errorMessage) = TransactionHandler.BuyPackage(validToken);

            // Assert
            Assert.IsFalse(isSuccess, "Transaction should fail due to no available packages.");
            Assert.IsNull(package, "Package should be null.");
            Assert.AreEqual("No packages available for purchase", errorMessage, "Error message should indicate no available packages.");
        }
    }
    */
}
