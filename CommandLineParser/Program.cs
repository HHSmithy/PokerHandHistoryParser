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
using HandEvaluator;

namespace CommandLineParser
{
    // the design on "class Card"  "class Hand" and "class HandEvaluator" need some serious re-work.  Stole this code from 7 years ago and it sucks but it works.


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
                    outputFile.WriteLine("DateOfHandUtc,HandId,DealerButtonPosition,TableName,GameDescription,NumPlayersActive,NumPlayersSeated,Rake,ComumnityCards,TotalPot,PlayerName,HoleCards,StartingStack,SeatNumber,ActionNumber,Amount,HandActionType,Outs,CardOuts,CurrentHandRank,currentPostSize,Street,IsAggressiveAction,IsAllIn,IsAllInAction,IsBlinds,IsGameAction,IsPreFlopRaise,IsRaise,IsWinningsAction");
                    foreach (var hand in hands)
                    {
                        var parsedHand = fastParser.ParseFullHandHistory(hand, true);

                        // probably not the best way to do this. This should be added to the ParseHandActions function or something.
                        decimal currentPotSize = 0;
                        int actionNumber = 0;
                        foreach (var action in parsedHand.HandActions)
                        {
                            // I tried to do this (this = 'actionNumber++') rtdfgcvdrt properly via the ParseHandActions function in Poker888FastParserImpl.cs file to be 1-1 with the raw handlines, however
                            // I was getting some un-expected behavior when the action was all in and the winning action the handline index would be 0 so you would end up with
                            // actions sequences like 1,2,3,4,5,6,0,8,9,10.
                            actionNumber++;
                            if (action.HandActionType == HandHistories.Objects.Actions.HandActionType.ANTE
                                || action.HandActionType == HandHistories.Objects.Actions.HandActionType.BET
                                || action.HandActionType == HandHistories.Objects.Actions.HandActionType.SMALL_BLIND
                                || action.HandActionType == HandHistories.Objects.Actions.HandActionType.RAISE
                                || action.HandActionType == HandHistories.Objects.Actions.HandActionType.BIG_BLIND
                                || action.HandActionType == HandHistories.Objects.Actions.HandActionType.CALL
                                || action.HandActionType == HandHistories.Objects.Actions.HandActionType.ALL_IN)
                            {
                                currentPotSize += action.Amount;
                            }

                            // Don't judge me.... this code is just chaos...but it's works. 
                            string handRank;
                            HandEvaluator.HandEvaluator he = new HandEvaluator.HandEvaluator();
                            string handstring = String.Format("{0}{1}", parsedHand.Players.First(p => p.PlayerName.Equals(action.PlayerName)).HoleCards, parsedHand.ComumnityCards);
                            string flopstring;
                            string turnstring;
                            string riverstring;
                            string currenthandstring;
                            Hand h = new Hand();
                            // holecards
                            Hand hc = new Hand();
                            Dictionary<Hand, int> handOuts = new Dictionary<Hand, int>();
                            Dictionary<Hand, List<Card>> cardouts;
                            int outs = 0;
                            double hr = 0.0;
                            string couts = "";
                            if (handstring.Length >= 14)
                            {
                                currenthandstring = "";
                                flopstring = handstring.Substring(4, 6);
                                turnstring = handstring.Substring(10, 2);
                                riverstring = handstring.Substring(12, 2);
                                // build the hand play-by-play so we can get current hand ranking and also build in hand outs. 
                                switch (action.Street)
                                {
                                    case HandHistories.Objects.Cards.Street.Flop:
                                        currenthandstring = handstring.Substring(0, 4) + flopstring;
                                        h = new Hand(currenthandstring, 7, 5);
                                        hc = new Hand(handstring.Substring(0, 4), 7, 5);
                                        hr = he.RankHand(h, out handRank);
                                        handOuts = he.ComputeOuts(h, hc.HAND, new Hand(flopstring, 7, 5).HAND, out cardouts);
                                        outs = handOuts[h];
                                        couts = new Hand(cardouts[h], 52, 5).HandToString();

                                        break;
                                    case HandHistories.Objects.Cards.Street.Turn:
                                        currenthandstring = handstring.Substring(0, 4) + flopstring + turnstring;
                                        h = new Hand(currenthandstring, 7, 5);
                                        hc = new Hand(handstring.Substring(0, 4), 7, 5);

                                        hr = he.RankHand(h, out handRank);
                                        handOuts = he.ComputeOuts(h, hc.HAND, new Hand(flopstring + turnstring, 7, 5).HAND, out cardouts);
                                        outs = handOuts[h];
                                        couts = new Hand(cardouts[h], 52, 5).HandToString();

                                        break;
                                    case HandHistories.Objects.Cards.Street.River:
                                        currenthandstring = handstring.Substring(0, 4) + flopstring + turnstring + riverstring;
                                        h = new Hand(currenthandstring, 7, currenthandstring.Length / 2);
                                        hc = new Hand(handstring.Substring(0, 4), 7, 5);
                                        hr = he.RankHand(h, out handRank);
                                        outs = 0; // No need to compute outs on the river.  No more cards to come. 

                                        break;
                                    default:
                                        hr = 0.0;
                                        outs = 0;

                                        break;
                                }

                            }
                            else
                            {
                                hr = 0.0;
                                outs = 0;
                            }
                            outputFile.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29}",
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
                                , actionNumber
                                , action.Amount
                                , action.HandActionType
                                , outs.ToString()
                                , couts
                                , hr.ToString()
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
                Console.ReadLine();
            }
        }
    }
}
