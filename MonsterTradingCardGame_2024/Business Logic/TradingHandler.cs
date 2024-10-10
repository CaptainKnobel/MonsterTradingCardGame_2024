using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Business_Logic
{
    internal static class TradingHandler
    {
        private static List<TradingDeal> tradingDeals = new List<TradingDeal>();

        // Fetch all current trading deals
        public static List<TradingDeal> GetAllTrades()
        {
            return tradingDeals;
        }

        // Add a new trade
        public static bool AddTrade(TradingDeal newTrade)
        {
            if (newTrade != null && !tradingDeals.Any(t => t.Id == newTrade.Id))
            {
                tradingDeals.Add(newTrade);
                return true;
            }
            return false;
        }

        // Remove a trade by its ID
        public static bool RemoveTrade(string tradeId)
        {
            var tradeToRemove = tradingDeals.FirstOrDefault(t => t.Id == tradeId);
            if (tradeToRemove != null)
            {
                tradingDeals.Remove(tradeToRemove);
                return true;
            }
            return false;
        }
    }
}
