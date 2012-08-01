using System;
using System.Collections.Generic;
using System.Linq;
using HandHistories.HandEvaluator.PokerEval;
using HandHistories.Objects.Cards;

namespace HandHistories.HandEvaluator.Equity
{
    public class PokerEvalEquityCalculatorImpl : IEquityCalculator
    {
        public decimal GetEquity(HoleCards heroHoleCards, List<HoleCards> villainCards, BoardCards boardCards, List<Card> deadCards = null)
        {
            throw new NotImplementedException();
        }

        public void HandOdds(List<HoleCards> holeCards, 
                             BoardCards boardCards, 
                             List<Card> deadCards, 
                             out long[] wins, 
                             out long[] ties, 
                             out long[] losses, 
                             out long totalHandsEnumerated)
        {            
            string[] pockets = holeCards.Select(h => h.ToString()).ToArray();
            string deadCardsString = (deadCards == null) ? string.Empty : string.Join("", deadCards.Select(h => h.ToString()));            

            Hand.HandOdds(pockets, boardCards.ToString(), deadCardsString, out wins, out ties, out losses, out totalHandsEnumerated);
        }
    }
}
