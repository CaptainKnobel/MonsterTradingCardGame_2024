using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.JsonConverters;
using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Http.Endpoints
{
    internal class PackagesEndpoint : IHttpEndpoint
    {
        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.POST && rq.Path[1] == "packages")
            {
                /*
                // Admin should be able to add packages
                if (string.IsNullOrWhiteSpace(rq.Content))
                {
                    rs.SetClientError("Package data is missing or invalid", 400);
                    return true;
                }

                // Create the JsonSerializerOptions and add the converter
                var options = new JsonSerializerOptions();
                options.Converters.Add(new CardConverter());    // Register converter

                try
                {
                    // Deserialize the content into a list of Card objects
                    var cards = JsonSerializer.Deserialize<List<Card>>(rq.Content, options);

                    if (cards == null || cards.Count != 5)  // Ensure exactly 5 cards
                    {
                        rs.SetClientError("A package must contain exactly 5 cards", 400);
                        return true;
                    }

                    // Create a new CardPackage with the list of cards
                    var package = new CardPackage(cards);

                    // Add the package to the repository
                    if (PackageRepository.AddPackage(package))
                    {
                        rs.SetSuccess("Package created successfully", 201);
                    }
                    else
                    {
                        rs.SetClientError("Failed to create package", 400);
                    }
                }
                catch (JsonException ex)
                {
                    rs.SetClientError($"Failed to deserialize package: {ex.Message}", 400);
                }
                */
                return true;
            }

            return false;
        }
    }
}
