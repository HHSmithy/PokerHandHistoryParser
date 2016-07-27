using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Parsers.FastParser.PartyPoker;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandHistories.Parser.Parsers.Exceptions;

namespace HandHistories.Parser.UnitTests.Parsers.FastParserTests.PartyPoker
{
    using Parser = PartyPokerFastParserImpl;
    [TestFixture]
    class PartyPokerFastParserActionTests
    {
        [Test]
        public void ParseBlindActionLine_PostingDead_Works()
        {
            HandAction handAction = Parser.ParseBlindAction("skullcrusher99 posts big blind + dead [6 $].");

            Assert.AreEqual(new HandAction("skullcrusher99", HandActionType.POSTS, 6m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseBlindActionLine_PostingDeadEuro_Works()
        {
            HandAction handAction = Parser.ParseBlindAction("skullcrusher99 posts big blind + dead [6 €].");

            Assert.AreEqual(new HandAction("skullcrusher99", HandActionType.POSTS, 6m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseBlindActionLine_PostingAnte_Works()
        {
            HandAction handAction = Parser.ParseBlindAction("saboniiplz posts ante [400]");

            Assert.AreEqual(new HandAction("saboniiplz", HandActionType.ANTE, 400m, Street.Preflop), handAction);
        }
        
        [Test]
        public void ParseBlindActionLine_PostingSmallBlind_Works()
        {
            HandAction handAction = Parser.ParseBlindAction("PUNISHER_DK posts small blind [$2 USD].");

            Assert.AreEqual(new HandAction("PUNISHER_DK", HandActionType.SMALL_BLIND, 2m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseBlindActionLine_PostingBigBlind_Works()
        {
            HandAction handAction = Parser.ParseBlindAction("PUNISHER_DK posts big blind [$4 USD].");

            Assert.AreEqual(new HandAction("PUNISHER_DK", HandActionType.BIG_BLIND, 4m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Bet_Works()
        {
            Street street = Street.Flop;
            HandAction handAction = Parser.ParseRegularActionLine("dr. spaz bets [$1.10 USD]", ref street);

            Assert.AreEqual(new HandAction("dr. spaz", HandActionType.BET, 1.1m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Check_Works()
        {
            Street street = Street.Flop;
            HandAction handAction = Parser.ParseRegularActionLine("jott1982 checks", ref street);

            Assert.AreEqual(new HandAction("jott1982", HandActionType.CHECK, 0m, Street.Flop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Call_Works()
        {
            Street street = Street.Preflop;
            HandAction handAction = Parser.ParseRegularActionLine("TYRANNOMAN1 calls [$1.10 USD]", ref street);

            Assert.AreEqual(new HandAction("TYRANNOMAN1", HandActionType.CALL, 1.1m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Raise_Works()
        {
            Street street = Street.Preflop;
            HandAction handAction = Parser.ParseRegularActionLine("Kelevra_91 raises [$0.25 USD]", ref street);

            Assert.AreEqual(new HandAction("Kelevra_91", HandActionType.RAISE, 0.25m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Fold_Works()
        {
            Street street = Street.Preflop;
            HandAction handAction = Parser.ParseRegularActionLine("hulkhoden1969 folds", ref street);

            Assert.AreEqual(new HandAction("hulkhoden1969", HandActionType.FOLD, 0m, Street.Preflop), handAction);
        }

        [Test]
        public void ParseRegularActionLine_Wins_Works()
        {
            Street street = Street.Preflop;
            HandAction handAction = Parser.ParseRegularActionLine("Player wins $5.18 USD", ref street);

            Assert.AreEqual(new WinningsAction("Player", HandActionType.WINS, 5.18m, 0), handAction);
        }
        
        [Test]
        public void ParseRegularActionLine_WinsTournament_Works()
        {
            Street street = Street.Preflop;
            HandAction handAction = Parser.ParseRegularActionLine("saboniiplz wins 28,304 chips", ref street);

            Assert.AreEqual(new WinningsAction("saboniiplz", HandActionType.WINS, 28304m, 0), handAction);
        }
    }
}
