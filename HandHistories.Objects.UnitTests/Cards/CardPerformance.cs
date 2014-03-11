using HandHistories.Objects.Cards;
using NUnit.Framework;
using System;

namespace HandHistories.Objects.UnitTests.Cards
{
    class CardPerformanceConstructionTest
    {
        [Test]
        public void NewCard_Performance()
        {
            const int numberOfCards = 20000000;
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            for (int i = 0; i < numberOfCards; i++)
            {
                Card card = new Card("T", "h");
            }
            timer.Stop();
            double elapsedMillis = timer.Elapsed.TotalMilliseconds;
            Console.Write("Time in milliseconds to generate " + numberOfCards + " cards: " + elapsedMillis);
            bool ExecutionTimeLessThanFiveSeconds = (elapsedMillis < 1000);
            Assert.IsTrue(ExecutionTimeLessThanFiveSeconds, "Should take less than one seconds: Time Taken: " + elapsedMillis + "ms");
        }
    }
}
