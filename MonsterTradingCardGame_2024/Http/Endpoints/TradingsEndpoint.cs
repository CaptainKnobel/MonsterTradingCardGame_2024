using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Http.Endpoints
{
    internal class TradingsEndpoint : IHttpEndpoint
    {
        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.GET && rq.Path[1] == "tradings")
            {
                // Handle retrieval of trading deals
                var trades = TradingHandler.GetAllTrades();

                rs.Content = JsonSerializer.Serialize(trades);
                rs.SetSuccess("Trades retrieved successfully", 200);
                return true;
            }
            else if (rq.Method == HttpMethod.POST && rq.Path[1] == "tradings")
            {
                // Handle the creation of a new trading deal
                var newTrade = JsonSerializer.Deserialize<TradingDeal>(rq.Content);
                bool success = TradingHandler.AddTrade(newTrade);

                if (success)
                {
                    rs.SetSuccess("Trade added successfully", 201);
                }
                else
                {
                    rs.SetClientError("Trade creation failed", 400);
                }
                return true;
            }
            else if (rq.Method == HttpMethod.DELETE && rq.Path[1] == "tradings")
            {
                // Handle the deletion of a trading deal
                bool success = TradingHandler.RemoveTrade(rq.Path[2]);

                if (success)
                {
                    rs.SetSuccess("Trade removed successfully", 200);
                }
                else
                {
                    rs.SetClientError("Trade removal failed", 400);
                }
                return true;
            }

            return false;
        }
    }
}
