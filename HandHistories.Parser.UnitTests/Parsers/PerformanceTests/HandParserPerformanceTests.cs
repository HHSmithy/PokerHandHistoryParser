using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System;

namespace HandHistories.Parser.UnitTests.Parsers.PerformanceTest
{
    [TestFixture("PokerStars")]
    [TestFixture("OnGame")]
    [TestFixture("IPoker")]
    [TestFixture("Pacific")]
    [TestFixture("Merge")]
    [TestFixture("Entraction")]
    [TestFixture("FullTilt")]
    [TestFixture("MicroGaming")]
    [TestFixture("Winamax")]
    [TestFixture("BossMedia")]
    [TestFixture("PartyPoker")]
    internal class HandParserPerformanceTest : HandHistoryParserBaseTests
    {
        public HandParserPerformanceTest(string site)
            : base(site)
        {
        }

        [Test]
        public void Performance_Parse1000Hands()
        {
            const int handsCount = 1000;
            string handText = SampleHandHistoryRepository.GetGeneralHandHistoryText(PokerFormat.CashGame, Site, "GeneralHand");

            //Warmup
            GetParser().ParseFullHandHistory(handText);

            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            for (int i = 0; i < handsCount; i++)
            {
                GetParser().ParseFullHandHistory(handText);
            }

            timer.Stop();
            double elapsedMillis = timer.Elapsed.TotalMilliseconds;
            Console.Write(Site + ": Time in milliseconds to parse " + handsCount + " hands: " + elapsedMillis);
            bool ExecutionTimeLessThanFiveSeconds = (elapsedMillis < 1000);
            Assert.IsTrue(ExecutionTimeLessThanFiveSeconds, "Should take less than one seconds: Time Taken: " + elapsedMillis + "ms");
        }
    }
}