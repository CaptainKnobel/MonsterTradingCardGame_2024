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
    public class DeckRepository : IDeckRepository
    {
        private readonly string _connectionString;

        public DeckRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Card> GetDeckByUserId(int userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT c.Id, c.Name, c.Damage, c.ElementType, c.CardType
                FROM Cards c
                JOIN Decks d ON c.Id = ANY(d.CardIds)
                WHERE d.UserId = @UserId
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
                var cardType = (CardType)reader.GetInt32(4);

                if (cardType == CardType.Monster)
                {
                    cards.Add(new MonsterCard(name, damage, elementType, Species.Dragon)); // TODO: Spezies anpassen
                }
                else
                {
                    cards.Add(new SpellCard(name, damage, elementType));
                }
            }

            return cards;
        }

        public bool UpdateDeck(int userId, IEnumerable<Guid> cardIds)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Decks (UserId, CardIds)
                VALUES (@UserId, @CardIds)
                ON CONFLICT (UserId)
                DO UPDATE SET CardIds = @CardIds
            ";
            command.Parameters.AddWithValue("@UserId", userId);
            command.Parameters.AddWithValue("@CardIds", cardIds.ToArray());

            return command.ExecuteNonQuery() > 0;
        }
    }
}
