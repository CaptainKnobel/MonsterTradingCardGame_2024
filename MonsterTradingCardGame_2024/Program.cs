using MonsterTradingCardGame_2024.Http.Endpoints;
using MonsterTradingCardGame_2024.Http;
using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Business_Logic;
using MonsterTradingCardGame_2024.Infrastructure;
using System;
using System.Net;
using System.Threading;
using Npgsql;
using MonsterTradingCardGame_2024.Infrastructure.Database;

namespace MonsterTradingCardGame_2024
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program Start ...");

            // Initialize Connection String for Database Connection
            string connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=mtcgdb";
            // Initialize the Http Server
            HttpServer? server = null;

            try
            {
                Console.WriteLine("=*=*=*=[ Monster Trading Card Game ]=*=*=*=");

                // Initialize Database / -Connection
                //#if DEBUG
                Console.WriteLine("Cleaning up Database...");
                DatabaseManager.CleanupTables(connectionString);
                //#endif
                Console.WriteLine("Initializing Database...");
                DatabaseManager.InitializeDatabase(connectionString);

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
                PackageHandler packageHandler = new PackageHandler(packageRepository);
                TransactionHandler transactionHandler = new TransactionHandler(userHandler, packageRepository);
                CardHandler cardHandler = new CardHandler(cardRepository);
                DeckHandler deckHandler = new DeckHandler(deckRepository, cardRepository);
                ScoreboardHandler scoreboardHandler = new ScoreboardHandler(userRepository);
                TradingHandler tradingHandler = new TradingHandler(tradingRepository, cardRepository);
                BattleHandler battleHandler = new BattleHandler(userRepository, deckRepository);

                // Setup the Http Server
                Console.WriteLine("Setting up server...");
                server = new HttpServer();
                var serverThread = new Thread(() => server.Run());

                // Register Endpoints for Http Server
                server.RegisterEndpoint("users", new UsersEndpoint(userHandler));                       // Registers Endpoint for user registration
                server.RegisterEndpoint("sessions", new SessionsEndpoint(userHandler));                 // Registers Endpoint for user login
                server.RegisterEndpoint("packages", new PackagesEndpoint(packageHandler));              // Registers Endpoint for card packages (creating packages)
                server.RegisterEndpoint("transactions", new TransactionsEndpoint(transactionHandler));  // Registers Endpoint for transactions regarding card packages
                server.RegisterEndpoint("cards", new CardsEndpoint(userHandler, cardHandler));          // Registers Endpoint for card listing
                server.RegisterEndpoint("deck", new DeckEndpoint(userHandler, deckHandler));            // Registers Endpoint for managing the user's deck
                server.RegisterEndpoint("stats", new StatsEndpoint(userHandler));                       // Registers Endpoint for viewing user statistics
                server.RegisterEndpoint("scoreboard", new ScoreboardEndpoint(scoreboardHandler));       // Registers Endpoint for viewing the scoreboard
                server.RegisterEndpoint("tradings", new TradingsEndpoint(tradingHandler));              // Registers Endpoint for trading cards
                server.RegisterEndpoint("battles", new BattleEndpoint(battleHandler, battleQueue));     // Registers Endpoint for battles between players

                // Start the Http Server
                serverThread.Start();

                // Open shut down option for Http Server
                Console.WriteLine("Press 'q' to stop the server...");
                while (Console.ReadKey(true).Key != ConsoleKey.Q)
                {
                    Thread.Sleep(100); // Avoids unnecessary Resource usage.
                }
                server.Stop();
                serverThread.Join();
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
                // Cleanup
                Console.WriteLine("Stopping server...");
                server?.Stop();

                //#if DEBUG
                Console.WriteLine("Cleaning up Database...");
                DatabaseManager.CleanupTables(connectionString);
                //#endif

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
 * docker start mtcgdb
 * 
 * stoppen:
 * docker stop postgresdb 
 * docker start mtcgdb
 * 
 */
//-----
/* 
 * Verbindung zur Datenbank prüfen:
 * psql -h localhost -U postgres -p 5432
 * 
 * Container löschen (optional):
 * docker rm postgresdb
 * 
 * Daten löschen (optional, entfernt auch persistente Daten):
 * docker volume rm pgdata
 */
