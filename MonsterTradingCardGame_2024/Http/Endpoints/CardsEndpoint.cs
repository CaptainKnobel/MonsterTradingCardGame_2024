using MonsterTradingCardGame_2024.Business_Logic;
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
        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.POST && rq.Path[1] == "packages")
            {
                // TODO: Implementiere das Erstellen eines Pakets...
                // TODO: Implementiere das Kaufen eines Pakets...
                rs.SetSuccess("Package bought successfully", 201);
                return true;
            }
            else if (rq.Method == HttpMethod.GET && rq.Path[1] == "cards")
            {
                // TODO: Implementiere das Anzeigen der Karten im Stack...
                rs.Content = JsonSerializer.Serialize(new { message = "List of cards..." });
                rs.SetSuccess("Cards retrieved successfully", 200);
                return true;
            }
            return false;
        }
    }
}
