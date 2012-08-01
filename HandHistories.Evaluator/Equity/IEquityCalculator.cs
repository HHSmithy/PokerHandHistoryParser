using System.Collections.Generic;
using HandHistories.Objects.Cards;

namespace HandHistories.HandEvaluator.Equity
{
    public interface IEquityCalculator
    {
        decimal GetEquity(HoleCards heroHoleCards, List<HoleCards> villainCards, BoardCards boardCards, List<Card> deadCards = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="holeCards"></param>
        /// <param name="boardCards"></param>
        /// <param name="deadCards"></param>
        /// <param name="wins"></param>
        /// <param name="ties">The ties returned need to be divided by the number of players in the hand to equate to Poker Stove.</param>
        /// <param name="losses"></param>
        /// <param name="totalHandsEnumerated"></param>
        void HandOdds(List<HoleCards> holeCards, 
                      BoardCards boardCards, 
                      List<Card> deadCards, 
                      out long[] wins, 
                      out long[] ties, 
                      out long[] losses, 
                      out long totalHandsEnumerated);
    }
}
