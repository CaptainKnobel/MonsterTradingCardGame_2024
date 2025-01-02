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
    public class CardRepository : ICardRepository
    {
        private readonly string _connectionString;

        public CardRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Card> GetCardsByUserId(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Name, Damage, ElementType, Species, CardType, OwnerId
                FROM Cards
                WHERE OwnerId = @UserId
            ";
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();
            var cards = new List<Card>();
            while (reader.Read())
            {
                var id = reader.GetGuid(0);
                var name = reader.GetString(1);
                var damage = reader.GetDouble(2);
                var elementType = (Element)reader.GetInt32(3);
                var cardType = (CardType)reader.GetInt32(5);
                var ownerId = reader.GetInt32(6);

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

        public Card? GetCardById(Guid cardId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Name, Damage, ElementType, Species, CardType, OwnerId
                FROM Cards
                WHERE Id = @CardId;
            ";
            command.Parameters.AddWithValue("@CardId", cardId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var id = reader.GetGuid(0);
                var name = reader.GetString(1);
                var damage = reader.GetDouble(2);
                var elementType = (Element)reader.GetInt32(3);
                var cardType = (CardType)reader.GetInt32(5);
                var ownerId = reader.GetInt32(6);

                if (cardType == CardType.Monster)
                {
                    var species = (Species)reader.GetInt32(4);
                    return new MonsterCard(name, damage, elementType, species, ownerId);
                }
                else
                {
                    return new SpellCard(name, damage, elementType, ownerId);
                }
            }
            return null;
        }

        public bool AddCard(Card card)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Cards (Id, Name, Damage, ElementType, CardType, OwnerId)
                VALUES (@Id, @Name, @Damage, @ElementType, @CardType, @OwnerId)
            ";
            command.Parameters.AddWithValue("@Id", card.Id);
            command.Parameters.AddWithValue("@Name", card.Name);
            command.Parameters.AddWithValue("@Damage", card.Damage);
            command.Parameters.AddWithValue("@ElementType", (int)card.ElementType);
            command.Parameters.AddWithValue("@CardType", (int)card.CardType);
            command.Parameters.AddWithValue("@OwnerId", card.OwnerId);

            return command.ExecuteNonQuery() > 0;
        }

        public void UpdateCard(Card card)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE Cards
                SET OwnerId = @OwnerId
                WHERE Id = @Id
            ";
            command.Parameters.AddWithValue("@OwnerId", card.OwnerId);
            command.Parameters.AddWithValue("@Id", card.Id);

            command.ExecuteNonQuery();
        }
    }
}
