using System;
using System.Runtime.Serialization;

namespace HandHistories.CommonObjects
{    
    [Serializable]
    [DataContract()]
    public class GameDescriptor
    {
        public GameDescriptor()
        {

        }        

        public GameDescriptor(Site site,
                              GameType gameType,
                              Limit limit,
                              TableType tableType,
                              SeatType seatType)
        {
            Site = site;
            GameType = gameType;
            Limit = limit;
            TableType = tableType;
            SeatType = seatType;            
        }

        public GameDescriptor(SiteNames siteName,
                              GameTypes gameType,
                              LimitEnum limitEnum,
                              Currency currency) : this(Site.FromSite(siteName), new GameType(gameType), null, null, null)
        {
            if (limitEnum != LimitEnum.All)
                Limit = Limit.GetLimitFromEnum(limitEnum, currency);
        }

        public GameDescriptor(SiteNames siteName,
                              GameTypes gameType,
                              Limit limit,
                              TableTypes tableType,
                              SeatType seatType)             
            : this(Site.FromSite(siteName), new GameType(gameType), limit, new TableType(tableType), seatType)
        {
            
        }

        [DataMember]
        // Fixed Limit Holdem, Omaha, NL Holdem etc
        public GameType GameType { get; set; }

        [DataMember]
        // 6max, HU, Full Ring etc
        public SeatType SeatType { get; set; }

        [DataMember]
        // deep, cap, regular, ss etc
        public TableType TableType { get; set; }

        [DataMember]
        public Site Site { get; set; }

        [DataMember]
        public Limit Limit { get; set; }
        
        public override bool Equals(object obj)
        {
            GameDescriptor descriptor = obj as GameDescriptor;
            if (descriptor == null) return false;
            return (descriptor.ToString().Equals(this.ToString()));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public String UniqueName
        {
            get { return string.Format("{0},{1},{2},{3},{4}", Site, GameType, Limit, TableType, SeatType); }
        }

        public override string ToString()
        {
            return UniqueName;
        }
    }
}
