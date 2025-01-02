using MonsterTradingCardGame_2024.Models;
using MonsterTradingCardGame_2024.Business_Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MonsterTradingCardGame_2024.Http.Endpoints
{
    internal class TradingsEndpoint : IHttpEndpoint
    {
        private readonly TradingHandler _tradingHandler;

        public TradingsEndpoint(TradingHandler tradingHandler)
        {
            _tradingHandler = tradingHandler;
        }

        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.POST && rq.Path[1] == "tradings")
            {
                if (rq.Path.Length == 2) return HandleCreateTradingDeal(rq, rs);
                if (rq.Path.Length == 3) return HandleAcceptTradingDeal(rq, rs);
            }

            return false;
        }

        private bool HandleCreateTradingDeal(HttpRequest rq, HttpResponse rs)
        {
            if (string.IsNullOrWhiteSpace(rq.Content))
            {
                rs.SetClientError("Invalid request", 400);
                return true;
            }

            var deal = JsonConvert.DeserializeObject<TradingDeal>(rq.Content);
            if (deal == null)
            {
                rs.SetClientError("Invalid trading deal format", 400);
                return true;
            }

            try
            {
                _tradingHandler.CreateTradingDeal(deal);
                rs.SetSuccess("Trading deal created successfully", 201);
            }
            catch (Exception ex)
            {
                rs.SetServerError($"Failed to create trading deal: {ex.Message}");
            }

            return true;
        }

        private bool HandleAcceptTradingDeal(HttpRequest rq, HttpResponse rs)
        {
            if (string.IsNullOrWhiteSpace(rq.Content) || rq.Path.Length < 3)
            {
                rs.SetClientError("Invalid request", 400);
                return true;
            }

            var tradingId = rq.Path[2];
            var offeredCard = JsonConvert.DeserializeObject<Card>(rq.Content);

            if (offeredCard == null)
            {
                rs.SetClientError("Invalid card format", 400);
                return true;
            }

            try
            {
                if (_tradingHandler.AcceptTradingDeal(tradingId, offeredCard))
                {
                    rs.SetSuccess("Trading deal accepted", 200);
                }
                else
                {
                    rs.SetClientError("Trading deal conditions not met", 400);
                }
            }
            catch (Exception ex)
            {
                rs.SetServerError($"Failed to accept trading deal: {ex.Message}");
            }

            return true;
        }
    }
}
