using MonsterTradingCardGame_2024.Business_Logic;
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
        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.GET && rq.Path[1] == "scoreboard")
            {
                // Handle scoreboard retrieval logic here
                var scoreboard = UserHandler.GetScoreboard();

                rs.Content = JsonSerializer.Serialize(scoreboard);
                rs.SetSuccess("Scoreboard retrieved successfully", 200);
                return true;
            }

            return false;
        }
    }
}
