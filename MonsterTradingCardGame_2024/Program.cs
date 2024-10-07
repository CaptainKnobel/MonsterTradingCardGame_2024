// See https://aka.ms/new-console-template for more information

using System;
using System.Net;
using Npgsql;
using MonsterTradingCardGame_2024.Http;

namespace MonsterTradingCardGame_2024
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpServer server = new HttpServer();
            server.Run();
        }
    }
}