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
        public override Card? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonDoc = JsonDocument.ParseValue(ref reader);
            var rootElement = jsonDoc.RootElement;

            // Check the CardType to distinguish between MonsterCard and SpellCard
            var cardType = (CardType)rootElement.GetProperty("CardType").GetInt32();

            string name = rootElement.GetProperty("Name").GetString() ?? "";
            double damage = rootElement.GetProperty("Damage").GetDouble();
            Element elementType = (Element)rootElement.GetProperty("ElementType").GetInt32();

            switch (cardType)
            {
                case CardType.Monster:
                    // Deserialize MonsterCard (which has additional MonsterSpecies)
                    Species monsterSpecies = (Species)rootElement.GetProperty("MonsterSpecies").GetInt32();
                    return new MonsterCard(name, damage, elementType, monsterSpecies);

                case CardType.Spell:
                    // Deserialize SpellCard
                    return new SpellCard(name, damage, elementType);

                default:
                    throw new JsonException("Unknown card type.");
            }
        }

        public override void Write(Utf8JsonWriter writer, Card value, JsonSerializerOptions options)
        {
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
