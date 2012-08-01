using System;
using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;

namespace HandHistories.Parser.Parsers.RegexParser.PartyPoker
{
    public abstract class SiteActionRegexesBase
    {
        protected abstract string ChatRegex { get; }

        protected abstract string BetsRegex { get; }
        protected abstract string CallsRegex { get; }
        protected abstract string FoldRegex { get; }
        protected abstract string RaiseToRegex { get; }
        protected abstract string ChecksRegex { get; }
        protected abstract string AllInRegex { get; }     

        protected abstract string PostsSmallBlindRegex { get; }
        protected abstract string PostsBigBlindRegex { get; }
        protected abstract string PostsRegex { get; }
        protected abstract string AntesRegex { get; }        

        protected abstract string WinsSidePotRegex { get; }
        protected abstract string WinsPotRegex { get; }
        protected abstract string WinsTheLowRegex { get;  }
        protected abstract string TiesSidePotRegex { get; }
        protected abstract string TiesPotRegex { get; }

        protected abstract string ShowsRegex { get; }
        protected abstract string ShowsForLow { get; }
        protected abstract string MucksRegex { get; }

        protected abstract string UncalledBetRegex { get; }
        
        protected abstract string TimesOutRegex { get; }
        protected abstract string SecondsToReconnectRegex { get; }
        protected abstract string DisconnectedRegex { get; }
        protected abstract string RequestsTimeRegex { get; }
        protected abstract string SecondsLeftToActRegex { get; }

        protected abstract string AddsRegex { get; }
        protected abstract string StandsUpRegex { get; }
        protected abstract string SitsDownRegex { get; }        
        protected abstract string SittingOutRegex { get; }
        protected abstract string ReconnectedRegex { get; }
        protected abstract string HasReturnedRegex { get; }                  
        
        public virtual string AmountRegex { get { return @"(?<=\$)[0-9.,]+(?= USD)"; } }
        public virtual string PostAmountRegex { get { return @"(?<=\$)[0-9.,]+(?=].)"; } }

        // Note: This is FTP only.
        protected virtual string IsFeelingRegex { get { return null; } }

        private readonly IDictionary<HandActionType, HandActionTypeRegexPair> _actionsToQuery;        

        protected SiteActionRegexesBase()
        {
            _actionsToQuery = GetActionsDictionary();

            _possiblePreFlopActions = GetParsablePreflopActions();
            _possibleFlopActions = GetParsableFlopActions();
            _possibleTurnActions = GetParsableFlopActions();
            _possibleRiverActions = GetParsableRiverActions();
        }

        private IDictionary<HandActionType, HandActionTypeRegexPair> GetActionsDictionary()
        {
            return new Dictionary<HandActionType, HandActionTypeRegexPair>()
                                 {
                                     {HandActionType.FOLD, new HandActionTypeRegexPair(HandActionType.FOLD, FoldRegex)},
                                     {HandActionType.ADDS, new HandActionTypeRegexPair(HandActionType.ADDS, AddsRegex)},
                                     {HandActionType.WINS_SIDE_POT, new HandActionTypeRegexPair(HandActionType.WINS_SIDE_POT, WinsSidePotRegex)},
                                     {HandActionType.WINS, new HandActionTypeRegexPair(HandActionType.WINS, WinsPotRegex)},
                                     {HandActionType.WINS_THE_LOW, new HandActionTypeRegexPair(HandActionType.WINS_THE_LOW, WinsTheLowRegex)},
                                     {HandActionType.UNCALLED_BET, new HandActionTypeRegexPair(HandActionType.UNCALLED_BET, UncalledBetRegex)},
                                     {HandActionType.TIMED_OUT, new HandActionTypeRegexPair(HandActionType.TIMED_OUT, TimesOutRegex)},
                                     {HandActionType.TIES_SIDE_POT, new HandActionTypeRegexPair(HandActionType.TIES_SIDE_POT, TiesSidePotRegex)},
                                     {HandActionType.TIES, new HandActionTypeRegexPair(HandActionType.TIES, TiesPotRegex)},
                                     {HandActionType.STANDS_UP, new HandActionTypeRegexPair(HandActionType.STANDS_UP, StandsUpRegex)},
                                     {HandActionType.SMALL_BLIND, new HandActionTypeRegexPair(HandActionType.SMALL_BLIND, PostsSmallBlindRegex)},
                                     {HandActionType.SITTING_OUT, new HandActionTypeRegexPair(HandActionType.SITTING_OUT, SittingOutRegex)},
                                     {HandActionType.SITS_DOWN, new HandActionTypeRegexPair(HandActionType.SITS_DOWN, SitsDownRegex)},
                                     {HandActionType.SHOW, new HandActionTypeRegexPair(HandActionType.SHOW, ShowsRegex)},
                                     {HandActionType.SHOWS_FOR_LOW, new HandActionTypeRegexPair(HandActionType.SHOWS_FOR_LOW, ShowsForLow)},
                                     {HandActionType.SECONDS_TO_RECONNECT, new HandActionTypeRegexPair(HandActionType.SECONDS_TO_RECONNECT, SecondsToReconnectRegex)},
                                     {HandActionType.RETURNED, new HandActionTypeRegexPair(HandActionType.RETURNED, HasReturnedRegex)},
                                     {HandActionType.REQUEST_TIME, new HandActionTypeRegexPair(HandActionType.REQUEST_TIME, RequestsTimeRegex)},
                                     {HandActionType.RECONNECTED, new HandActionTypeRegexPair(HandActionType.RECONNECTED, ReconnectedRegex)},
                                     {HandActionType.RAISE, new HandActionTypeRegexPair(HandActionType.RAISE, RaiseToRegex)},
                                     {HandActionType.POSTS, new HandActionTypeRegexPair(HandActionType.POSTS, PostsRegex)},
                                     {HandActionType.ALL_IN, new HandActionTypeRegexPair(HandActionType.ALL_IN, AllInRegex)},                  
                                     {HandActionType.ANTE, new HandActionTypeRegexPair(HandActionType.ANTE, AntesRegex)},
                                     {HandActionType.BIG_BLIND, new HandActionTypeRegexPair(HandActionType.BIG_BLIND, PostsBigBlindRegex)},
                                     {HandActionType.BET, new HandActionTypeRegexPair(HandActionType.BET, BetsRegex)},
                                     {HandActionType.CALL, new HandActionTypeRegexPair(HandActionType.CALL, CallsRegex)},
                                     {HandActionType.CHAT, new HandActionTypeRegexPair(HandActionType.CHAT, ChatRegex)},
                                     {HandActionType.CHECK, new HandActionTypeRegexPair(HandActionType.CHECK, ChecksRegex)},
                                     {HandActionType.DISCONNECTED, new HandActionTypeRegexPair(HandActionType.DISCONNECTED, DisconnectedRegex)},
                                     {HandActionType.FEELING_CHANGE, new HandActionTypeRegexPair(HandActionType.FEELING_CHANGE, IsFeelingRegex)},
                                     {HandActionType.FIFTEEN_SECONDS_LEFT, new HandActionTypeRegexPair(HandActionType.FIFTEEN_SECONDS_LEFT, SecondsLeftToActRegex)},
                                     {HandActionType.FIVE_SECONDS_LEFT, new HandActionTypeRegexPair(HandActionType.FIVE_SECONDS_LEFT, SecondsLeftToActRegex)},
                                     {HandActionType.MUCKS, new HandActionTypeRegexPair(HandActionType.MUCKS, MucksRegex)}                                     
                                 };
        }

        protected List<HandActionTypeRegexPair> GetParsablePreflopActions()
        {
            return new List<HandActionTypeRegexPair>()
                                         {
                                             _actionsToQuery[HandActionType.CHAT],
                                             _actionsToQuery[HandActionType.FOLD],
                                             _actionsToQuery[HandActionType.SMALL_BLIND],
                                             _actionsToQuery[HandActionType.BIG_BLIND],
                                             _actionsToQuery[HandActionType.RAISE],
                                             _actionsToQuery[HandActionType.CALL],
                                             _actionsToQuery[HandActionType.CHECK],
                                             _actionsToQuery[HandActionType.ALL_IN],                                             
                                             _actionsToQuery[HandActionType.POSTS],
                                             _actionsToQuery[HandActionType.ANTE],
                                             _actionsToQuery[HandActionType.SHOW],
                                             _actionsToQuery[HandActionType.WINS],
                                             _actionsToQuery[HandActionType.WINS_SIDE_POT],
                                             _actionsToQuery[HandActionType.MUCKS],
                                             _actionsToQuery[HandActionType.UNCALLED_BET],
                                             _actionsToQuery[HandActionType.TIES],
                                             _actionsToQuery[HandActionType.TIES_SIDE_POT],
                                             _actionsToQuery[HandActionType.SHOWS_FOR_LOW],
                                             _actionsToQuery[HandActionType.WINS_THE_LOW],
                                         };          
        }

        protected List<HandActionTypeRegexPair> GetParsableFlopActions()
        {
            return new List<HandActionTypeRegexPair>()
                                         {
                                             _actionsToQuery[HandActionType.CHAT],
                                             _actionsToQuery[HandActionType.CHECK],
                                             _actionsToQuery[HandActionType.BET],
                                             _actionsToQuery[HandActionType.FOLD],                                             
                                             _actionsToQuery[HandActionType.RAISE],
                                             _actionsToQuery[HandActionType.CALL],
                                             _actionsToQuery[HandActionType.ALL_IN],
                                             _actionsToQuery[HandActionType.SHOW],
                                             _actionsToQuery[HandActionType.WINS],
                                             _actionsToQuery[HandActionType.WINS_SIDE_POT],
                                             _actionsToQuery[HandActionType.MUCKS],
                                             _actionsToQuery[HandActionType.UNCALLED_BET],
                                             _actionsToQuery[HandActionType.TIES],
                                             _actionsToQuery[HandActionType.TIES_SIDE_POT],
                                             _actionsToQuery[HandActionType.SHOWS_FOR_LOW],
                                             _actionsToQuery[HandActionType.WINS_THE_LOW],
                                         };
        }

        private List<HandActionTypeRegexPair> GetParsableRiverActions()
        {
            return new List<HandActionTypeRegexPair>()
                                         {
                                             _actionsToQuery[HandActionType.CHAT],
                                             _actionsToQuery[HandActionType.CHECK],
                                             _actionsToQuery[HandActionType.BET],
                                             _actionsToQuery[HandActionType.FOLD],                                             
                                             _actionsToQuery[HandActionType.RAISE],
                                             _actionsToQuery[HandActionType.CALL],
                                             _actionsToQuery[HandActionType.ALL_IN],
                                             _actionsToQuery[HandActionType.SHOW],
                                             _actionsToQuery[HandActionType.WINS],
                                             _actionsToQuery[HandActionType.WINS_SIDE_POT],
                                             _actionsToQuery[HandActionType.MUCKS],
                                             _actionsToQuery[HandActionType.UNCALLED_BET],
                                             _actionsToQuery[HandActionType.TIES],
                                             _actionsToQuery[HandActionType.TIES_SIDE_POT],
                                             _actionsToQuery[HandActionType.SHOWS_FOR_LOW],
                                             _actionsToQuery[HandActionType.WINS_THE_LOW],
                                         };
        }

        private readonly List<HandActionTypeRegexPair> _possiblePreFlopActions;
        private readonly List<HandActionTypeRegexPair> _possibleFlopActions;
        private readonly List<HandActionTypeRegexPair> _possibleTurnActions;
        private readonly List<HandActionTypeRegexPair> _possibleRiverActions;

        public IEnumerable<HandActionTypeRegexPair> GetPossibleActions(Street street)
        {
            switch (street)
            {
                case Street.Preflop:
                    return _possiblePreFlopActions;
                case Street.Flop:
                    return _possibleFlopActions;
                case Street.Turn:
                    return _possibleTurnActions;
                case Street.River:
                    return _possibleRiverActions;
                default:
                    throw new NotImplementedException("GetPossibleActions: Unknown street " + street);
            }
        }
    }
}
