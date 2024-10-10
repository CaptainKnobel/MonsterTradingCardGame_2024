using MonsterTradingCardGame_2024.Enums;
using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.JsonConverters
{
    internal class CardConverter : JsonConverter<Card>
    {
        public override Card Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonDoc = JsonDocument.ParseValue(ref reader);
            var rootElement = jsonDoc.RootElement;

            // Determine the card type from the JSON data
            var cardType = (CardType)rootElement.GetProperty("CardType").GetInt32();

            switch (cardType)
            {
                case CardType.Monster:
                    var monsterCard = JsonSerializer.Deserialize<MonsterCard>(rootElement.GetRawText(), options);
                    if (monsterCard == null)
                        throw new JsonException("Failed to deserialize MonsterCard.");
                    return monsterCard;

                case CardType.Spell:
                    var spellCard = JsonSerializer.Deserialize<SpellCard>(rootElement.GetRawText(), options);
                    if (spellCard == null)
                        throw new JsonException("Failed to deserialize SpellCard.");
                    return spellCard;

                default:
                    throw new JsonException("Unknown card type.");
            }
        }

        public override void Write(Utf8JsonWriter writer, Card value, JsonSerializerOptions options)
        {
            // Serialize the card based on its actual type
            if (value is MonsterCard monsterCard)
            {
                JsonSerializer.Serialize(writer, monsterCard, options);
            }
            else if (value is SpellCard spellCard)
            {
                JsonSerializer.Serialize(writer, spellCard, options);
            }
            else
            {
                throw new JsonException("Unknown card type.");
            }
        }
    }
}
