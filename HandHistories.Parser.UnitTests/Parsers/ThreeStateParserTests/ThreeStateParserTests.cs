using HandHistories.Objects.Actions;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.ThreeStateParserTests
{
    abstract internal class ThreeStateParserTests : HandHistoryParserBaseTests
    {
        IThreeStateParser parser;

        protected ThreeStateParserTests(string site)
            : base(site)
        {
            parser = GetParser() as IThreeStateParser;
        }

        protected void TestBlindActions(string fileName, List<HandAction> expectedActions)
        {
            List<HandAction> actions = new List<HandAction>();
            parser.ParseBlindActions(GetBlindTest(fileName), ref actions, 0);

            Assert.AreEqual(expectedActions, actions);
        }

        string[] GetBlindTest(string name)
        {
            return SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, "BlindActionTests", name)
                .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        protected abstract List<HandAction> ExpectedHandActionsAnte { get; }
        protected virtual bool BlindIgnoreActionsTestable { get { return true; } }
        protected virtual bool BlindChatEndingWithNumberTestable { get { return false; } }

        [Test]
        public void ParseBlindActions_Ante()
        {
            TestBlindActions("Ante", ExpectedHandActionsAnte);
        }

        /// <summary>
        /// This tests for all actions that the parser should skip during blinds
        /// </summary>
        [Test]
        public void ParseBlindActions_SkipActions()
        {
            if (!BlindIgnoreActionsTestable)
            {
                Assert.Ignore();
            }
            TestBlindActions("Ignore", new List<HandAction>());
        }

        /// <summary>
        /// This tests for all actions that the parser should skip during blinds
        /// </summary>
        [Test]
        public void ParseBlindActions_ChatEndingWithNumber()
        {
            if (!BlindChatEndingWithNumberTestable)
            {
                Assert.Ignore();
            }
            TestBlindActions("ChatNumber", new List<HandAction>());
        }
    }
}
