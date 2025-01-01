using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Data_Access
{
    public interface IPackageRepository
    {
        CardPackage? GetAvailablePackage();
        bool AddPackage(CardPackage package);
        int GetAvailablePackageCount();
    }
}
