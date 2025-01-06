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

        public Guid? GetAdminId()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id FROM Users WHERE Token = 'admin-mtcgToken';";

            var result = command.ExecuteScalar();

            if (result == null || result == DBNull.Value)
            {
                return null;
            }

            if (Guid.TryParse(result.ToString(), out var adminId))
            {
                return adminId;
            }

            return null;
        }

        // Retrieve an available package (if any exists)
        public List<Card>? GetAvailablePackage()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, CardIds FROM Packages LIMIT 1;";  // Get the first available package

            using var reader = command.ExecuteReader(); 
            if (reader.Read())
            {
                var packageId = reader.GetGuid(0);              // Reads the UUID from column from Id
                var cardIds = reader.GetFieldValue<Guid[]>(1);  // Reads the UUID-Array colimn from CardIds

                var cards = GetCardsByIds(cardIds);

                Console.WriteLine($"Retrieved package {packageId} with cards:");
                foreach (var card in cards)
                {
                    Console.WriteLine($"Card {card.Id}, OwnerId: {card.OwnerId}");
                }
                DeletePackageById(packageId);   // Remove it from the available list
                return cards;
            }

            return null;  // No packages available
        }

        // Add a new package to the repository (for "admin" usage)
        public bool AddPackage(CardPackage package)
        {
            // A package must have exactly 5 cards
            if (package.Cards.Count != 5)
            {
                throw new InvalidOperationException("A package must contain exactly 5 cards.");
            }

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

        public void AddCard(Card card, NpgsqlConnection connection)
        {
            using var command = connection.CreateCommand();

            if (card is MonsterCard monsterCard)
            {
                command.CommandText = @"
                    INSERT INTO Cards (Id, Name, Damage, ElementType, CardType, Species, OwnerId, Locked)
                    VALUES (@Id, @Name, @Damage, @ElementType, @CardType, @Species, @OwnerId, @Locked)
                    ON CONFLICT (Id) DO NOTHING;
                ";
                command.Parameters.AddWithValue("@Species", (int)monsterCard.MonsterSpecies);
            }
            else
            {
                command.CommandText = @"
                    INSERT INTO Cards (Id, Name, Damage, ElementType, CardType, OwnerId, Locked)
                    VALUES (@Id, @Name, @Damage, @ElementType, @CardType, @OwnerId, @Locked)
                    ON CONFLICT (Id) DO NOTHING;
                ";
            }
            command.Parameters.AddWithValue("@Id", card.Id);
            command.Parameters.AddWithValue("@Name", card.Name);
            command.Parameters.AddWithValue("@Damage", card.Damage);
            command.Parameters.AddWithValue("@ElementType", (int)card.ElementType);
            command.Parameters.AddWithValue("@CardType", (int)card.CardType);
            command.Parameters.AddWithValue("@OwnerId", card.OwnerId);
            command.Parameters.AddWithValue("@Locked", card.Locked);

            command.ExecuteNonQuery();
        }

        public List<Card> GetCardsByIds(Guid[] cardIds)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Damage, ElementType, Species, CardType, OwnerId FROM Cards WHERE Id = ANY(@CardIds);";
            command.Parameters.AddWithValue("@CardIds", cardIds);

            using var reader = command.ExecuteReader();
            var cards = new List<Card>();
            while (reader.Read())
            {
                var id = reader.GetGuid(0);
                var name = reader.GetString(1);
                var damage = reader.GetDouble(2);
                var elementType = (Element)reader.GetInt32(3);
                var cardType = (CardType)reader.GetInt32(5);
                var ownerId = reader.GetGuid(6);

                if (cardType == CardType.Monster)
                {
                    var species = (Species)reader.GetInt32(4);
                    cards.Add(new MonsterCard(name, damage, elementType, species, ownerId));
                }
                else
                {
                    cards.Add(new SpellCard(name, damage, elementType, ownerId));
                }
            }

            return cards;
        }

        public void DeletePackageById(Guid packageId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Packages WHERE Id = @Id;";
            command.Parameters.AddWithValue("@Id", packageId);

            command.ExecuteNonQuery();
        }

        public bool TransferOwnership(List<Card> cards, Guid newOwnerId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                foreach (var card in cards)
                {
                    using var command = connection.CreateCommand();
                    command.CommandText = "UPDATE Cards SET OwnerId = @NewOwnerId WHERE Id = @CardId;";
                    command.Parameters.AddWithValue("@NewOwnerId", newOwnerId);
                    command.Parameters.AddWithValue("@CardId", card.Id);

                    Console.WriteLine($"Transferring card {card.Id} to owner {newOwnerId}");
                    command.ExecuteNonQuery();
                }

                transaction.Commit();

                // Update local Card objects
                foreach(var card in cards)
                {
                    card.OwnerId = newOwnerId;
                }
                Console.WriteLine($"All cards successfully transferred.");

                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error transferring ownership: {ex.Message}");
                return false;
            }
        }
    }
}
