using MonsterTradingCardGame_2024.Business_Logic;
using MonsterTradingCardGame_2024.Data_Access;
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
                    rs.SetClientError("Unauthorized - Missing or invalid token", 401);
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
                    // Better error differentiation
                    if (PackageRepository.GetAvailablePackageCount() == 0)
                    {
                        rs.SetClientError("No packages available for purchase", 400);
                    }
                    else
                    {
                        rs.SetClientError("Not enough money to buy the package", 400);
                    }
                }
                return true;
            }

            return false; // Unhandled request
        }
    }
}
