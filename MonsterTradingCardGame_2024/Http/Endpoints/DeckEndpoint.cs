using MonsterTradingCardGame_2024.Business_Logic;
using MonsterTradingCardGame_2024.Data_Access;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Http.Endpoints
{
    internal class DeckEndpoint : IHttpEndpoint
    {
        private readonly UserHandler _userHandler;
        private readonly DeckHandler _deckHandler;
        public DeckEndpoint(UserHandler userHandler, DeckHandler deckHandler)
        {
            _userHandler = userHandler;
            _deckHandler = deckHandler;
        }

        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.GET && rq.Path[1] == "deck")
            {
                return HandleGetRequest(rq, rs);
            }
            if (rq.Method == HttpMethod.PUT && rq.Path[1] == "deck")
            {
                return HandlePutRequest(rq, rs);
            }

            return false;
        }

        private bool HandleGetRequest(HttpRequest rq, HttpResponse rs)
        {
            if (!rq.Headers.ContainsKey("Authorization") || !rq.Headers["Authorization"].StartsWith("Bearer "))
            {
                rs.SetClientError("Unauthorized - Missing or invalid token", 401);
                return true;
            }

            string token = rq.Headers["Authorization"].Substring("Bearer ".Length);

            var user = _userHandler.FindUserByToken(token);
            if (user == null)
            {
                rs.SetClientError("Unauthorized - Invalid token", 401);
                return true;
            }

            var deck = _deckHandler.GetDeckByUserId(user.Id);

            rs.SetJsonContentType();
            rs.Content = JsonConvert.SerializeObject(deck, Formatting.Indented);
            rs.SetSuccess("Deck retrieved successfully", 200);

            return true;
        }

        private bool HandlePutRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq == null || string.IsNullOrWhiteSpace(rq.Content))
            {
                rs.SetClientError("Invalid request", 400);
                return true;
            }

            if (!rq.Headers.ContainsKey("Authorization") || !rq.Headers["Authorization"].StartsWith("Bearer "))
            {
                rs.SetClientError("Unauthorized - Missing or invalid token", 401);
                return true;
            }

            string token = rq.Headers["Authorization"].Substring("Bearer ".Length);

            var user = _userHandler.FindUserByToken(token);
            if (user == null)
            {
                rs.SetClientError("Unauthorized - Invalid token", 401);
                return true;
            }

            var cardIds = JsonConvert.DeserializeObject<IEnumerable<Guid>>(rq.Content);
            if (cardIds == null || cardIds.Count() != 4)
            {
                rs.SetClientError("Deck must contain exactly 4 valid card IDs", 400);
                return true;
            }

            try
            {
                if (_deckHandler.UpdateDeck(user.Id, cardIds))
                {
                    rs.SetSuccess("Deck updated successfully", 200);
                }
                else
                {
                    rs.SetServerError("Failed to update deck");
                }
            }
            catch (InvalidOperationException ex)
            {
                rs.SetClientError(ex.Message, 400);
            }

            return true;
        }
    }
}
