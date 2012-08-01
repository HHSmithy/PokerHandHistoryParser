using System;
using System.IO;
using System.Linq;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Parsers.Pacific;
using NUnit.Framework;

namespace HandHistories.Parser.UnitTests.Parsers.Pacific
{
    [TestFixture]
    public class PacificParserTests
    {
        public PacificHistorySummaryParserImpl ParserImpl;
        public string MultipleHandsText;
        public string Fl100_4Seat_Text;
        public string Fl100_6Seat_Text;
        public string Fl12_6Seat_Text;
        public string Fl300_2Seat_Text;
        public string Nl100_8Seat_Text;
        public string Nl5000_5Seat_Text;

        [SetUp]
        public void Setup()
        {
            MultipleHandsText = File.ReadAllText(@"HandHistories/Pacific/MultipleHands.txt");
            Fl100_4Seat_Text = File.ReadAllText(@"HandHistories/Pacific/FL100-4Seat.txt");
            Fl100_6Seat_Text = File.ReadAllText(@"HandHistories/Pacific/FL100-6Seat.txt");
            Fl12_6Seat_Text = File.ReadAllText(@"HandHistories/Pacific/FL12-6Seat.txt");
            Fl300_2Seat_Text = File.ReadAllText(@"HandHistories/Pacific/FL300-2Seat.txt");
            Nl100_8Seat_Text = File.ReadAllText(@"HandHistories/Pacific/NL100-8Seat.txt");
            Nl5000_5Seat_Text = File.ReadAllText(@"HandHistories/Pacific/NL5000-5Seat.txt");

            ParserImpl = new PacificHistorySummaryParserImpl();
        }

        [Test]
        public void Test_SplittingHands_Works()
        {
            var handStrings = ParserImpl.SplitUpMultipleHands(MultipleHandsText);

            var handList = handStrings.ToList();
            Assert.AreEqual(12, handList.Count);
        }

        [Test]
        public void Test_ParseTableName_Works()
        {
            string nlTableName = ParserImpl.ParseTableName(Nl100_8Seat_Text);
            string flTableName = ParserImpl.ParseTableName(Fl100_4Seat_Text);
            Assert.AreEqual("Leeuwarden (Real Money)", nlTableName);
            Assert.AreEqual("Basel (Real Money)", flTableName);
        }

        [Test]
        public void Test_ParseDate_Works()
        {
            var date = ParserImpl.ParseDateUtc(Nl100_8Seat_Text);

            //Date looks like: 04 02 2012 23:59:57
            DateTime comparisonDate = new DateTime(2012, 2, 4, 23, 59, 57, 0, DateTimeKind.Utc);
            Assert.AreEqual(comparisonDate, date);
        }

        [Test]
        public void Test_ParseLimit_Works()
        {
            var flLowLimit = ParserImpl.ParseLimit(Fl12_6Seat_Text);
            var flMidLimit = ParserImpl.ParseLimit(Fl300_2Seat_Text);
            var nlMidLimit = ParserImpl.ParseLimit(Nl100_8Seat_Text);
            var nlHighLimit = ParserImpl.ParseLimit(Nl5000_5Seat_Text);

            Assert.AreEqual(Limit.FromSmallBlindBigBlind(0.06m, .12m, Currency.USD), flLowLimit);
            Assert.AreEqual(Limit.FromSmallBlindBigBlind(1.5m, 3, Currency.USD), flMidLimit);
            Assert.AreEqual(Limit.FromSmallBlindBigBlind(.5m, 1, Currency.USD), nlMidLimit);
            Assert.AreEqual(Limit.FromSmallBlindBigBlind(25m, 50, Currency.USD), nlHighLimit);            
        }

        [Test]
        public void Test_ParseSeatCount_Works()
        {
            var SeatType2 = ParserImpl.ParseSeatType(Fl300_2Seat_Text);
            var SeatType4 = ParserImpl.ParseSeatType(Fl100_4Seat_Text);
            var SeatType5 = ParserImpl.ParseSeatType(Nl5000_5Seat_Text);
            var SeatType6 = ParserImpl.ParseSeatType(Fl12_6Seat_Text);
            var SeatType8 = ParserImpl.ParseSeatType(Nl100_8Seat_Text);

            Assert.AreEqual(SeatType.FromMaxPlayers(2), SeatType2);
            Assert.AreEqual(SeatType.FromMaxPlayers(4), SeatType4);
            Assert.AreEqual(SeatType.FromMaxPlayers(5), SeatType5);
            Assert.AreEqual(SeatType.FromMaxPlayers(6), SeatType6);
            Assert.AreEqual(SeatType.FromMaxPlayers(8), SeatType8);
        }

        [Test]
        public void Test_ParseGameType_Works()
        {
            var nlGameType = ParserImpl.ParseGameType(Nl5000_5Seat_Text);
            var flGameType = ParserImpl.ParseGameType(Fl300_2Seat_Text);

            Assert.AreEqual(GameType.NoLimitHoldem, nlGameType);
            Assert.AreEqual(GameType.FixedLimitHoldem, flGameType);
        }
    }
}
