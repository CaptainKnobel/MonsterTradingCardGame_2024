using MonsterTradingCardGame_2024.Enums;
using MonsterTradingCardGame_2024.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Data_Access
{
    public class TradingRepository : ITradingRepository
    {
        private readonly string _connectionString;

        public TradingRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddTradingDeal(TradingDeal deal)
        {
            if (deal.CardToTrade == null)
            {
                throw new InvalidOperationException("CardToTrade cannot be null when adding a trading deal.");
            }

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO TradingDeals (Id, CardToTradeId, AcceptedElement, AcceptedSpecies, MinimumDamage)
                VALUES (@Id, @CardToTradeId, @AcceptedElement, @AcceptedSpecies, @MinimumDamage)
            ";
            command.Parameters.AddWithValue("@Id", deal.Id);
            command.Parameters.AddWithValue("@CardToTradeId", deal.CardToTrade.Id);
            command.Parameters.AddWithValue("@AcceptedElement", (int)deal.AcceptedElement);
            command.Parameters.AddWithValue("@AcceptedSpecies", (int)deal.AcceptedSpecies);
            command.Parameters.AddWithValue("@MinimumDamage", deal.MinimumDamage);

            command.ExecuteNonQuery();
        }

        public TradingDeal? GetTradingDealById(string id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT td.Id, c.Id, c.Name, c.Damage, c.ElementType, c.CardType, td.AcceptedElement, td.AcceptedSpecies, td.MinimumDamage
                FROM TradingDeals td
                JOIN Cards c ON td.CardToTradeId = c.Id
                WHERE td.Id = @Id
            ";
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var card = new MonsterCard(
                    reader.GetString(2),            // Name
                    reader.GetDouble(3),            // Damage
                    (Element)reader.GetInt32(4),    // ElementType
                    (Species)reader.GetInt32(5)     // Species
                )
                {
                    Id = reader.GetGuid(1)          // Card ID
                };

                return new TradingDeal(
                    reader.GetString(0),            // TradingDeal ID
                    card,
                    (Element)reader.GetInt32(6),    // AcceptedElement
                    (Species)reader.GetInt32(7),    // AcceptedSpecies
                    reader.GetFloat(8)              // MinimumDamage
                );
            }

            return null;
        }

        public void RemoveTradingDeal(string id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM TradingDeals WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }
    }
}
