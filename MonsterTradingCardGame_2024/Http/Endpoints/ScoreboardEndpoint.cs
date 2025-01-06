using MonsterTradingCardGame_2024.Business_Logic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Http.Endpoints
{
    internal class ScoreboardEndpoint : IHttpEndpoint
    {
        private readonly ScoreboardHandler _scoreboardHandler;

        public ScoreboardEndpoint(ScoreboardHandler scoreboardHandler)
        {
            _scoreboardHandler = scoreboardHandler;
        }

        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.GET && rq.Path[1] == "scoreboard")
            {
                var scoreboard = _scoreboardHandler.GetScoreboard();

                // Transform tuples into a serializable object
                var response = scoreboard.Select(entry => new
                {
                    Username = entry.Username,
                    Stats = entry.Stats
                });

                rs.SetJsonContentType();
                rs.Content = JsonConvert.SerializeObject(response, Formatting.Indented);
                rs.SetSuccess("Scoreboard retrieved successfully", 200);

                return true;
            }

            return false;   // Unhandled request
        }
    }
}
