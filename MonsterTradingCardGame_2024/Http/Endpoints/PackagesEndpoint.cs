using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.JsonConverters;
using MonsterTradingCardGame_2024.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
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
                // Admin should be able to add packages
                if (string.IsNullOrWhiteSpace(rq.Content))
                {
                    rs.SetClientError("Package data is missing or invalid", 400);
                    return true;
                }

                // Create the JsonSerializerOptions and add the converter
                var options = new JsonSerializerOptions();
                options.Converters.Add(new CardConverter());    // Register converter

                // so sollte es funzen <---- ----> TODO: beim serializen -- nur card schicken?
                var options2 = new JsonSerializerSettings();
                options2.TypeNameHandling = TypeNameHandling.Auto;

                var package = JsonConvert.DeserializeObject<CardPackage>(rq.Content);

                if (package != null && PackageRepository.AddPackage(package))
                {
                    rs.SetSuccess("Package created successfully", 201);
                }
                else
                {
                    rs.SetClientError("Failed to create package", 400);
                }

                return true;
            }

            return false;
        }
    }
}
