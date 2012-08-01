using System;

namespace HandHistories.Objects.GameDescription
{
    public class LimitGameTypePair
    {
        public GameType GameType { get; private set; }
        public Limit Limit { get; private set; }

        public LimitGameTypePair(Limit limit, GameType gameType)
        {
            Limit = limit;
            GameType = gameType;
        }        

        public static LimitGameTypePair Parse(string limitGamePair)
        {
            string limitString = limitGamePair.Split('_')[0];

            string gameTypeString = limitGamePair.Split('_')[1];

            GameType gameType = (GameType) Enum.Parse(typeof (GameType), gameTypeString);

            Limit limit = Limit.ParseDbSafeString(limitString);

            return new LimitGameTypePair(limit, gameType);
        }

        public override string ToString()
        {
            return Limit.ToDbSafeString() + "_" + GameType;
        }

        public string ToDisplayName()
        {
            return Limit.ToString() + " " + GameTypeUtils.GetShortName(GameType);
        }

        public string ToBuyinFormatString()
        {
            return "$" + (int)(Limit.BigBlind * 100) + " " + GameTypeUtils.GetShortName(GameType);
        }

        public override bool Equals(object obj)
        {
            return obj.ToString().Equals(ToString());
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}