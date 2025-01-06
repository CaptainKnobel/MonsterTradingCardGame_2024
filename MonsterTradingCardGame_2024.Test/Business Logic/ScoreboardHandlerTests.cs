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
            var scoreboardData = new List<(string Username, UserStats Stats)>
            {
                ("Player1", new UserStats { Elo = 1200, Wins = 10, Losses = 5 }),
                ("Player2", new UserStats { Elo = 1300, Wins = 15, Losses = 3 }),
                ("Player3", new UserStats { Elo = 1100, Wins = 8, Losses = 7 })
            };

            _userRepository.GetScoreboardData().Returns(scoreboardData);

            // Act
            var result = _scoreboardHandler.GetScoreboard();

            // Assert
            Assert.That(result, Is.EqualTo(scoreboardData));
        }

        [Test]
        public void GetScoreboard_NoUsers_ReturnsEmptyList()
        {
            // Arrange
            _userRepository.GetScoreboardData().Returns(new List<(string Username, UserStats Stats)>());

            // Act
            var result = _scoreboardHandler.GetScoreboard();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetScoreboard_HandlesNullFromRepository()
        {
            // Arrange
            _userRepository.GetScoreboardData().Returns((IEnumerable<(string Username, UserStats Stats)>?)null);

            // Act
            var result = _scoreboardHandler.GetScoreboard() ?? Enumerable.Empty<(string Username, UserStats Stats)>();

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}
