﻿using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Data_Access
{
    public interface IUserRepository
    {
        bool Register(string username, string password);
        User? Login(string username, string password);
        User? GetUserByToken(string token);
        List<User> GetAllUsers();
        IEnumerable<UserStats> GetScoreboardData();
        void UpdateUser(User user);
    }
}