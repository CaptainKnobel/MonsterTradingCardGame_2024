using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Infrastructure.Database
{
    internal static class DatabaseManager
    {
        public static void InitializeDatabase(string connectionString)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            CREATE TABLE IF NOT EXISTS Users (
                                Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                Username VARCHAR(50) NOT NULL UNIQUE,
                                Password VARCHAR(50) NOT NULL,
                                Coins INT NOT NULL DEFAULT 20,
                                Token VARCHAR(100) UNIQUE,
                                Elo INT NOT NULL DEFAULT 100,
                                Wins INT NOT NULL DEFAULT 0,
                                Losses INT NOT NULL DEFAULT 0
                            );

                            CREATE TABLE IF NOT EXISTS Cards (
                                Id UUID PRIMARY KEY,
                                Name VARCHAR(50) NOT NULL,
                                Damage FLOAT NOT NULL,
                                ElementType INT NOT NULL,
                                Species INT DEFAULT 0,
                                CardType INT NOT NULL,
                                OwnerId UUID NOT NULL REFERENCES Users(Id),
                                Locked BOOLEAN DEFAULT FALSE
                            );

                            CREATE TABLE IF NOT EXISTS Packages (
                                Id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
                                CardIds UUID[] NOT NULL
                            );

                            CREATE TABLE IF NOT EXISTS Decks (
                                UserId UUID PRIMARY KEY REFERENCES Users(Id),
                                CardIds UUID[] NOT NULL
                            );

                            CREATE TABLE IF NOT EXISTS TradingDeals (
                                Id UUID PRIMARY KEY,
                                CardToTradeId UUID NOT NULL REFERENCES Cards(Id),
                                AcceptedElement INT NOT NULL,
                                AcceptedSpecies INT NOT NULL,
                                MinimumDamage FLOAT NOT NULL
                            );
                        ";
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Warning: Unable to initialize database. Reason: {ex.Message}");
                // Only logging to allow for dry testing.
            }
        }

        public static void CleanupTables(string connectionString)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            DO $$
                            BEGIN
                                IF EXISTS (SELECT FROM pg_tables WHERE tablename = 'cards') THEN
                                    EXECUTE 'TRUNCATE TABLE Cards RESTART IDENTITY CASCADE';
                                END IF;
                                IF EXISTS (SELECT FROM pg_tables WHERE tablename = 'users') THEN
                                    EXECUTE 'TRUNCATE TABLE Users RESTART IDENTITY CASCADE';
                                END IF;
                                IF EXISTS (SELECT FROM pg_tables WHERE tablename = 'packages') THEN
                                    EXECUTE 'TRUNCATE TABLE Packages RESTART IDENTITY CASCADE';
                                END IF;
                                IF EXISTS (SELECT FROM pg_tables WHERE tablename = 'decks') THEN
                                    EXECUTE 'TRUNCATE TABLE Decks RESTART IDENTITY CASCADE';
                                END IF;
                                IF EXISTS (SELECT FROM pg_tables WHERE tablename = 'tradingdeals') THEN
                                    EXECUTE 'TRUNCATE TABLE TradingDeals RESTART IDENTITY CASCADE';
                                END IF;
                            END $$;
                        ";
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Unable to clean up database tables. Reason: {ex.Message}");
                // Only logging to allow for dry testing.
            }
        }
    }
}
