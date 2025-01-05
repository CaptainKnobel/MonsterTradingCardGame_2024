using MonsterTradingCardGame_2024.Http;
using MonsterTradingCardGame_2024.Models;
using MonsterTradingCardGame_2024.Business_Logic;
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
        private readonly PackageHandler _packageHandler;

        public PackagesEndpoint(PackageHandler packageHandler)
        {
            _packageHandler = packageHandler;
        }

        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.POST)
            {
                // Admin should be able to add packages
                // Verify if the request is authorized
                var authHeader = rq.Headers["Authorization"];
                if (authHeader == null || authHeader != "Bearer admin-mtcgToken")
                {
                    rs.SetClientError("Unauthorized: Only admin can create packages", 403);
                    return true;
                }

                if (string.IsNullOrWhiteSpace(rq.Content))
                {
                    rs.SetClientError("Package data is missing or invalid", 400);
                    return true;
                }

                try
                {
                    // Deserialize the content into a CardPackage object
                    var package = JsonConvert.DeserializeObject<List<Card>>(rq.Content, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.None,
                        Converters = { new JsonConversion.CardConverter() }
                    });

                    // Validate the package
                    if (package == null || package.Count != 5)
                    {
                        rs.SetClientError("A package must contain exactly 5 cards", 400);
                        return true;
                    }

                    // Add the package to the repository via the PackageHandler
                    bool success = _packageHandler.CreatePackage(package);

                    if (success)
                    {
                        rs.SetSuccess("Package created successfully", 201);
                    }
                    else
                    {
                        rs.SetServerError("Failed to save the package");
                    }
                }
                catch (Newtonsoft.Json.JsonException ex)
                {
                    // Catch deserialization errors
                    rs.SetClientError($"Failed to deserialize package: {ex.Message}", 400);
                }
                catch (Exception ex)
                {
                    // Catch unexpected errors
                    rs.SetServerError($"An unexpected error occurred: {ex.Message}");
                }

                return true;    // Success!
            }
            return false;   // Unhandled request
        }
    }
}
