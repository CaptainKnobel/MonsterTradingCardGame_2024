using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Data_Access
{
    public interface ITradingRepository
    {
        void AddTradingDeal(TradingDeal deal);
        TradingDeal? GetTradingDealById(string id);
        void RemoveTradingDeal(string id);
    }
}
