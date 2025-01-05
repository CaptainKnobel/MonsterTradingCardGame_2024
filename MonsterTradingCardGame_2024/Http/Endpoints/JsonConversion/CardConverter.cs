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

            // Pflichtfelder auslesen
            var name = jsonObject["Name"]?.Value<string>() ?? throw new JsonSerializationException("Card name is missing");
            var damage = jsonObject["Damage"]?.Value<double>() ?? throw new JsonSerializationException("Card damage is missing");

            // GUIDs auslesen oder generieren
            Guid id = Guid.TryParse(jsonObject["Id"]?.Value<string>(), out var parsedId) ? parsedId : Guid.NewGuid();
            Guid ownerId = jsonObject["OwnerId"]?.Value<Guid>() ?? Guid.NewGuid();

            // Weitere Felder auslesen oder ableiten
            var cardType = jsonObject["CardType"]?.Value<int>() != null
                ? (CardType)jsonObject["CardType"]!.Value<int>()
                : InferCardTypeFromName(name);

            var species = cardType == CardType.Monster && jsonObject["Species"]?.Value<int>() != null
                ? (Species)jsonObject["Species"]!.Value<int>()
                : InferSpeciesFromName(name);

            var element = jsonObject["ElementType"]?.Value<int>() != null
                ? (Element)jsonObject["ElementType"]!.Value<int>()
                : InferElementFromName(name);

            // Karte basierend auf Typ erstellen
            Card card = cardType switch
            {
                CardType.Monster => new MonsterCard(name, damage, element, species, ownerId) { Id = id },
                CardType.Spell => new SpellCard(name, damage, element, ownerId) { Id = id },
                _ => throw new JsonSerializationException($"Unknown card type for name: {name}")
            };

            // Zusätzliche JSON-Werte befüllen
            serializer.Populate(jsonObject.CreateReader(), card);
            return card;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
        private Element InferElementFromName(string name)
        {
            if (name.Contains("Water", StringComparison.OrdinalIgnoreCase))
                return Element.Water;
            if (name.Contains("Fire", StringComparison.OrdinalIgnoreCase))
                return Element.Fire;
            if (name.Contains("Earth", StringComparison.OrdinalIgnoreCase) || name.Contains("Grass", StringComparison.OrdinalIgnoreCase))
                return Element.Earth;
            if (name.Contains("Air", StringComparison.OrdinalIgnoreCase))
                return Element.Air;
            return Element.Normal; // Default-Wert
        }

        private CardType InferCardTypeFromName(string name)
        {
            if (name.Contains("Spell", StringComparison.OrdinalIgnoreCase))
                return CardType.Spell;
            return CardType.Monster; // Default-Wert
        }

        private Species InferSpeciesFromName(string name)
        {
            if (name.Contains("Goblin", StringComparison.OrdinalIgnoreCase))
                return Species.Goblin;
            if (name.Contains("Dragon", StringComparison.OrdinalIgnoreCase))
                return Species.Dragon;
            if (name.Contains("Ork", StringComparison.OrdinalIgnoreCase))
                return Species.Ork;
            if (name.Contains("Elf", StringComparison.OrdinalIgnoreCase))
                return Species.Ork;
            if (name.Contains("Knight", StringComparison.OrdinalIgnoreCase))
                return Species.Knight;
            return Species.Goblin; // Default-Wert
        }
    }
}
