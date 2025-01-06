using MonsterTradingCardGame_2024.Models;
using MonsterTradingCardGame_2024.Business_Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MonsterTradingCardGame_2024.Enums;

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
            if (rq.Path.Length > 0 && rq.Path[1] == "tradings")
            {
                switch (rq.Method)
                {
                    case HttpMethod.GET:
                        return rq.Path.Length == 2 ? HandleGetTradingDeals(rq, rs) : HandleGetSingleTradingDeal(rq, rs);
                    case HttpMethod.POST:
                        return rq.Path.Length == 2 ? HandleCreateTradingDeal(rq, rs) : HandleAcceptTradingDeal(rq, rs);
                    case HttpMethod.DELETE:
                        return HandleDeleteTradingDeal(rq, rs);
                }
            }
            return false;
        }

        private bool HandleGetTradingDeals(HttpRequest rq, HttpResponse rs)
        {
            try
            {
                Console.WriteLine("Fetching all trading deals...");
                var deals = _tradingHandler.GetAllTradingDeals();

                // Transformiere die Deals in ein einfaches serialisierbares Format
                var response = deals.Select(deal => new
                {
                    Id = deal.Id,
                    CardToTradeId = deal.CardToTradeId,
                    AcceptedElement = deal.AcceptedElement.ToString(),
                    AcceptedSpecies = deal.AcceptedSpecies.ToString(),
                    MinimumDamage = deal.MinimumDamage,
                    CardDetails = deal.CardToTrade != null ? new
                    {
                        deal.CardToTrade.Id,
                        deal.CardToTrade.Name,
                        deal.CardToTrade.Damage,
                        deal.CardToTrade.ElementType,
                        Type = deal.CardToTrade is MonsterCard monsterCard
                            ? $"Monster ({monsterCard.MonsterSpecies})"
                            : "Spell"
                    } : null
                });

                rs.SetJsonContentType();
                rs.Content = JsonConvert.SerializeObject(response, Formatting.Indented);
                rs.SetSuccess("Trading deals retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                rs.SetServerError($"Failed to retrieve trading deals: {ex.Message}");
            }

            return true;
        }

        private bool HandleGetSingleTradingDeal(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Path.Length < 3 || !Guid.TryParse(rq.Path[2], out var tradingId))
            {
                rs.SetClientError("Invalid trading deal ID format", 400);
                return true;
            }

            try
            {
                var deal = _tradingHandler.GetTradingDealById(tradingId);

                if (deal == null)
                {
                    rs.SetClientError("Trading deal not found", 404);
                    return true;
                }

                // Transformiere das Deal-Objekt in ein einfach serialisierbares Format
                var response = new
                {
                    Id = deal.Id,
                    CardToTradeId = deal.CardToTradeId,
                    AcceptedElement = deal.AcceptedElement.ToString(),
                    AcceptedSpecies = deal.AcceptedSpecies.ToString(),
                    MinimumDamage = deal.MinimumDamage,
                    CardDetails = deal.CardToTrade != null ? new
                    {
                        deal.CardToTrade.Id,
                        deal.CardToTrade.Name,
                        deal.CardToTrade.Damage,
                        deal.CardToTrade.ElementType,
                        Type = deal.CardToTrade is MonsterCard monsterCard
                            ? $"Monster ({monsterCard.MonsterSpecies})"
                            : "Spell"
                    } : null
                };

                rs.SetJsonContentType();
                rs.Content = JsonConvert.SerializeObject(response, Formatting.Indented);
                rs.SetSuccess("Trading deal retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                rs.SetServerError($"Failed to retrieve trading deal: {ex.Message}");
            }

            return true;
        }

        private bool HandleCreateTradingDeal(HttpRequest rq, HttpResponse rs)
        {
            if (string.IsNullOrWhiteSpace(rq.Content))
            {
                rs.SetClientError("Invalid request", 400);
                return true;
            }

            try
            {
                Console.WriteLine($"Attempting to Create Trade Deal...");

                // Parse die Eingabe als Dictionary, um Fehler zu vermeiden
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(rq.Content);
                if (data == null || !data.ContainsKey("Id") || !data.ContainsKey("CardToTrade") || !data.ContainsKey("MinimumDamage"))
                {
                    rs.SetClientError("Invalid JSON format or missing required fields (Id, CardToTrade, MinimumDamage)", 400);
                    return true;
                }

                // Werte extrahieren
                if (!Guid.TryParse(data["Id"].ToString(), out var id))
                {
                    id = Guid.NewGuid(); // Fallback zu einem neuen GUID
                }

                if (!Guid.TryParse(data["CardToTrade"].ToString(), out var cardToTradeId))
                {
                    rs.SetClientError("Invalid CardToTrade GUID format", 400);
                    return true;
                }

                if (!float.TryParse(data["MinimumDamage"].ToString(), out var minimumDamage))
                {
                    rs.SetClientError("Invalid MinimumDamage value", 400);
                    return true;
                }

                // Element und Species validieren
                var acceptedElement = data.ContainsKey("AcceptedElement") && Enum.TryParse(typeof(Element), data["AcceptedElement"].ToString(), out var element)
                    ? (Element)element
                    : Element.Normal;

                var acceptedSpecies = data.ContainsKey("AcceptedSpecies") && Enum.TryParse(typeof(Species), data["AcceptedSpecies"].ToString(), out var species)
                    ? (Species)species
                    : Species.Goblin;

                // Karte laden
                var cardToTrade = _tradingHandler.GetCardById(cardToTradeId);
                if (cardToTrade == null)
                {
                    rs.SetClientError("Card to trade not found", 404);
                    return true;
                }

                // Erstelle das TradingDeal-Objekt
                var deal = new TradingDeal
                {
                    Id = id,
                    CardToTrade = cardToTrade,
                    CardToTradeId = cardToTrade.Id,
                    AcceptedElement = acceptedElement,
                    AcceptedSpecies = acceptedSpecies,
                    MinimumDamage = minimumDamage
                };

                // Trading Deal erstellen
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
            if (rq.Path.Length < 3 || !Guid.TryParse(rq.Path[2], out var tradingId))
            {
                rs.SetClientError("Invalid trading deal ID format", 400);
                return true;
            }

            if (string.IsNullOrWhiteSpace(rq.Content) || !Guid.TryParse(rq.Content.Trim('"'), out var offeredCardId))
            {
                rs.SetClientError("Invalid card format", 400);
                return true;
            }

            try
            {
                if (_tradingHandler.AcceptTradingDeal(tradingId, offeredCardId))
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

        private bool HandleDeleteTradingDeal(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Path.Length < 3 || !Guid.TryParse(rq.Path[2], out var tradingId))
            {
                rs.SetClientError("Invalid trading deal ID format", 400);
                return true;
            }

            try
            {
                _tradingHandler.RemoveTradingDeal(tradingId);
                rs.SetSuccess("Trading deal deleted successfully", 200);
            }
            catch (Exception ex)
            {
                rs.SetServerError($"Failed to delete trading deal: {ex.Message}");
            }

            return true;
        }
    }
}
