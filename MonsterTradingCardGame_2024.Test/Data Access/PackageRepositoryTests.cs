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
    public class PackageRepositoryTests : IDisposable
    {
        private PackageRepository _packageRepository;
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
            _packageRepository = new PackageRepository(_connectionString);
        }

        [TearDown]
        public void Teardown()
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _connection.Dispose();
        }

        [Test]
        public void AddPackage_ValidPackage_SuccessfullyAddsPackage()
        {
            // Arrange
            var package = new CardPackage(new List<Card>
            {
                new MonsterCard("Dragon", 50, Element.Fire, Species.Dragon),
                new SpellCard("Fireball", 40, Element.Fire),
                new MonsterCard("Elf", 30, Element.Earth, Species.Elf),
                new SpellCard("WaterSplash", 25, Element.Water),
                new MonsterCard("Goblin", 20, Element.Normal, Species.Goblin)
            });

            // Act
            Assert.DoesNotThrow(() => _packageRepository.AddPackage(package));

            // Assert
            var retrievedPackage = _packageRepository.GetAvailablePackage();
            Assert.That(retrievedPackage, Is.Not.Null);
            Assert.That(retrievedPackage?.Cards, Has.Count.EqualTo(5));
        }

        [Test]
        public void GetAvailablePackage_WhenAvailable_ReturnsPackage()
        {
            // Arrange
            var package = new CardPackage(new List<Card>
            {
                new MonsterCard("Goblin", 25, Element.Earth, Species.Goblin),
                new SpellCard("WaterSplash", 35, Element.Water),
                new MonsterCard("Elf", 30, Element.Earth, Species.Elf),
                new SpellCard("Fireball", 50, Element.Fire),
                new MonsterCard("Dragon", 50, Element.Fire, Species.Dragon)
            });
            _packageRepository.AddPackage(package);

            // Act
            var availablePackage = _packageRepository.GetAvailablePackage();

            // Assert
            Assert.That(availablePackage, Is.Not.Null);
            Assert.That(availablePackage?.Cards, Has.Count.EqualTo(5));
        }

        [Test]
        public void RemovePackage_ValidPackage_RemovesPackage()
        {
            // Arrange
            var package = new CardPackage(new List<Card>
            {
                new MonsterCard("Goblin", 20, Element.Normal, Species.Goblin),
                new SpellCard("Fireball", 50, Element.Fire),
                new MonsterCard("Elf", 30, Element.Earth, Species.Elf),
                new SpellCard("WaterSplash", 25, Element.Water),
                new MonsterCard("Dragon", 50, Element.Fire, Species.Dragon)
            });
            _packageRepository.AddPackage(package);

            // Act
            var retrievedPackage = _packageRepository.GetAvailablePackage();
            Assert.That(retrievedPackage, Is.Not.Null); // Ensure package exists
            var packageId = retrievedPackage.Cards.First().Id;
            _packageRepository.DeletePackageById(packageId.GetHashCode()); // Delete by ID

            // Assert
            var afterDeletion = _packageRepository.GetAvailablePackage();
            Assert.That(afterDeletion, Is.Null);
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}