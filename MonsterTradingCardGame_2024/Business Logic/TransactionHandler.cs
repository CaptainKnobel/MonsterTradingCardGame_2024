using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Business_Logic
{
    internal class TransactionHandler
    {
        private readonly UserHandler _userHandler;
        private readonly IPackageRepository _packageRepository;

        public TransactionHandler(UserHandler userHandler, IPackageRepository packageRepository)
        {
            _userHandler = userHandler;
            _packageRepository = packageRepository;
        }

        public (bool IsSuccess, CardPackage? Package, string? ErrorMessage) BuyPackage(string token)
        {
            // 1. Validate the user by token
            var user = _userHandler.FindUserByToken(token);
            if (user == null)
            {
                return (false, null, "Invalid authentication token");
            }

            // 2. Deduct the cost from the user's coins
            if (!_userHandler.SpendCoins(user, 5))
            {
                return (false, null, "Not enough money to buy the package"); // Not enough coins
            }

            // 3. Check if packages are available
            var package = _packageRepository.GetAvailablePackage();
            if (package == null)
            {
                return (false, null, "No packages available for purchase"); // No packages available
            }

            // 4. Add all the cards from the package to the user's stack
            foreach (var card in package.Cards)
            {
                _userHandler.AddCardToStack(user, card);
            }

            // 5. Successful return of the Package
            return (true, package, null);
        }
    }
}
