using System.Collections.Generic;
using HandHistories.Objects.Cards;

namespace HandHistories.HandEvaluator.Equity
{
    public class FakeEquityCalculatorImpl : IEquityCalculator
    {
        public decimal GetEquity(HoleCards heroHoleCards, List<HoleCards> villainCards, BoardCards boardCards, List<Card> deadCards = null)
        {
            return 100.0m;
        }

        public void HandOdds(List<HoleCards> holeCards, BoardCards boardCards, List<Card> deadCards, out long[] wins, out long[] ties, out long[] losses, out long totalHandsEnumerated)
        {
            losses = null;
            ties = null;
            wins = null;
            totalHandsEnumerated = 0;
        }
    }
}
