using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Business_Logic;
using MonsterTradingCardGame_2024.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Http.Endpoints
{
    internal class CardsEndpoint : IHttpEndpoint
    {
        private readonly UserHandler _userHandler;
        private readonly CardHandler _cardHandler;
        public CardsEndpoint(UserHandler userHandler, CardHandler cardHandler)
        {
            _userHandler = userHandler;
            _cardHandler = cardHandler;
        }

        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.GET)
            {
                // Validierung des Tokens
                if (!rq.Headers.ContainsKey("Authorization") || !rq.Headers["Authorization"].StartsWith("Bearer "))
                {
                    rs.SetClientError("Unauthorized - Missing or invalid token", 401);
                    return true;
                }

                string token = rq.Headers["Authorization"].Substring("Bearer ".Length);

                try
                {
                    // Nutzer anhand des Tokens finden
                    var user = _userHandler.FindUserByToken(token);
                    if (user == null)
                    {
                        rs.SetClientError("Unauthorized - Invalid token", 401);
                        return true;
                    }

                    // Karten des Nutzers abrufen
                    var cards = _cardHandler.GetUserCards(user.Id);
                    Console.WriteLine($"User {user.Id} has {cards.Count()} cards.");
                    foreach (var card in cards)
                    {
                        Console.WriteLine($"Card: {card.Id}, Name: {card.Name}, OwnerId: {card.OwnerId}");
                    }

                    // Karten als JSON zurückgeben
                    var response = new
                    {
                        message = "Cards retrieved successfully",
                        cards = cards.Select(card => new
                        {
                            card.Id,
                            card.Name,
                            card.Damage,
                            card.ElementType,
                            card.CardType,
                            card.OwnerId,
                            card.Locked
                        }).ToList()
                    };

                    rs.SetSuccess(JsonConvert.SerializeObject(response, new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented
                    }), 201);
                }
                catch (Exception ex)
                {
                    rs.SetServerError($"An unexpected error occurred: {ex.Message}");
                }

                return true;
            }

            return false; // Methode nicht unterstützt
        }
    }
}
