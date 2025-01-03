using MonsterTradingCardGame_2024.Business_Logic;
using MonsterTradingCardGame_2024.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Http.Endpoints
{
    internal class BattleEndpoint : IHttpEndpoint
    {
        private readonly BattleHandler _battleHandler;
        private readonly BattleQueue _battleQueue;

        public BattleEndpoint(BattleHandler battleHandler, BattleQueue battleQueue)
        {
            _battleHandler = battleHandler;
            _battleQueue = battleQueue;
        }

        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.POST && rq.Path[1] == "battles")
            {
                if (string.IsNullOrWhiteSpace(rq.Content))
                {
                    rs.SetClientError("Invalid request", 400);
                    return true;
                }

                // Deserialize JSON content directly into a dictionary
                var tokens = JsonConvert.DeserializeObject<Dictionary<string, string>>(rq.Content);

                // Validate input tokens
                if (tokens == null || !tokens.ContainsKey("PlayerToken") || string.IsNullOrEmpty(tokens["PlayerToken"]))
                {
                    rs.SetClientError("Player token is required", 400);
                    return true;
                }

                var playerToken = tokens["PlayerToken"];
                var bonus = tokens.ContainsKey("Bonus") ? tokens["Bonus"] : null;

                // Matchmaking: Check if another player is available in the queue
                if (_battleQueue.TryPairPlayers(playerToken, bonus, out var opponent))
                {
                    if (opponent != null)
                    {
                        try
                        {
                            // Retrieve opponent bonus if applicable
                            var opponentBonus = tokens.ContainsKey("OpponentBonus") ? tokens["OpponentBonus"] : null;

                            // Start the battle
                            var (winner, loser) = _battleHandler.StartBattle(playerToken, opponent.Value.Token, bonus, opponent.Value.Bonus);
                            rs.SetJsonContentType();
                            rs.Content = JsonConvert.SerializeObject(new { Winner = winner, Loser = loser }, Formatting.Indented);
                            rs.SetSuccess("Battle completed", 200);
                        }
                        catch (InvalidOperationException ex)
                        {
                            rs.SetClientError(ex.Message, 400);
                        }
                    }
                    else
                    {
                        rs.SetClientError("Opponent token is invalid.", 500);
                    }
                }
                else
                {
                    // If no opponent is available, the player waits
                    rs.SetSuccess("Waiting for opponent...", 202);
                }

                return true;
            }

            return false;
        }
    }
}
