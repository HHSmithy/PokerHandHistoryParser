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
        {// takes a full path to a directory that contains raw *.txt poker history files from 888 poker.
            IHandHistoryParserFactory handHistoryParserFactory = new HandHistoryParserFactoryImpl();

            // Get the correct parser from the factory.
            var handHistoryParser = new Poker888FastParserImpl();
            HandHistoryParserFastImpl fastParser = handHistoryParser as HandHistoryParserFastImpl;


            try
            {
                // The true causes hand-parse errors to get thrown. If this is false, hand-errors will
                // be silent and null will be returned.
                string[] files = Directory.GetFiles(args[0], "*.txt", SearchOption.AllDirectories);
                int fileCount = files.Length;
                foreach (string file in files)
                {
                    Console.WriteLine("number of files left {0} out of {1}", fileCount--, files.Length);
                    string fileText = new StreamReader(file).ReadToEnd();
                    var hands = fastParser.SplitUpMultipleHandsToLines(fileText);
                    var outputFile = new StreamWriter(file + ".csv");
                    outputFile.WriteLine("DateOfHandUtc,HandId,DealerButtonPosition,TableName,GameDescription,NumPlayersActive,NumPlayersSeated,Rake,ComumnityCards,TotalPot,PlayerName,HoleCards,StartingStack,SeatNumber,ActionNumber,Amount,HandActionType,currentPostSize,Street,IsAggressiveAction,IsAllIn,IsAllInAction,IsBlinds,IsGameAction,IsPreFlopRaise,IsRaise,IsWinningsAction");
                    foreach (var hand in hands)
                    {
                        var parsedHand = fastParser.ParseFullHandHistory(hand, true);

                        // probably not the best way to do this. This should be added to the ParseHandActions function or something.
                        decimal currentPotSize = 0;
                        foreach (var action in parsedHand.HandActions)
                        {
                            if (action.HandActionType == HandHistories.Objects.Actions.HandActionType.ANTE
                                || action.HandActionType == HandHistories.Objects.Actions.HandActionType.BET
                                || action.HandActionType == HandHistories.Objects.Actions.HandActionType.SMALL_BLIND
                                || action.HandActionType == HandHistories.Objects.Actions.HandActionType.RAISE
                                || action.HandActionType == HandHistories.Objects.Actions.HandActionType.BIG_BLIND
                                || action.HandActionType == HandHistories.Objects.Actions.HandActionType.CALL
                                || action.HandActionType ==  HandHistories.Objects.Actions.HandActionType.ALL_IN)
                            {
                                currentPotSize += action.Amount;
                            }

                            outputFile.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26}",
                                parsedHand.DateOfHandUtc
                                , parsedHand.HandId
                                , parsedHand.DealerButtonPosition
                                , parsedHand.TableName
                                , parsedHand.GameDescription
                                , parsedHand.NumPlayersActive
                                , parsedHand.NumPlayersSeated
                                , parsedHand.Rake
                                , parsedHand.ComumnityCards
                                , parsedHand.TotalPot
                                , action.PlayerName
                                , parsedHand.Players.First(p => p.PlayerName.Equals(action.PlayerName)).HoleCards
                                , parsedHand.Players.First(p => p.PlayerName.Equals(action.PlayerName)).StartingStack
                                , parsedHand.Players.First(p => p.PlayerName.Equals(action.PlayerName)).SeatNumber
                                , action.ActionNumber
                                , action.Amount
                                , action.HandActionType
                                , currentPotSize
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
                    outputFile.Close();
                }
            }
            catch (Exception ex) // Catch hand-parsing exceptions
            {
                Console.WriteLine("Parsing Error: {0}", ex.Message); // Example logging.
            }
        }
    }
}
