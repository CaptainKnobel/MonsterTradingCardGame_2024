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

            _tradingRepository.AddTradingDeal(deal);
        }

        public bool AcceptTradingDeal(string tradingId, Card offeredCard)
        {
            var deal = _tradingRepository.GetTradingDealById(tradingId);
            if (deal == null)
                throw new InvalidOperationException("Trading deal not found");

            if (offeredCard.ElementType != deal.AcceptedElement ||
                offeredCard is MonsterCard monsterCard && monsterCard.MonsterSpecies != deal.AcceptedSpecies ||
                offeredCard.Damage < deal.MinimumDamage)
            {
                return false; // Bedingungen nicht erfüllt
            }

            // Handel durchführen
            // Besitzerwechsel durchführen
            offeredCard.OwnerId = deal.CardToTrade?.OwnerId ?? throw new InvalidOperationException("Invalid trading deal state");
            deal.CardToTrade.OwnerId = offeredCard.OwnerId;

            // Karten aktualisieren
            _cardRepository.UpdateCard(offeredCard);
            _cardRepository.UpdateCard(deal.CardToTrade);

            // Handelsangebot entfernen
            _tradingRepository.RemoveTradingDeal(tradingId);
            return true;
        }
    }
}
