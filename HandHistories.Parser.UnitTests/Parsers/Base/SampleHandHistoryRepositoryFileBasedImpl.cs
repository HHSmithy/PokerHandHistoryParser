using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.UnitTests.Utils.IO;

namespace HandHistories.Parser.UnitTests.Parsers.Base
{
    internal class SampleHandHistoryRepositoryFileBasedImpl : ISampleHandHistoryRepository
    {
        private readonly IFileReader _fileReader;

        public SampleHandHistoryRepositoryFileBasedImpl(IFileReader fileReader)
        {
            _fileReader = fileReader;
        }

        public string GetValidHandHandHistoryText(PokerFormat pokerFormat, SiteName siteName, bool isValid)
        {
            return GetHandText(pokerFormat, siteName, "ValidHandTests", (isValid) ? "ValidHand" : "InvalidHand");
        }

        public string GetSeatExampleHandHistoryText(PokerFormat pokerFormat, SiteName siteName, SeatType seatType)
        {
            return GetHandText(pokerFormat, siteName, "Seats", seatType.ToString());
        }

        public string GetLimitExampleHandHistoryText(PokerFormat pokerFormat, SiteName siteName, string fileName)
        {
            return GetHandText(pokerFormat, siteName, "Limits", fileName);
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

        public string GetCommunityCardsHandHistoryText(PokerFormat pokerFormat, SiteName siteName, Street street)
        {
            return GetHandText(pokerFormat, siteName, "StreetTests", street.ToString());
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
            string subFolder = System.IO.Path.Combine(GetSampleHandHistoryFolder(pokerFormat, siteName), subFolderName);
            string path = System.IO.Path.Combine(subFolder, textFileName) + ".txt";

            return _fileReader.ReadAllText(path);
        }

        private string GetSampleHandHistoryFolder(PokerFormat pokerFormat, SiteName siteName)
        {
            return string.Format(@"SampleHandHistories\{0}\{1}\", siteName, pokerFormat);
        }
    }
}