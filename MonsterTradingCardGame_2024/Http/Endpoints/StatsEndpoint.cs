using MonsterTradingCardGame_2024.Business_Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Http.Endpoints
{
    internal class StatsEndpoint : IHttpEndpoint
    {
        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.GET && rq.Path[1] == "stats")
            {
                // Handle statistics retrieval logic here
                string? token = rq.Headers["Authorization"].Split(' ')[1];
                var stats = UserHandler.GetStatsByToken(token);

                rs.Content = JsonSerializer.Serialize(stats);
                rs.SetSuccess("Stats retrieved successfully", 200);
                return true;
            }

            return false;
        }
    }
}
