using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.FastParser._888;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.Pacific
{
    
    using Parser = Poker888FastParserImpl;
    [TestFixture]
    class PacificFastParserActionTests
    {
        [Test]
        public void ParseBlindActionLine_PostingDead_Works()
        {
            HandAction handAction = Parser.ParseBlindAction("GYAMEPRO posts dead blind [$1 + $2]");

            Assert.AreEqual(new HandAction("GYAMEPRO", HandActionType.POSTS, 3m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseBlindActionLine_PostingSmallBlind_Works()
        {
            HandAction handAction = Parser.ParseBlindAction("Lesnik444 posts small blind [$0.50]");

            Assert.AreEqual(new HandAction("Lesnik444", HandActionType.SMALL_BLIND, 0.5m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseBlindActionLine_PostingBigBlind_Works()
        {
            HandAction handAction = Parser.ParseBlindAction("Lesnik444 posts big blind [$1]");

            Assert.AreEqual(new HandAction("Lesnik444", HandActionType.BIG_BLIND, 1m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Bet_Works()
        {
            HandAction handAction = Parser.ParseRegularActionLine("Jack3bet bets [$10.43]", Street.Flop);

            Assert.AreEqual(new HandAction("Jack3bet", HandActionType.BET, 10.43m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Check_Works()
        {
            HandAction handAction = Parser.ParseRegularActionLine("jott1982 checks", Street.Flop);

            Assert.AreEqual(new HandAction("jott1982", HandActionType.CHECK, 0m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Call_Works()
        {
            HandAction handAction = Parser.ParseRegularActionLine("M00700 calls [$1]", Street.Preflop);

            Assert.AreEqual(new HandAction("M00700", HandActionType.CALL, 1m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Raise_Works()
        {
            HandAction handAction = Parser.ParseRegularActionLine("Jack3bet raises [$6.37]", Street.Preflop);

            Assert.AreEqual(new HandAction("Jack3bet", HandActionType.RAISE, 6.37m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Fold_Works()
        {
            HandAction handAction = Parser.ParseRegularActionLine("hulkhoden1969 folds", Street.Preflop);

            Assert.AreEqual(new HandAction("hulkhoden1969", HandActionType.FOLD, 0m, Street.Preflop), handAction);
        }
    }
}
