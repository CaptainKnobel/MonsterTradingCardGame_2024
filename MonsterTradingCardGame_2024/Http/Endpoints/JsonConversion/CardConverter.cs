using MonsterTradingCardGame_2024.Enums;
using MonsterTradingCardGame_2024.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Http.Endpoints.JsonConversion
{
    public class CardConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Card).IsAssignableFrom(objectType);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);

            var name = jsonObject["Name"]?.Value<string>() ?? throw new JsonSerializationException("Card name is missing");
            var damage = jsonObject["Damage"]!.Value<double>();
            var cardType = (CardType)jsonObject["CardType"]!.Value<int>();
            var element = InferElementFromName(name);

            Card card = cardType switch
            {
                CardType.Monster => new MonsterCard(
                    name,
                    damage,
                    element,
                    (Species)jsonObject["MonsterSpecies"]!.Value<int>()
                ),
                CardType.Spell => new SpellCard(name, damage, element),
                _ => throw new JsonSerializationException("Unknown card type")
            };

            serializer.Populate(jsonObject.CreateReader(), card);
            return card;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        private Element InferElementFromName(string name)
        {
            if (name.Contains("Normal", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Grass", StringComparison.OrdinalIgnoreCase) ||
                name.Contains("Regular", StringComparison.OrdinalIgnoreCase))
            {
                return Element.Normal;
            }
            if (name.Contains("Fire", StringComparison.OrdinalIgnoreCase))
            {
                return Element.Fire;
            }
            if (name.Contains("Water", StringComparison.OrdinalIgnoreCase))
            {
                return Element.Water;
            }
            if (name.Contains("Earth", StringComparison.OrdinalIgnoreCase))
            {
                return Element.Earth;
            }
            if (name.Contains("Air", StringComparison.OrdinalIgnoreCase))
            {
                return Element.Air;
            }

            // Fallback to normal if element cannot be inferred
            return Element.Normal;
        }
    }
}
