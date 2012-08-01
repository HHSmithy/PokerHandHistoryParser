using System;
using System.IO;
using System.Linq;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Parsers.Entraction;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.Entraction
{
    [TestFixture]
    public class EntractionParserTests
    {
        public EntractionHistorySummaryParserImpl ParserImpl;
        public string MultipleHandsText;
        public string Holdem6MaxHandText;
        public string Holdem20FLFullHandText;
        public string Omaha50NL6MaxHandText;

        [SetUp]
        public void Setup()
        {
            MultipleHandsText = File.ReadAllText(@"HandHistories/Entraction/MultipleHands.txt");
            Holdem6MaxHandText = File.ReadAllText(@"HandHistories/Entraction/Holdem100NL6Max.txt");
            Holdem20FLFullHandText = File.ReadAllText(@"HandHistories/Entraction/Holdem20FLFull.txt");
            Omaha50NL6MaxHandText = File.ReadAllText(@"HandHistories/Entraction/Omaha50NL6Max.txt");
            ParserImpl = new EntractionHistorySummaryParserImpl();
        }

        [Test]
        public void Test_SplittingHands_Works()
        {

            var handStrings = ParserImpl.SplitUpMultipleHands(MultipleHandsText);

            var handList = handStrings.ToList();
            Assert.AreEqual(11, handList.Count);
        }

        [Test]
        public void Test_ParseTableName_Works()
        {
            string nlTableName = ParserImpl.ParseTableName(Holdem6MaxHandText);
            string flTableName = ParserImpl.ParseTableName(Holdem20FLFullHandText);
            string ploTableName = ParserImpl.ParseTableName(Omaha50NL6MaxHandText);
            Assert.AreEqual("Abanilla", nlTableName);
            Assert.AreEqual("Johannedal", flTableName);
            Assert.AreEqual("Vac", ploTableName);
        }

        [Test]
        public void Test_ParseDate_Works()
        {
            var date = ParserImpl.ParseDateUtc(Holdem6MaxHandText);

            //Date looks like: 2012-02-06 20:01:44 GMT+01:00 
            DateTime comparisonDate = new DateTime(2012, 2, 6, 19, 1, 44, 0, DateTimeKind.Utc);
            Assert.AreEqual(comparisonDate, date);
        }

        [Test]
        public void Test_ParseLimit_Works()
        {
            var nlLimit = ParserImpl.ParseLimit(Holdem6MaxHandText);
            var flLimit = ParserImpl.ParseLimit(Holdem20FLFullHandText);
            var ploLimit = ParserImpl.ParseLimit(Omaha50NL6MaxHandText);
            Assert.AreEqual(Limit.FromSmallBlindBigBlind(.5m, 1, Currency.EURO), nlLimit);
            Assert.AreEqual(Limit.FromSmallBlindBigBlind(.1m, .2m, Currency.EURO), flLimit);
            Assert.AreEqual(Limit.FromSmallBlindBigBlind(.25m, .5m, Currency.EURO), ploLimit);
        }

        [Test]
        public void Test_ParseSeatCount_Works()
        {
            var nlSeatType = ParserImpl.ParseSeatType(Holdem6MaxHandText);
            var flSeatType = ParserImpl.ParseSeatType(Holdem20FLFullHandText);
            var ploSeatType = ParserImpl.ParseSeatType(Omaha50NL6MaxHandText);
            Assert.AreEqual(SeatType.FromMaxPlayers(6), nlSeatType);
            Assert.AreEqual(SeatType.FromMaxPlayers(10), flSeatType);
            Assert.AreEqual(SeatType.FromMaxPlayers(6), ploSeatType);
        }

        [Test]
        public void Test_ParseGameType_Works()
        {
            var nlGameType = ParserImpl.ParseGameType(Holdem6MaxHandText);
            var flGameType = ParserImpl.ParseGameType(Holdem20FLFullHandText);
            var ploGameType = ParserImpl.ParseGameType(Omaha50NL6MaxHandText);
            Assert.AreEqual(GameType.NoLimitHoldem, nlGameType);
            Assert.AreEqual(GameType.FixedLimitHoldem, flGameType);
            Assert.AreEqual(GameType.PotLimitOmaha, ploGameType);
        }
    }
}
