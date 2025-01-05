using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Business_Logic
{
    internal class PackageHandler
    {
        private readonly IPackageRepository _packageRepository;

        public PackageHandler(IPackageRepository packageRepository)
        {
            _packageRepository = packageRepository;
        }

        public bool CreatePackage(List<Card> cards)
        {
            if (cards.Count != 5)
            {
                return false;
            }

            var adminId = _packageRepository.GetAdminId();

            if (!adminId.HasValue)
            {
                Console.WriteLine("Admin ID does not exist.");
                return false; // Abbrechen, wenn kein Admin existiert
            }

            foreach (var card in cards)
            {
                card.OwnerId = adminId.Value; // Setze den Admin als Besitzer
            }

            var package = new CardPackage(cards);
            return _packageRepository.AddPackage(package);
        }
    }
}
