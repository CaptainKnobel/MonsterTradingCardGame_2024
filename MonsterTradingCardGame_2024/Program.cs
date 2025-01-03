using MonsterTradingCardGame_2024.Http.Endpoints;
using MonsterTradingCardGame_2024.Http;
using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Services.Business_Logic;
using MonsterTradingCardGame_2024.Business_Logic;
using System;
using System.Net;
using Npgsql;
using MonsterTradingCardGame_2024.Infrastructure;

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

                // Initialize Database Connection
                string connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=mtcgdb";

                // Initialize Infrastructure
                BattleQueue battleQueue = new BattleQueue();

                // Create Repositories
                IUserRepository userRepository = new UserRepository(connectionString);
                IPackageRepository packageRepository = new PackageRepository(connectionString);
                ICardRepository cardRepository = new CardRepository(connectionString);
                IDeckRepository deckRepository = new DeckRepository(connectionString);
                ITradingRepository tradingRepository = new TradingRepository(connectionString);

                // Initialize Handlers
                UserHandler userHandler = new UserHandler(userRepository);
                TransactionHandler transactionHandler = new TransactionHandler(userHandler, packageRepository);
                CardHandler cardHandler = new CardHandler(cardRepository);
                DeckHandler deckHandler = new DeckHandler(deckRepository, cardRepository);
                ScoreboardHandler scoreboardHandler = new ScoreboardHandler(userRepository);
                TradingHandler tradingHandler = new TradingHandler(tradingRepository, cardRepository);
                BattleHandler battleHandler = new BattleHandler(userRepository, deckRepository);

                // Create the Http Server
                HttpServer server = new HttpServer();

                // Register Endpoints for Http Server
                server.RegisterEndpoint("users", new UsersEndpoint(userHandler));                       // Registers Endpoint for user registration
                server.RegisterEndpoint("sessions", new SessionsEndpoint(userHandler));                 // Registers Endpoint for user login
                server.RegisterEndpoint("packages", new PackagesEndpoint(packageRepository));           // Registers Endpoint for card packages (creating packages)
                server.RegisterEndpoint("transactions", new TransactionsEndpoint(transactionHandler));  // Registers Endpoint for transactions regarding card packages
                server.RegisterEndpoint("cards", new CardsEndpoint(userHandler, cardHandler));          // Registers Endpoint for card listing
                server.RegisterEndpoint("deck", new DeckEndpoint(userHandler, deckHandler));            // Registers Endpoint for managing the user's deck
                server.RegisterEndpoint("stats", new StatsEndpoint());                                  // Registers Endpoint for viewing user statistics
                server.RegisterEndpoint("scoreboard", new ScoreboardEndpoint(scoreboardHandler));       // Registers Endpoint for viewing the scoreboard
                server.RegisterEndpoint("tradings", new TradingsEndpoint(tradingHandler));              // Registers Endpoint for trading cards
                server.RegisterEndpoint("battles", new BattleEndpoint(battleHandler, battleQueue));                  // Registers Endpoint for battles between players

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


/* 
 * Docker container erstellen (first time):
 * docker run -d --rm --name postgresdb -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -v pgdata:/var/lib/postgresql/data postgres
 * oder besser:
 * docker run --name mtcgdb -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -v pgdata:/var/lib/postgresql/data postgres
 * weil -d blockiert die konsole, -rm löscht die datenbank danach, deswegen ist das nicht drin
 * 
 * Danach:
 * docker start postgresdb
 * 
 * stoppen:
 * docker stop postgresdb 
 * 
 */