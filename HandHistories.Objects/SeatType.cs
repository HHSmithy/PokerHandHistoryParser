using System;
using System.Linq;
using System.Runtime.Serialization;

namespace HandHistories.CommonObjects
{
    public enum SeatTypes : byte
    {
        Unknown = 0,
        All = 1,
        SixMax = 2,
        FullRing = 3,
        HeadsUp = 4
    }

    [Serializable]
    [DataContract]
    public class SeatType
    {
        [DataMember]
        private int _maxPlayers;

        private SeatType()
        {
            
        }

        private SeatType(int maxPlayers)
        {            
            if (maxPlayers > 10) maxPlayers = 10;
            if (maxPlayers < 2 && maxPlayers != -1) maxPlayers = 2;

            _maxPlayers = maxPlayers;

            switch (maxPlayers)
            {
                case -1:
                    _seatType = "All";
                    break;                    
                case 2:
                    _seatType = "HU";
                    break;
                case 6:
                    _seatType = "6 Max";
                    break;
                case 9:
                case 10:
                    _seatType = "Full Ring";
                    break;
                default:
                    _seatType = maxPlayers + " Handed";
                    break;
            }
        }

        [DataMember]
        private string _seatType;
        

        public string Name
        {
            get { return _seatType; }
            set { _seatType = value; }
        }

        public int MaxPlayers
        {
            get { return _maxPlayers; }
            set { _maxPlayers = value; }
        }

        public SeatTypes SeatTypeEnum
        {
            get
            {
                switch (MaxPlayers)
                {
                    case -1:
                        return SeatTypes.Unknown;
                    case 2:
                        return SeatTypes.HeadsUp;
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                        return SeatTypes.SixMax;
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        return SeatTypes.FullRing;
                    default:                                               
                        throw new Exception("Unhandled Max player => Enum conversation. Max number of players is: " + MaxPlayers);
                }
            }
        }

        public override string ToString()
        {
            return _seatType;
        }

        public static SeatType AllSeatType()
        {
            return new SeatType(-1);
        }

        public static SeatType FromMaxPlayers(int maxPlayers)
        {
            return new SeatType(maxPlayers);
        }

        public static SeatType FromSeatTypes(string seatTypes)
        {
            return FromSeatTypes((SeatTypes)Enum.Parse(typeof (SeatTypes), seatTypes));
        }

        public static SeatType FromSeatTypes(SeatTypes seatTypes)
        {
            switch (seatTypes)
            {
                case SeatTypes.All:
                    return new SeatType(-1);
                case SeatTypes.SixMax:
                    return new SeatType(6);
                case SeatTypes.FullRing:
                    return new SeatType(10);
                case SeatTypes.HeadsUp:
                    return new SeatType(2);
                default:
                    throw new ArgumentException("FromSeatTypes: Unknown seat type enum " + seatTypes);
            }
        }

        public static SeatType FromTableScanPlayerColumn(string playerColumn, string tableName)
        {
            int numSeats = -1;

            if (!string.IsNullOrEmpty(playerColumn))
            {                
                // Table scan outputs player columns as players/max for FTP and Party
                // but only numplayers for stars
                if (playerColumn.Split('/').Count() == 2)
                {
                    numSeats = Int32.Parse(playerColumn.Split('/')[1]);                    
                }
                else // Handle for stars
                {
                    if (tableName.Contains("6 max"))
                    {
                        numSeats = 6;
                    }
                    else if (tableName.Contains("1-on-1"))
                    {
                        numSeats = 2;
                    }
                    else
                    {
                        numSeats = 10;
                    }
                }
            }

            return new SeatType(numSeats);
        }

        public override bool Equals(object obj)
        {
            return obj.ToString().Equals(ToString());
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static string GetSeatTypeName(string seat)
        {
            switch (seat.ToLower())
            {
                case "6 max":
                case "six max":
                case "sixmax":
                case "6max":
                    return "6 Max";
                case "hu":
                case "heads up":
                case "1-on-1":
                    return "HU";
                case "fr":
                case "full ring":
                case "ring":
                case "full":
                    return "Full Ring";
                default:
                    return "Unknown";
            }            
        }
    }
}
