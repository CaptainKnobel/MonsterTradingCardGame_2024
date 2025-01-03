using MonsterTradingCardGame_2024.Services.Business_Logic;
using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonsterTradingCardGame_2024.Business_Logic.Services;

namespace MonsterTradingCardGame_2024.Business_Logic
{
    public class BattleHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IDeckRepository _deckRepository;
        private readonly BattleService _battleService;
        private readonly BonusService _bonusService;

        public BattleHandler(IUserRepository userRepository, IDeckRepository deckRepository)
        {
            _userRepository = userRepository;
            _deckRepository = deckRepository;
            _battleService = new BattleService();
            _bonusService = new BonusService();
        }

        public (string Winner, string Loser) StartBattle(string player1Token, string player2Token, string? bonus1 = null, string? bonus2 = null)
        {
            var battleLog = new List<string>(); // Store round logs

            try
            {
                var player1 = _userRepository.GetUserByToken(player1Token);
                var player2 = _userRepository.GetUserByToken(player2Token);

                if (player1 == null || player2 == null)
                {
                    throw new InvalidOperationException("One or both players are invalid.");
                }

                var deck1 = new CardDeck { Cards = _deckRepository.GetDeckByUserId(player1.Id).ToList() };
                var deck2 = new CardDeck { Cards = _deckRepository.GetDeckByUserId(player2.Id).ToList() };

                if (deck1.Cards.Count != 4 || deck2.Cards.Count != 4)
                {
                    throw new InvalidOperationException("Both players must have exactly 4 cards in their decks.");
                }

                // Apply bonuses if specified
                if (bonus1 != null)
                {
                    battleLog.Add($"Player 1 Bonus: {bonus1}");
                    _bonusService.ApplyBonus(bonus1, deck1);
                }
                if (bonus2 != null)
                {
                    battleLog.Add($"Player 2 Bonus: {bonus2}");
                    _bonusService.ApplyBonus(bonus2, deck2);
                }

                int score1 = 0, score2 = 0;

                for (int round = 1; round <= 4; round++)
                {
                    var card1 = deck1.DrawRandomCard();
                    var card2 = deck2.DrawRandomCard();

                    int result = _battleService.DetermineWinner(card1, card2);
                    if (result > 0)
                    {
                        score1++;
                        battleLog.Add($"Round {round}: {card1.Name} (Player 1) defeated {card2.Name} (Player 2).");
                    }
                    else if (result < 0)
                    {
                        score2++;
                        battleLog.Add($"Round {round}: {card2.Name} (Player 2) defeated {card1.Name} (Player 1).");
                    }
                    else
                    {
                        battleLog.Add($"Round {round}: {card1.Name} (Player 1) tied with {card2.Name} (Player 2).");
                    }
                }

                string winner, loser;
                if (score1 > score2)
                {
                    winner = player1.Username;
                    loser = player2.Username;
                    UpdateStats(player1, player2, true);
                }
                else if (score2 > score1)
                {
                    winner = player2.Username;
                    loser = player1.Username;
                    UpdateStats(player2, player1, true);
                }
                else
                {
                    winner = "Draw";
                    loser = "Draw";
                    UpdateStats(player1, player2, false);
                }

                // Log the battle summary
                battleLog.Add($"Battle Result: Winner - {winner}, Loser - {loser}");

                // Optionally: Save battleLog to a database or return it for logging
                Console.WriteLine(string.Join(Environment.NewLine, battleLog));

                return (winner, loser);
            }
            catch(Exception ex)
            {
                battleLog.Add($"Error: {ex.Message}"); // Log the exception for debugging
                Console.WriteLine(string.Join(Environment.NewLine, battleLog));
                throw;  // Re-throw
            }
        }

        private void UpdateStats(User winner, User loser, bool hasWinner)
        {
            const int eloWinChange = 3;
            const int eloLossChange = -5;
            const int eloDrawChange = 1;

            if (hasWinner)
            {
                winner.Stats.Wins++;
                winner.Stats.Elo += eloWinChange;

                loser.Stats.Losses++;
                loser.Stats.Elo += eloLossChange;
            }
            else
            {
                // On draw, both players gain a smaller ELO increase
                winner.Stats.Elo += eloDrawChange;
                loser.Stats.Elo += eloDrawChange;
            }

            _userRepository.UpdateUser(winner);
            _userRepository.UpdateUser(loser);
        }

    }
}
