using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Http;
using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Business_Logic
{
    internal static class UserHandler
    {
        // Register a new user through the repository
        public static bool RegisterUser(string username, string password)
        {
            return UserRepository.Register(username, password);
        }

        // Validate user credentials for login
        public static string? LoginUser(string username, string password)
        {
            var user = UserRepository.Login(username, password);
            return user?.Token;
        }
    } // <- End of UserHandler class
} // <- End of MonsterTradingCardGame_2024.Business_Logic namesspace
