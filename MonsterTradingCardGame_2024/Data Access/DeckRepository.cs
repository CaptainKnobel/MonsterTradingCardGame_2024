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

        public List<Card> GetDeckByUserId(Guid userId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                SELECT c.Id, c.Name, c.Damage, c.ElementType, c.Species, c.CardType
                FROM Cards c
                JOIN Decks d ON c.Id = ANY(d.CardIds)
                WHERE d.UserId = @UserId;
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

                if (cardType == CardType.Monster)
                {
                    var species = (Species)reader.GetInt32(4);
                    cards.Add(new MonsterCard(name, damage, elementType, species, userId) { Id = id });
                }
                else
                {
                    cards.Add(new SpellCard(name, damage, elementType, userId) { Id = id });
                }
                Console.WriteLine($"Retrieved Card: ID={id}, OwnerID={userId}, Name={name}");
            }
            if (!cards.Any())
            {
                Console.WriteLine($"No cards found in the deck for user {userId}.");
            }
            return cards;
        }

        public bool UpdateDeck(Guid userId, IEnumerable<Guid> cardIds)
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

        public bool RemoveCardsFromDeck(Guid userId, IEnumerable<Guid> cardIds)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            // Aktuelles CardIds-Array abrufen
            using var selectCommand = connection.CreateCommand();
            selectCommand.CommandText = @"
                SELECT CardIds
                FROM Decks
                WHERE UserId = @UserId;
            ";
            selectCommand.Parameters.AddWithValue("@UserId", userId);

            var currentCardIds = new List<Guid>();
            using (var reader = selectCommand.ExecuteReader())
            {
                if (reader.Read() && !reader.IsDBNull(0))
                {
                    currentCardIds = reader.GetFieldValue<Guid[]>(0).ToList();
                }
            }

            // Entfernte Karten ermitteln
            var updatedCardIds = currentCardIds.Except(cardIds).ToArray();

            // CardIds-Array aktualisieren
            using var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = @"
                UPDATE Decks
                SET CardIds = @UpdatedCardIds
                WHERE UserId = @UserId;
            ";
            updateCommand.Parameters.AddWithValue("@UpdatedCardIds", updatedCardIds);
            updateCommand.Parameters.AddWithValue("@UserId", userId);

            return updateCommand.ExecuteNonQuery() > 0;
        }

    }
}
