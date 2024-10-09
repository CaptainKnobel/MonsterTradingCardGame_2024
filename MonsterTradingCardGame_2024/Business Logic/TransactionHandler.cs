using MonsterTradingCardGame_2024.Data_Access;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Business_Logic
{
    internal static class TransactionHandler
    {
        public static bool BuyPackage(string token)
        {
            // 1. Validate the user by token
            var user = UserHandler.FindUserByToken(token);
            if (user == null)
            {
                return false; // Invalid user
            }

            // 2. Check if user has enough coins
            if (user.Coins < 5)  // Example: a package costs 5 coins // TODO: Maybe integrate this number into a definition script
            {
                return false; // Not enough coins
            }

            // 3. Check if packages are available
            var package = PackageRepository.GetAvailablePackage();
            if (package == null)
            {
                return false; // No packages available
            }

            // 4. Deduct the cost from the user's coins
            user.Coins -= 5;

            // 5. Add all the cards from the package to the user's stack
            foreach (var card in package.Cards)
            {
                user.AddCardToStack(card);
            }

            return true;
        }
    }
}
