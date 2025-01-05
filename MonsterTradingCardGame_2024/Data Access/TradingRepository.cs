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

            // Check if the Card is locked
            if (deal.CardToTrade.Locked)
            {
                throw new InvalidOperationException("The card to trade is locked and cannot be added to a trading deal.");
            }

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                // Lock the card before adding it to the trade
                using (var lockCommand = connection.CreateCommand())
                {
                    lockCommand.CommandText = @"
                        UPDATE Cards
                        SET Locked = TRUE
                        WHERE Id = @CardId
                    ";
                    lockCommand.Parameters.AddWithValue("@CardId", deal.CardToTrade.Id);
                    if (lockCommand.ExecuteNonQuery() == 0)
                    {
                        throw new InvalidOperationException("Failed to lock the card for trading.");
                    }
                }

                // Add TradingDeal to the Database
                using (var command = connection.CreateCommand())
                {
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

                // Finalize Transaktion
                transaction.Commit();
            }
            catch
            {
                // Rollback in case of an error
                transaction.Rollback();
                throw;
            }
        }


        public TradingDeal? GetTradingDealById(string id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT td.Id, c.Id, c.Name, c.Damage, c.ElementType, c.Species, c.CardType, c.OwnerId, c.Locked, 
                       td.AcceptedElement, td.AcceptedSpecies, td.MinimumDamage
                FROM TradingDeals td
                JOIN Cards c ON td.CardToTradeId = c.Id
                WHERE td.Id = @Id
            ";
            command.Parameters.AddWithValue("@Id", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var cardType = (CardType)reader.GetInt32(6);
                var ownerId = reader.GetGuid(7);

                Card card = cardType switch
                {
                    CardType.Monster => new MonsterCard(
                        reader.GetString(2),            // Name
                        reader.GetDouble(3),            // Damage
                        (Element)reader.GetInt32(4),    // ElementType
                        (Species)reader.GetInt32(5),    // Species
                        ownerId                         // OwnerId
                    )
                    {
                        Id = reader.GetGuid(1),         // Card ID
                        Locked = reader.GetBoolean(8)   // Locked
                    },
                    CardType.Spell => new SpellCard(
                        reader.GetString(2),            // Name
                        reader.GetDouble(3),            // Damage
                        (Element)reader.GetInt32(4),    // ElementType
                        ownerId                         // OwnerId
                    )
                    {
                        Id = reader.GetGuid(1),         // Card ID
                        Locked = reader.GetBoolean(8)   // Locked
                    },
                    _ => throw new InvalidOperationException("Unknown card type.")
                };

                return new TradingDeal(
                    reader.GetString(0),                // TradingDeal ID
                    card,
                    (Element)reader.GetInt32(9),        // AcceptedElement
                    (Species)reader.GetInt32(10),       // AcceptedSpecies
                    reader.GetFloat(11)                 // MinimumDamage
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
