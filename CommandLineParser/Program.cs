using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers.Factory;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.FastParser._888;
using HandHistories.Parser.Parsers.FastParser.Base;

namespace CommandLineParser
{
    class Program
    {


        static void Main(string[] args)
        {
            IHandHistoryParserFactory handHistoryParserFactory = new HandHistoryParserFactoryImpl();

            // Get the correct parser from the factory.
            var handHistoryParser = new Poker888FastParserImpl();
            HandHistoryParserFastImpl fastParser = handHistoryParser as HandHistoryParserFastImpl;


            try
            {
                // The true causes hand-parse errors to get thrown. If this is false, hand-errors will
                // be silent and null will be returned.
                string fileText = new StreamReader(args[0]).ReadToEnd();
                var hands = fastParser.SplitUpMultipleHandsToLines(fileText);
                var outputFile = new StreamWriter(args[0] + ".csv");
                outputFile.WriteLine("DateOfHandUtc,HandId,DealerButtonPosition,TableName,GameDescription,NumPlayersActive,NumPlayersSeated,ActionNumber,Amount,HandActionType,PlayerName,Street,IsAggressiveAction,IsAllIn,IsAllInAction,IsBlinds,IsGameAction,IsPreFlopRaise,IsRaise,IsWinningsAction");
                foreach (var hand in hands)
                {
                    var parsedHand = fastParser.ParseFullHandHistory(hand, true);

                    foreach (var action in parsedHand.HandActions)
                    {
						outputFile.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}",
                            parsedHand.DateOfHandUtc
                            , parsedHand.HandId
                            , parsedHand.DealerButtonPosition
                            , parsedHand.TableName
                            , parsedHand.GameDescription
                            , parsedHand.NumPlayersActive
                            , parsedHand.NumPlayersSeated
                            , action.ActionNumber
                            , action.Amount
                            , action.HandActionType
                            , action.PlayerName
                            , action.Street
                            , action.IsAggressiveAction
                            , action.IsAllIn
                            , action.IsAllInAction
                            , action.IsBlinds
                            , action.IsGameAction
                            , action.IsPreFlopRaise
                            , action.IsRaise
                            , action.IsWinningsAction
);
                    }

                }


            }
            catch (Exception ex) // Catch hand-parsing exceptions
            {
                Console.WriteLine("Parsing Error: {0}", ex.Message); // Example logging.
            }
        }
    }
}
