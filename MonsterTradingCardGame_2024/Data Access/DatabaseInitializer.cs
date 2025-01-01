using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Data_Access
{
    internal static class DatabaseInitializer
    {
        public static void InitializeDatabase(string connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            string dbName = builder.Database;

            builder.Remove("Database");
            string baseConnectionString = builder.ToString();

            using (var connection = new NpgsqlConnection(baseConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"DROP DATABASE IF EXISTS {dbName} WITH (FORCE);";
                    command.ExecuteNonQuery();

                    command.CommandText = $"CREATE DATABASE {dbName};";
                    command.ExecuteNonQuery();
                }
            }

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Users (
                            Id SERIAL PRIMARY KEY,
                            Username VARCHAR(50) NOT NULL UNIQUE,
                            Password VARCHAR(50) NOT NULL,
                            Coins INT NOT NULL DEFAULT 20,
                            Token VARCHAR(100) UNIQUE,
                            Elo INT NOT NULL DEFAULT 100,
                            Wins INT NOT NULL DEFAULT 0,
                            Losses INT NOT NULL DEFAULT 0
                        );

                        CREATE TABLE IF NOT EXISTS Packages (
                            Id SERIAL PRIMARY KEY,
                            CardIds UUID[] NOT NULL  -- Array von Card-IDs
                        );

                        CREATE TABLE IF NOT EXISTS Cards (
                            Id UUID PRIMARY KEY,
                            Name VARCHAR(50) NOT NULL,
                            Damage FLOAT NOT NULL,
                            ElementType INT NOT NULL,
                            CardType INT NOT NULL,
                            OwnerId INT NOT NULL REFERENCES Users(Id)
                        );
                    ";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
