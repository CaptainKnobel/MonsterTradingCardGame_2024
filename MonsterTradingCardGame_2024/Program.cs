// See https://aka.ms/new-console-template for more information

using MonsterTradingCardGame_2024.Http.Endpoints;
using MonsterTradingCardGame_2024.Http;
using System;
using System.Net;
using Npgsql;

namespace MonsterTradingCardGame_2024
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program Start ...");
            try
            {
                Console.WriteLine("=*=*=*=[ Monster Trading Card Game ]=*=*=*=");
                HttpServer server = new HttpServer();
                server.Run();
            }
            catch (NpgsqlException ex)
            {
                // Handle PostgreSQL exceptions
                Console.WriteLine("A PostgreSQL exception occurred: " + ex.Message);
            }
            catch (Exception ex)
            {
                // Generic exception handling for any other unexpected errors
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("... Program End");
            }

        } // <- End of Main function
    } // <- End of Program class
} // <- End of MonsterTradingCardGame_2024 namesspace