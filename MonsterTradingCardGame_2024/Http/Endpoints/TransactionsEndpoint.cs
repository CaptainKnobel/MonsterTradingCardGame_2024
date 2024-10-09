using MonsterTradingCardGame_2024.Business_Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Http.Endpoints
{
    internal class TransactionsEndpoint : IHttpEndpoint
    {
        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            // Check if the path is `/transactions/packages` and method is POST
            if (rq.Method == HttpMethod.POST && rq.Path[1] == "transactions" && rq.Path[2] == "packages")
            {
                // Check for valid authorization token
                if (!rq.Headers.ContainsKey("Authorization") || !rq.Headers["Authorization"].StartsWith("Bearer "))
                {
                    rs.SetClientError("Unauthorized", 401);
                    return true;
                }

                string token = rq.Headers["Authorization"].Substring("Bearer ".Length);

                // Call the Business Logic to handle purchasing a package
                bool success = TransactionHandler.BuyPackage(token);

                if (success)
                {
                    rs.SetSuccess("Package purchased successfully", 201);
                }
                else
                {
                    rs.SetClientError("Not enough money or no packages available", 400);
                }
                return true;
            }

            return false; // Unhandled request
        }
    }
}
