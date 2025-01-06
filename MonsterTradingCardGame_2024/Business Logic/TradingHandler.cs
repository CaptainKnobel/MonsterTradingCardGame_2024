using MonsterTradingCardGame_2024.Data_Access;
using MonsterTradingCardGame_2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame_2024.Business_Logic
{
    public class TradingHandler
    {
        private readonly ITradingRepository _tradingRepository;
        private readonly ICardRepository _cardRepository;

        public TradingHandler(ITradingRepository tradingRepository, ICardRepository cardRepository)
        {
            _tradingRepository = tradingRepository;
            _cardRepository = cardRepository;
        }

        public void CreateTradingDeal(TradingDeal deal)
        {
            if (deal.CardToTrade == null)
                throw new ArgumentException("Card to trade cannot be null");

            if (deal.CardToTrade.Locked)
                throw new InvalidOperationException("The card to trade is locked and cannot be offered.");

            _tradingRepository.AddTradingDeal(deal);
        }

        public bool AcceptTradingDeal(string tradingId, Card offeredCard)
        {
            var deal = _tradingRepository.GetTradingDealById(tradingId);
            if (deal == null)
                throw new InvalidOperationException("Trading deal not found");

            if (offeredCard.Locked)
                throw new InvalidOperationException("The offered card is locked and cannot be traded.");

            if (deal.CardToTrade?.Locked == true)
                throw new InvalidOperationException("The card in the trading deal is locked and cannot be traded.");

            if (offeredCard.ElementType != deal.AcceptedElement ||
                (offeredCard is MonsterCard monsterCard && monsterCard.MonsterSpecies != deal.AcceptedSpecies) ||
                offeredCard.Damage < deal.MinimumDamage)
            {
                return false; // Conditions not met
            }

            // Perform trade
            var offeredCardOwnerId = offeredCard.OwnerId;
            offeredCard.OwnerId = deal.CardToTrade?.OwnerId ?? throw new InvalidOperationException("Invalid trading deal state");
            deal.CardToTrade.OwnerId = offeredCardOwnerId;

            // Lock traded cards after the trade (to prevent immediate re-trade)
            offeredCard.Locked = true;
            deal.CardToTrade.Locked = true;

            // Update cards in the database
            _cardRepository.UpdateCard(offeredCard);
            _cardRepository.UpdateCard(deal.CardToTrade);

            // Remove the trading deal after completion
            _tradingRepository.RemoveTradingDeal(tradingId);

            return true;
        }

        public List<TradingDeal> GetAllTradingDeals()
        {
            return _tradingRepository.GetAllTradingDeals();
        }

        public TradingDeal? GetTradingDealById(string id)
        {
            return _tradingRepository.GetTradingDealById(id);
        }

        public void RemoveTradingDeal(string tradingId)
        {
            _tradingRepository.RemoveTradingDeal(tradingId);
        }
    }
}
