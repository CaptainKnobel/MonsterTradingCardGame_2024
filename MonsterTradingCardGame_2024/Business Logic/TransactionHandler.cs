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
        private const int packageCost = 5;

        public TransactionHandler(UserHandler userHandler, IPackageRepository packageRepository)
        {
            _userHandler = userHandler;
            _packageRepository = packageRepository;
        }

        public (bool IsSuccess, List<Card>? PurchasedCards, string? ErrorMessage) BuyPackage(string token)
        {
            // 1. Validate the user by token
            var user = _userHandler.FindUserByToken(token);
            if (user == null)
            {
                return (false, null, "Invalid authentication token");
            }

            // 2. Check if user has enough coins
            if(user.Coins < packageCost)
            {
                return (false, null, "Not enough money to buy the package");
            }

            // 3. Check if packages are available
            List<Card>? cards = _packageRepository.GetAvailablePackage();
            if (cards == null)
            {
                return (false, null, "No packages available for purchase"); // No packages available
            }

            // 4. Add all the cards from the package to the user's stack in the database
            var transferSuccess = _packageRepository.TransferOwnership(cards, user.Id);
            if (!transferSuccess)
            {
                return (false, null, "Failed to transfer ownership of the package");
            }

            // 5. Deduct the cost from the user's coins
            if (!_userHandler.SpendCoins(user, packageCost))
            {
                return (false, null, "Not enough money to buy the package"); // Not enough coins
            }

            // 5. Successful return of the Package
            return (true, cards, null);
        }
    }
}
