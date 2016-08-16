using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.IPoker
{
    class IPokerExtraTests_MTT : IPokerExtraTests
    {
        public IPokerExtraTests_MTT()
            : base(PokerFormat.MultiTableTournament)
        {
        }

        [Test]
        public void ShootoutTounament_Issue()
        {
            string handText = SampleHandHistoryRepository.GetHandExample(_format, Site, "ExtraHands", "ShootOutTournament");

            var hand = GetParser().ParseFullHandHistory(handText);

            var expected = Limit.FromSmallBlindBigBlind(1, 1, Currency.CHIPS);

            Assert.AreEqual(expected, hand.GameDescription.Limit);
        }
    }
}
