using MonsterTradingCardGame_2024.Business_Logic;
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
        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.GET && rq.Path[1] == "deck")
            {
                // Handle deck retrieval logic here
                // Retrieve user's deck and return it as a JSON response
                string? token = rq.Headers["Authorization"].Split(' ')[1]; // Example logic to extract token
                var deck = UserHandler.GetDeckByToken(token);

                rs.Content = JsonSerializer.Serialize(deck);
                rs.SetSuccess("Deck retrieved successfully", 200);
                return true;
            }
            else if (rq.Method == HttpMethod.PUT && rq.Path[1] == "deck")
            {
                // Handle deck update logic here
                rs.SetSuccess("Deck updated successfully", 200);
                return true;
            }

            return false;
        }
    }
}
