using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Compression;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.RegexParser.Base;

namespace HandHistories.Parser.Parsers.RegexParser.PartyPoker
{
    public class PartyHandHistoryRegexParserImpl : HandHistoryRegexParserImplBase
    {                                
        // Mantis Bug 92 - a possible speed optimization is converting these into constants
        // foreach site so compiler can do its magic but how would we do overrides? and would this increase speed?

        public PartyHandHistoryRegexParserImpl() 
            : base()
        {
        }

        public override SiteName SiteName { get { return SiteName.PartyPoker; } }

        public override string TableNameRegex { get { return @"(?<=Table ).*(?= \(Real)"; } }

        public override string TableSizeRegex { get { return @"((?<=Total number of players : [0-9]/)|(?<=Total number of players : 10/))[0-9]+"; } }

        //Mantis Bug 93 make sure this works for hand histories without USD in them
        public override string GameTypeRegex { get { return @"(?<=(USD|GBP|EUR) ).*(?= -)"; } }

        public override string GameNumberRegex { get { return @"(?<=Hand History for Game )\#{0,1}[0-9]*"; } }

        public override string GameLimitRegex { get {  return @"(?<=\n)\$[0-9,]*(\.[0-9]{2}){0,1}"; } }

        public override string GameLimitRegexWithSlash { get { return @"(?<=\n)\$[0-9,]*(\.[0-9]{2}){0,1}/\$[0-9,]*(\.[0-9]{2}){0,1}"; } }

        public override string GameDateRegex { get { return @"(?<= - ).*, [012][0-9]:[0-5][0-9]:[0-5][0-9] [A-Za-z]{0,5} [0-9]{4}";} } 

        public override string DateYearRegex { get { return @"[0-9]{4}"; } }

        public override string DateMonthRegex { get { return @"(January|February|March|April|May|June|July|August|September|October|November|December)"; } }

        public override string DateDayRegex { get { return @"[0-9]{1,2}(?=,)"; } }

        public override string DateTimeRegex { get { return @"[012][0-9]:[0-5][0-9]:[0-5][0-9]"; } }

        public override string BoardRegexFlop { get { return @"(?<=\*\* Dealing Flop \*\* \[).*(?=\])";  } }

        public override string BoardRegexTurn { get { return @"(?<=\*\* Dealing Turn \*\* \[).*(?=\])"; } }

        public override string BoardRegexRiver { get { return @"(?<=\*\* Dealing River \*\* \[).*(?=\])";  } }

        public override string GetHoleCardsRegex(string playerName)
        {
            const string cardHoleCard = "[0-9TJKQA][cdhs]";
            const string regex = @"((?<={playerName} doesn't show \[ )([0-9TJKQAcdhs, ])+)|((?<={playerName} shows \[ )([0-9TJKQAcdhs, ])+)";
            return regex.Replace("{playerName}", playerName).Replace("{Card}", cardHoleCard);
        }
        
        public override string SeatInfoRegex { get { return @"Seat [0-9]+: " + PlayerNameRegex + @" \( \$[0-9,.]+ USD \)"; } }

        public override string SeatInfoPlayerNameRegex { get { return @"((?<=Seat [1-9]: )|(?<=Seat 10: )).*(?= \()"; } }

        public override string SeatInfoStartingStackRegex { get { return @"(?<=\( \$).*(?= USD \))"; } }

        public override string SeatInfoSeatNumberRegex { get { return @"(?<=Seat )[0-9]+(?=:)"; } }

    //    public override string ShowDownHoleCardsLineRegex { get { return @".* shows \[ [2-9TJQKA][cdhs], [2-9TJQKA][cdhs] \]"; } }

      //  public override string ShowDownHoleCardsScreenname { get { return @".*(?= shows)";  } }

      //  public override string ShowDownHoleCardsRegex { get { return @"(?<=\[ ).*(?= \])";  } }

        public override string ButtonLocationRegex { get { return @"(?<=Seat )[0-9]+(?= is the button)";  } }
        
        public override bool IsValidHand(string handText)
        {
            return (handText.Contains(" wins ")) && (handText.Contains("Connection Lost due to some reason") == false);
        }

        public override IEnumerable<string> SplitUpMultipleHands(string rawHandHistories)
        {
            rawHandHistories = rawHandHistories.Replace("\r\n\r\n", "~");
            rawHandHistories = rawHandHistories.Replace("\n\n", "~");

            string[] splitText = rawHandHistories.Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);

            return splitText;       
        }

        public override Limit ParseLimit(string handText)
        {
            GameType gameType = ParseGameType(handText);
            string tableName = ParseTableName(handText);

            return ParseLimit(gameType, tableName, handText);
        }


        private Limit ParseLimit(GameType gameType, string tableName, string handText)
        {
            try
            {
                var gameLimitNoSlash = Regex.Match(handText, GameLimitRegex).Value;
                var gameLimitWithSlash = Regex.Match(handText, GameLimitRegexWithSlash).Value;

                gameLimitNoSlash = gameLimitNoSlash.Replace("$", "").Replace("*", "").Replace(",", "");
                gameLimitWithSlash = gameLimitWithSlash.Replace("$", "").Replace("*", "").Replace(",", "");

                // Handle 20BB tables, due to Party putting the limit up as 40% of the actual
                // limit. So for instance 20BB party $100NL the limit is displayed as $40NL.
                // No idea why this is so.               
                if (tableName.StartsWith("20BB"))
                {
                    gameLimitNoSlash = ((int)((Int32.Parse(gameLimitNoSlash) / 4.0) * 10.0)).ToString();
                }
                // If there is a game limit with a slash then the limit is in form $2/$4
                // then convert the game limit into a game type without a slash which would be 400 for 2/4
                else if (!string.IsNullOrWhiteSpace(gameLimitWithSlash))
                {
                    // get the bb amount
                    gameLimitNoSlash = gameLimitWithSlash.Split('/')[1];

                    // turn into a buy in only if the game type is not fixed limit
                    // as there are no buy ins in fixed limit.
                    if (gameType != GameType.FixedLimitHoldem)
                        gameLimitNoSlash = (decimal.Parse(gameLimitNoSlash, System.Globalization.CultureInfo.InvariantCulture) * 100).ToString();
                }

                Currency currency = ParseCurrency(handText);

                decimal bigBlind;

                // If its fixed limit we can return now as we don't have our
                // data in buy in format so don't need to down convert to
                // a sb/bb
                if (gameType == GameType.FixedLimitHoldem)
                {
                    bigBlind = decimal.Parse(gameLimitNoSlash, System.Globalization.CultureInfo.InvariantCulture);

                    return Limit.FromSmallBlindBigBlind(bigBlind/2.0m, bigBlind, currency);
                }

                // All other game types

                decimal buyIn = decimal.Parse(gameLimitNoSlash, System.Globalization.CultureInfo.InvariantCulture);
                bigBlind = buyIn/100.0m;

                if (bigBlind == 0.25m)
                {
                    return Limit.FromSmallBlindBigBlind(0.10m, 0.25m, currency);
                }

                return Limit.FromSmallBlindBigBlind(bigBlind / 2.0m, bigBlind, currency);
            }
            catch (Exception exception)
            {
                throw new LimitException(handText, "ParseLimit: " + exception.Message);
            }
        }
    }
}
