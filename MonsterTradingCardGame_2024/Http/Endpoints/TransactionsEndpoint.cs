using MonsterTradingCardGame_2024.Business_Logic;
using MonsterTradingCardGame_2024.Data_Access;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Http.Endpoints
{
    internal class TransactionsEndpoint : IHttpEndpoint
    {
        private readonly TransactionHandler _transactionHandler;

        public TransactionsEndpoint(TransactionHandler transactionHandler)
        {
            _transactionHandler = transactionHandler;
        }

        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            // Check if the path is `/transactions/packages` and method is POST
            if (rq.Method == HttpMethod.POST && rq.Path.Length >= 3 &&
                rq.Path[1] == "transactions" && rq.Path[2] == "packages")
            {
                // Check for valid authorization token
                if (!rq.Headers.ContainsKey("Authorization") || !rq.Headers["Authorization"].StartsWith("Bearer "))
                {
                    rs.SetClientError("Unauthorized - Missing or invalid token", 401);
                    return true;
                }

                string token = rq.Headers["Authorization"].Substring("Bearer ".Length);

                try
                {
                    // Call the Business Logic to handle purchasing a package
                    var (isSuccess, package, errorMessage) = _transactionHandler.BuyPackage(token);

                    if (isSuccess)
                    {
                        rs.SetSuccess(JsonConvert.SerializeObject(new
                        {
                            message = "Package purchased successfully",
                            package = package
                        }, new JsonSerializerSettings { Formatting = Formatting.Indented }), 201);
                    }
                    else
                    {
                        // Differentiate between failure reasons
                        if (errorMessage != null)
                        {
                            rs.SetClientError(errorMessage, 400);
                        }
                        else
                        {
                            rs.SetServerError("An unexpected error occurred");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Catch unexpected errors and return a server error
                    rs.SetServerError($"An unexpected error occurred: {ex.Message}");
                }

                return true;
            }

            return false; // Unhandled request
        }
    }
}
