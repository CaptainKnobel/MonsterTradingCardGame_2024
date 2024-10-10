using MonsterTradingCardGame_2024.Enums;
using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Data_Access
{
    internal static class PackageRepository
    {
        // Dummy storage for packages (in-memory list)
        private static List<CardPackage> packages = new List<CardPackage>
        {
            /*
            new CardPackage(new List<Card>
            {
                new MonsterCard("Dragon", 50.0, Element.Fire, Species.Dragon),
                new SpellCard("FireSpell", 30.0, Element.Fire),
                new MonsterCard("WaterGoblin", 10.0, Element.Water, Species.Goblin),
                new MonsterCard("Ork", 40.0, Element.Normal, Species.Ork),
                new SpellCard("RegularSpell", 20.0, Element.Normal)
            }),
            new CardPackage(new List<Card>
            {
                new MonsterCard("WaterGoblin", 10.0, Element.Water, Species.Goblin),
                new MonsterCard("Dragon", 50.0, Element.Fire, Species.Dragon),
                new SpellCard("FireSpell", 25.0, Element.Fire),
                new MonsterCard("Ork", 45.0, Element.Normal, Species.Ork),
                new SpellCard("RegularSpell", 20.0, Element.Normal)
            })
            */
        };

        // Retrieve an available package (if any exists)
        public static CardPackage? GetAvailablePackage()
        {
            if (packages.Any())
            {
                var package = packages.First();  // Get the first available package
                packages.Remove(package);        // Remove it from the available list
                return package;
            }
            return null;  // No packages available
        }

        // Add a new package to the repository (for "admin" usage)
        public static bool AddPackage(CardPackage package)
        {
            if (package.Cards.Count == 5)  // A package must have exactly 5 cards
            {
                packages.Add(package);
                return true;
            }
            return false;
        }

        // Check how many packages are available
        public static int GetAvailablePackageCount()
        {
            return packages.Count;
        }
    }
}
