using MonsterTradingCardGame_2024.Http;
using MonsterTradingCardGame_2024.Business_Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using MonsterTradingCardGame_2024.Models;

namespace MonsterTradingCardGame_2024.Http.Endpoints
{
    internal class UsersEndpoint : IHttpEndpoint
    {
        // Handles HTTP POST requests for registering users
        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.POST && rq.Path[1] == "users")
            {
                if (string.IsNullOrEmpty(rq.Content))
                {
                    rs.SetClientError("No content provided", 400);
                    return true;
                }

                // Deserialize the JSON content into a User object
                var userData = JsonSerializer.Deserialize<User>(rq.Content);
                if (userData == null || string.IsNullOrEmpty(userData.Username) || string.IsNullOrEmpty(userData.Password))
                {
                    rs.SetClientError("Invalid data provided", 400);
                    return true;
                }

                // Call the UserHandler to register the user
                bool registrationSuccess = UserHandler.RegisterUser(userData.Username, userData.Password);
                if (registrationSuccess)
                {
                    rs.SetSuccess("User created", 201);
                }
                else
                {
                    rs.SetClientError("User already exists", 409); // Conflict
                }
                return true;
            }

            return false; // Unhandled request
        }
    } // <- End of UsersEndpoint class
} // <- End of MonsterTradingCardGame_2024.Http.Endpoints namesspace
