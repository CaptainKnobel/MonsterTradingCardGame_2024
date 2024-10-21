using MonsterTradingCardGame_2024.Data_Access;
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
    internal class PackagesEndpoint : IHttpEndpoint
    {
        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.POST)
            {
                // Admin should be able to add packages
                if (string.IsNullOrWhiteSpace(rq.Content))
                {
                    rs.SetClientError("Package data is missing or invalid", 400);
                    return true;
                }

                // Create the JsonSerializerOptions and add the converter
                var options = new JsonSerializerSettings();
                options.TypeNameHandling = TypeNameHandling.Auto;

                try
                {
                    // Deserialize the content into a list of Card objects
                    var package = JsonConvert.DeserializeObject<CardPackage>(rq.Content);

                    if (package == null)  // Ensure exactly 5 cards
                    {
                        rs.SetClientError("Failed to create package", 400);
                        return true;
                    }
                    if(package.Cards.Count != 5)
                    {
                        rs.SetClientError("A package must contain exactly 5 cards", 400);
                        return true;
                    }

                    // Add the package to the repository
                    if (PackageRepository.AddPackage(package))
                    {
                        rs.SetSuccess("Package created successfully", 201);
                    }
                }
                catch (Newtonsoft.Json.JsonException ex)
                {
                    rs.SetClientError($"Failed to deserialize package: {ex.Message}", 400);
                }
                return true;
            }
            return false;
        }
    }
}
