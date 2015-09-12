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

namespace CommandLineParser
{
    class Program
    {

        
        static void Main(string[] args)
        {
            IHandHistoryParserFactory handHistoryParserFactory = new HandHistoryParserFactoryImpl();

            // Get the correct parser from the factory.
            IHandHistoryParser handHistoryParser = new Poker888FastParserImpl();

            try
            {
                // The true causes hand-parse errors to get thrown. If this is false, hand-errors will
                // be silent and null will be returned.
                string fileText = new StreamReader(args[0]).ReadToEnd();
                HandHistory handHistory = handHistoryParser.ParseFullHandHistory(fileText, true);
            }
            catch (Exception ex) // Catch hand-parsing exceptions
            {
                Console.WriteLine("Parsing Error: {0}", ex.Message); // Example logging.
                Console.ReadLine();
            }
        }
    }
}
