using MonsterTradingCardGame_2024.Models;
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
                // Check, if the Authorization-Header exists
                if (!rq.Headers.ContainsKey("Authorization") || !rq.Headers["Authorization"].StartsWith("Bearer "))
                {
                    rs.SetClientError("Missing or invalid Authorization header", 400);
                    return true;
                }

                // Token aus dem Authorization-Header extrahieren
                string playerToken = rq.Headers["Authorization"].Substring("Bearer ".Length);

                // Validate input tokens
                if (playerToken == null || string.IsNullOrEmpty(playerToken))
                {
                    rs.SetClientError("Player token is required", 400);
                    return true;
                }

                // Optionaler Bonus aus der Anfrage (falls vorhanden)
                string? bonus = null;
                if (!string.IsNullOrWhiteSpace(rq.Content))
                {
                    var tokens = JsonConvert.DeserializeObject<Dictionary<string, string>>(rq.Content);
                    if (tokens != null && tokens.ContainsKey("Bonus"))
                    {
                        bonus = tokens["Bonus"];
                    }
                }


                // Matchmaking: Check if another player is available in the queue
                if (_battleQueue.TryPairPlayers(playerToken, bonus, out var opponent))
                {
                    if (opponent != null)
                    {
                        try
                        {
                            // Battle starten
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
