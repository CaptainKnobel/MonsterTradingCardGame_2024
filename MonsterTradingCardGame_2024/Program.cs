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
                
                // Create the Http Server
                HttpServer server = new HttpServer();

                // Register Endpoints for Http Server
                server.RegisterEndpoint("users", new UsersEndpoint());                  // Registers Endpoint for user registration
                server.RegisterEndpoint("sessions", new SessionsEndpoint());            // Registers Endpoint for user login
                server.RegisterEndpoint("packages", new PackagesEndpoint());            // Registers Endpoint for card packages (creating packages)
                server.RegisterEndpoint("transactions", new TransactionsEndpoint());    // Registers Endpoint for transactions regarding card packages
                server.RegisterEndpoint("cards", new CardsEndpoint());                  // Registers Endpoint for card listing
                server.RegisterEndpoint("deck", new DeckEndpoint());                    // Registers Endpoint for managing the user's deck
                server.RegisterEndpoint("stats", new StatsEndpoint());                  // Registers Endpoint for viewing user statistics
                server.RegisterEndpoint("scoreboard", new ScoreboardEndpoint());        // Registers Endpoint for viewing the scoreboard
                server.RegisterEndpoint("tradings", new TradingsEndpoint());            // Registers Endpoint for trading cards

                // Start the Http Server
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