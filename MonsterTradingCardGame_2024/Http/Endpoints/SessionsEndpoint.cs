using MonsterTradingCardGame_2024.Business_Logic;
using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Http.Endpoints
{
    internal class SessionsEndpoint : IHttpEndpoint
    {
        public bool HandleRequest(HttpRequest rq, HttpResponse rs)
        {
            if (rq.Method == HttpMethod.POST && rq.Path[1] == "sessions")
            {
                if (string.IsNullOrEmpty(rq.Content))
                {
                    rs.SetClientError("No content provided", 400);
                    return true;
                }

                // Deserialize for login
                var userData = JsonSerializer.Deserialize<User>(rq.Content);
                if (userData == null || string.IsNullOrEmpty(userData.Username) || string.IsNullOrEmpty(userData.Password))
                {
                    rs.SetClientError("Invalid login data provided", 400);
                    return true;
                }

                // Call the UserHandler to log the user in
                string? token = UserHandler.LoginUser(userData.Username, userData.Password);
                if (token != null)
                {
                    rs.SetSuccess("Login successful", 200);
                    rs.Content = JsonSerializer.Serialize(new { Token = token });
                }
                else
                {
                    rs.SetClientError("Login failed", 401); // Unauthorized
                }
                return true;
            }

            return false;
        }
    } // <- End of SessionsEndpoint class
} // <- End of MonsterTradingCardGame_2024.Http.Endpoints namesspace
