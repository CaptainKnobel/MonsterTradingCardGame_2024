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
        private readonly IUserRepository _userRepository;

        public TradingHandler(ITradingRepository tradingRepository, ICardRepository cardRepository, IUserRepository userRepository)
        {
            _tradingRepository = tradingRepository;
            _cardRepository = cardRepository;
            _userRepository = userRepository;
        }

        public void CreateTradingDeal(TradingDeal deal)
        {
            if (deal.CardToTradeId == null)
                throw new ArgumentException("CardToTradeId cannot be null");

            var cardToTrade = _cardRepository.GetCardById(deal.CardToTradeId.Value);
            if (cardToTrade == null)
                throw new InvalidOperationException("The specified card does not exist");

            if (cardToTrade.Locked)
                throw new InvalidOperationException("The card to trade is locked and cannot be offered.");

            // Erstelle das TradingDeal-Objekt
            deal.CardToTrade = cardToTrade;

            _tradingRepository.AddTradingDeal(deal);
        }

        public bool AcceptTradingDeal(Guid tradingId, Guid offeredCardId)
        {
            var deal = _tradingRepository.GetTradingDealById(tradingId);
            if (deal == null || deal.CardToTradeId == null)
                throw new InvalidOperationException("Trading deal not found");

            var offeredCard = _cardRepository.GetCardById(offeredCardId);
            if (offeredCard == null || offeredCard.Locked)
                throw new InvalidOperationException("The offered card is invalid or locked.");

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

        public TradingDeal? GetTradingDealById(Guid id)
        {
            return _tradingRepository.GetTradingDealById(id);
        }

        public void RemoveTradingDeal(Guid tradingId)
        {
            _tradingRepository.RemoveTradingDeal(tradingId);
        }

        public Card? GetCardById(Guid cardId)
        {
            return _cardRepository.GetCardById(cardId);
        }
        public User? GetUserByToken(string token)
        {
            return _userRepository.GetUserByToken(token);
        }
    }
}
