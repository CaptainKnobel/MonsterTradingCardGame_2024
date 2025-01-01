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
        private readonly IPackageRepository _packageRepository;

        public PackagesEndpoint(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

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

                try
                {
                    // Deserialize the content into a CardPackage object
                    var package = JsonConvert.DeserializeObject<CardPackage>(rq.Content, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto
                    });

                    // Validate the package
                    if (package == null || package.Cards.Count != 5)
                    {
                        rs.SetClientError("A package must contain exactly 5 cards", 400);
                        return true;
                    }

                    // Add the package to the repository
                    var repository = new PackageRepository("YourDatabaseConnectionString");

                    if (repository.AddPackage(package))
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

                return true;
            }
            return false;
        }
    }
}
