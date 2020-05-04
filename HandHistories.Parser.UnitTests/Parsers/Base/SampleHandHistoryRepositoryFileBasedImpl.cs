using System.Text;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Utils.IO;
using System;

namespace HandHistories.Parser.UnitTests.Parsers.Base
{
    internal class SampleHandHistoryRepositoryFileBasedImpl : ISampleHandHistoryRepository
    {
        private readonly IFileReader _fileReader;
        private readonly string _version;

        public SampleHandHistoryRepositoryFileBasedImpl(IFileReader fileReader)
        {
            _fileReader = fileReader;
        }

        public SampleHandHistoryRepositoryFileBasedImpl(IFileReader fileReader, string version) : this(fileReader)
        {
            _version = version;
        }

        public string GetCancelledHandHandHistoryText(PokerFormat pokerFormat, SiteName siteName)
        {
            return GetHandText(pokerFormat, siteName, "ValidHandTests", "CancelledHand");
        }

        public string GetValidHandHandHistoryText(PokerFormat pokerFormat, SiteName siteName, bool isValid, int testNumber)
        {
            return GetHandText(pokerFormat, siteName, "ValidHandTests", (isValid ? "ValidHand" : "InvalidHand") + "_" + testNumber);
        }

        public string GetSeatExampleHandHistoryText(PokerFormat pokerFormat, SiteName siteName, SeatType seatType)
        {
            return GetHandText(pokerFormat, siteName, "Seats", seatType.ToString());
        }

        public string GetLimitExampleHandHistoryText(PokerFormat pokerFormat, SiteName siteName, string fileName)
        {
            return GetHandText(pokerFormat, siteName, "Limits", fileName);
        }

        public string GetBuyinExampleHandHistoryText(PokerFormat pokerFormat, SiteName siteName, string fileName)
        {
            return GetHandText(pokerFormat, siteName, "Buyins", fileName);
        }

        public string GetTableExampleHandHistoryText(PokerFormat pokerFormat, SiteName siteName, int tableTestNumber)
        {
            return GetHandText(pokerFormat, siteName, "Tables", "Table" + tableTestNumber);
        }

        public string GetGeneralHandHistoryText(PokerFormat pokerFormat, SiteName siteName, string testName)
        {
            return GetHandText(pokerFormat, siteName, "GeneralHands", testName);
        }

        public string GetFormatHandHistoryText(PokerFormat pokerFormat, SiteName siteName, string name)
        {
            return GetHandText(pokerFormat, siteName, "FormatTests", name);
        }

        public string GetGameTypeHandHistoryText(PokerFormat pokerFormat, SiteName siteName, GameType gameType)
        {
            return GetHandText(pokerFormat, siteName, "GameTypeTests", gameType.ToString());
        }

        public string GetCommunityCardsHandHistoryText(PokerFormat pokerFormat, SiteName siteName, Street street, int testNumber)
        {
            return GetHandText(pokerFormat, siteName, "StreetTests", street.ToString() + (testNumber == 1 ? "" : testNumber.ToString()));
        }

        public string GetMultipleHandExampleText(PokerFormat pokerFormat, SiteName siteName, int handCount)
        {
            return GetHandText(pokerFormat, siteName, "MultipleHandsTests", handCount + "MultipleHands");
        }

        public string GetHandExample(PokerFormat pokerFormat, SiteName siteName, string subFolder, string fileName)
        {
            return GetHandText(pokerFormat, siteName, subFolder, fileName);
        }

        private string GetHandText(PokerFormat pokerFormat, SiteName siteName, string subFolderName, string textFileName)
        {
            string workPath = AppDomain.CurrentDomain.BaseDirectory;
            string subFolder = System.IO.Path.Combine(workPath, GetSampleHandHistoryFolder(pokerFormat, siteName), subFolderName);
            string path = System.IO.Path.Combine(subFolder, textFileName) + ".txt";

            if (_fileReader.FileExists(path) == false)
            {
                return null;
            }

            return _fileReader.ReadAllText(path, Encoding.UTF8);
        }

        private string GetSampleHandHistoryFolder(PokerFormat pokerFormat, SiteName siteName)
        {
            return string.Format(@"SampleHandHistories\{0}\{1}\", siteName, pokerFormat);
        }
    }
}