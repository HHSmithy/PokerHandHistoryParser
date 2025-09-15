using HandHistories.Objects.Actions;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.UnitTests.Parsers.Base;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Parser.UnitTests.Parsers.ThreeStateParserTests
{
    abstract internal class ThreeStateParserTests : HandHistoryParserBaseTests
    {
        protected IThreeStateParser parser;

        protected ThreeStateParserTests(string site)
            : base(site)
        {
            parser = GetParser() as IThreeStateParser;
        }

        protected void TestBlindActions(string fileName, List<HandAction> expectedActions)
        {
            List<HandAction> actions = new List<HandAction>();
            parser.ParseBlindActions(GetBlindTest(fileName), ref actions, 0);

            ClassicAssert.AreEqual(expectedActions, actions);
        }

        protected string[] GetBlindTest(string name)
        {
            return GetTest("BlindActionTests", name);
        }

        protected string[] GetShowDownTest(string name)
        {
            return GetTest("ShowDownActionTests", name);
        }

        protected void TestShowDownActions(string fileName, List<HandAction> expectedActions)
        {
            List<HandAction> actions = new List<HandAction>();
            parser.ParseShowDown(GetShowDownTest(fileName), ref actions, 0, GameType.Unknown);

            ClassicAssert.AreEqual(expectedActions, actions);
        }

        string[] GetTest(string test, string name)
        {
            return SampleHandHistoryRepository.GetHandExample(PokerFormat.CashGame, Site, test, name)
                .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        protected abstract List<HandAction> ExpectedHandActionsAnte { get; }
        protected virtual bool BlindIgnoreActionsTestable { get { return true; } }
        protected virtual bool BlindChatEndingWithNumberTestable { get { return false; } }

        protected abstract List<HandAction> ExpectedShowDownActions_Wins { get; }
        protected virtual bool ShowDownIgnoreActionsTestable { get { return true; } }

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
                ClassicAssert.Ignore();
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
                ClassicAssert.Ignore();
            }
            TestBlindActions("ChatNumber", new List<HandAction>());
        }

        [Test]
        public void ParseShowDownActions_Wins()
        {
            if (!ShowDownIgnoreActionsTestable)
            {
                ClassicAssert.Ignore();
            }
            TestShowDownActions("Wins", ExpectedShowDownActions_Wins);
        }

        /// <summary>
        /// This tests for all actions that the parser should skip during showdown
        /// </summary>
        [Test]
        public void ParseShowDownActions_SkipActions()
        {
            if (!ShowDownIgnoreActionsTestable)
            {
                ClassicAssert.Ignore();
            }
            TestShowDownActions("Ignore", new List<HandAction>());
        }
    }
}
