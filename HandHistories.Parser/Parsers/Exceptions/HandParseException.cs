using System;

namespace HandHistories.Parser.Parsers.Exceptions
{
    public abstract class HandParseException : Exception
    {
        private readonly string _handText;

        protected HandParseException(string handText)
            : base()
        {
            _handText = handText;
        }

        protected HandParseException(string handText, string message) : base(message)
        {
            _handText = handText;
        }

        public string HandText
        {
            get { return _handText; }
        }
    }

    public class InvalidHandException : HandParseException
    {
        public InvalidHandException(string handText) : base(handText) 
        {
        }
    }

    public class HandActionException : HandParseException
    {
        public HandActionException(string handText, string message) : base(handText, message)
        {
        }
    }

    public class ExtraHandParsingAction : HandParseException
    {
        public ExtraHandParsingAction(string handText)
            : base(handText)
        {
        }
    }

    public class PlayersException : HandParseException
    {
        public PlayersException(string handText, string message) : base(handText, message)
        {
        }
    }

    public class UnrecognizedGameTypeException : HandParseException
    {
        public UnrecognizedGameTypeException(string handText, string message) : base(handText, message)
        {
        }
    }

    public class ParseHandDateException : HandParseException
    {
        public ParseHandDateException(string handText, string message) : base(handText, message)
        {
        }
    }

    public class HandIdException : HandParseException
    {
        public HandIdException(string handText, string message) : base(handText, message)
        {
        }
    }

    public class TournamentIdException : HandParseException
    {
        public TournamentIdException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class TableNameException : HandParseException
    {
        public TableNameException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class PokerFormatException : HandParseException
    {
        public PokerFormatException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class SeatTypeException : HandParseException
    {
        public SeatTypeException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class TableTypeException : HandParseException
    {
        public TableTypeException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class LimitException : HandParseException
    {
        public LimitException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class BuyinException : HandParseException
    {
        public BuyinException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class CurrencyException : HandParseException
    {
        public CurrencyException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class CardException :HandParseException
    {
        public CardException(string handText, string message)
            : base(handText, message)
        {
        }
    }

    public class RunItTwiceHandException : HandParseException
    {
        public RunItTwiceHandException()
            : base("", "Do not currently support Run It Twice")
        {
        }
    }
}
