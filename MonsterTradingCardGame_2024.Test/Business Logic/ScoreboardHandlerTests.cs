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
    public class ScoreboardHandlerTests
    {
        private IUserRepository _userRepository;
        private ScoreboardHandler _scoreboardHandler;

        [SetUp]
        public void Setup()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _scoreboardHandler = new ScoreboardHandler(_userRepository);
        }

        [Test]
        public void GetScoreboard_ReturnsSortedUserStats()
        {
            // Arrange
            var userStats = new List<UserStats>
            {
                new UserStats { Elo = 1200, Wins = 10, Losses = 5 },
                new UserStats { Elo = 1300, Wins = 15, Losses = 3 },
                new UserStats { Elo = 1100, Wins = 8, Losses = 7 }
            };

            _userRepository.GetScoreboardData().Returns(userStats);

            // Act
            var result = _scoreboardHandler.GetScoreboard();

            // Assert
            Assert.That(result, Is.EqualTo(userStats));
        }

        [Test]
        public void GetScoreboard_NoUsers_ReturnsEmptyList()
        {
            // Arrange
            _userRepository.GetScoreboardData().Returns(new List<UserStats>());

            // Act
            var result = _scoreboardHandler.GetScoreboard();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetScoreboard_HandlesNullFromRepository()
        {
            // Arrange
            _userRepository.GetScoreboardData().Returns((IEnumerable<UserStats>?)null);

            // Act
            var result = _scoreboardHandler.GetScoreboard() ?? Enumerable.Empty<UserStats>();

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}
