using HandHistories.Objects.Hand;

namespace HandHistories.Parser.Compression
{
    public interface IHandHistoryCompressor
    {
        string CompressHandHistory(string fullHandText);
        string CompressHandHistory(HandHistory parsedHandHistory);

        string UncompressHandHistory(string compressedHandHistory);        
    }
}
