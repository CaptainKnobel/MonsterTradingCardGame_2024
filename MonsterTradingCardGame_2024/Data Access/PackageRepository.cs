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
    internal class PackageRepository : IPackageRepository
    {
        private readonly string _connectionString;
        public PackageRepository(string connectionString)
        {
            _connectionString = connectionString;
        }


        // Retrieve an available package (if any exists)
        public CardPackage? GetAvailablePackage()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, CardIds FROM Packages LIMIT 1;";  // Get the first available package

            using var reader = command.ExecuteReader(); 
            if (reader.Read())
            {
                var packageId = reader.GetInt32(0);
                var cardIds = reader.GetFieldValue<Guid[]>(1);

                var cards = GetCardsByIds(cardIds);
                DeletePackageById(packageId);   // Remove it from the available list
                return new CardPackage(cards);
            }

            return null;  // No packages available
        }

        // Add a new package to the repository (for "admin" usage)
        public bool AddPackage(CardPackage package)
        {
            // TODO: A package must have exactly 5 cards

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                foreach (var card in package.Cards)
                {
                    AddCard(card, connection);
                }

                var cardIds = package.Cards.Select(card => card.Id).ToArray();
                using var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Packages (CardIds) VALUES (@CardIds);";
                command.Parameters.AddWithValue("@CardIds", cardIds);

                command.ExecuteNonQuery();
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        // Check how many packages are available
        public int GetAvailablePackageCount()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM Packages;";

            return Convert.ToInt32(command.ExecuteScalar());
        }

        private void AddCard(Card card, NpgsqlConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Cards (Id, Name, Damage, ElementType, CardType)
                VALUES (@Id, @Name, @Damage, @ElementType, @CardType)
                ON CONFLICT (Id) DO NOTHING;
            ";
            command.Parameters.AddWithValue("@Id", card.Id);
            command.Parameters.AddWithValue("@Name", card.Name);
            command.Parameters.AddWithValue("@Damage", card.Damage);
            command.Parameters.AddWithValue("@ElementType", (int)card.ElementType);
            command.Parameters.AddWithValue("@CardType", (int)card.CardType);

            command.ExecuteNonQuery();
        }

        private List<Card> GetCardsByIds(Guid[] cardIds)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Damage, ElementType, CardType FROM Cards WHERE Id = ANY(@CardIds);";
            command.Parameters.AddWithValue("@CardIds", cardIds);

            using var reader = command.ExecuteReader();
            var cards = new List<Card>();
            while (reader.Read())
            {
                var id = reader.GetGuid(0);
                var name = reader.GetString(1);
                var damage = reader.GetDouble(2);
                var elementType = (Element)reader.GetInt32(3);
                var cardType = (CardType)reader.GetInt32(4);

                if (cardType == CardType.Monster)
                {
                    cards.Add(new MonsterCard(name, damage, elementType, Species.Dragon)); // Spezies anpassen
                }
                else
                {
                    cards.Add(new SpellCard(name, damage, elementType));
                }
            }

            return cards;
        }

        private void DeletePackageById(int packageId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Packages WHERE Id = @Id;";
            command.Parameters.AddWithValue("@Id", packageId);

            command.ExecuteNonQuery();
        }

    }
}
