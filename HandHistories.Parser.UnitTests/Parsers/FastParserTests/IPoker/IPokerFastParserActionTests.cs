using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using NUnit.Framework;
using HandHistories.Parser.UnitTests.Parsers.Base;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser.IPoker;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.IPoker
{
    using Parser = IPokerFastParserImpl;
    [TestFixture]
    class IPokerFastParserActionTests
    {
        [Test]
        public void ParseBlindActionLine_PostingAnte_Works()
        {
            HandAction handAction = Parser.ParseHandAction("<action no=\"1\" player=\"Amalfitano1\" type=\"15\" sum=\"€0.02\" cards=\"[cards]\" />", Street.Preflop);

            Assert.AreEqual(new HandAction("Amalfitano1", HandActionType.ANTE, 0.02m, Street.Preflop, 1), handAction);
        }

        [Test]
        public void ParseBlindActionLine_PostingSmallBlind_Works()
        {
            HandAction handAction = Parser.ParseHandAction("<action no=\"3\" player=\"Amalfitano1\" type=\"1\" sum=\"€0.05\" cards=\"[cards]\" />", Street.Preflop);

            Assert.AreEqual(new HandAction("Amalfitano1", HandActionType.SMALL_BLIND, 0.05m, Street.Preflop, 3), handAction);
        }

        [Test]
        public void ParseBlindActionLine_PostingBigBlind_Works()
        {
            HandAction handAction = Parser.ParseHandAction("<action no=\"4\" player=\"killAA007\" type=\"2\" sum=\"€0.10\" cards=\"[cards]\" />", Street.Preflop);

            Assert.AreEqual(new HandAction("killAA007", HandActionType.BIG_BLIND, 0.10m, Street.Preflop, 4), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Bet_Works()
        {
            HandAction handAction = Parser.ParseHandAction("<action no=\"10\" player=\"Dullaghan\" type=\"5\" sum=\"€0.20\" cards=\"\" />", Street.Flop);

            Assert.AreEqual(new HandAction("Dullaghan", HandActionType.BET, 0.20m, Street.Flop, 10), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Check_Works()
        {
            HandAction handAction = Parser.ParseHandAction("<action no=\"9\" player=\"joemags\" type=\"4\" sum=\"€0\" cards=\"\" />", Street.Flop);

            Assert.AreEqual(new HandAction("joemags", HandActionType.CHECK, 0m, Street.Flop, 9), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Call_Works()
        {
            HandAction handAction = Parser.ParseHandAction("<action no=\"6\" player=\"Taras2107\" type=\"3\" sum=\"€1.30\" cards=\"\" />", Street.Preflop);

            Assert.AreEqual(new HandAction("Taras2107", HandActionType.CALL, 1.3m, Street.Preflop, 6), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Raise_Works()
        {
            HandAction handAction = Parser.ParseHandAction("<action no=\"7\" player=\"Amalfitano1\" type=\"23\" sum=\"€12.10\" cards=\"\" />", Street.Preflop);

            Assert.AreEqual(new HandAction("Amalfitano1", HandActionType.RAISE, 12.10m, Street.Preflop, 7), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Fold_Works()
        {
            HandAction handAction = Parser.ParseHandAction("<action no=\"3\" player=\"17111982\" type=\"0\" sum=\"€0\" cards=\"\" />", Street.Preflop);

            Assert.AreEqual(new HandAction("17111982", HandActionType.FOLD, 0m, Street.Preflop, 3), handAction);
        }
    }
}
