using System;
using System.Linq;
using System.Runtime.Serialization;

namespace HandHistories.Objects.GameDescription
{
    [Serializable]
    [DataContract]
    public struct SeatType
    {
        enum SeatTypeEnum : byte
        {
            Unknown = 0,
            All = 1,
            HeadsUp = 2,
            _3Handed = 3,
            _4Max = 4,
            _5Handed = 5,
            _6Max = 6, 
            _7Handed = 7, 
            _8Handed = 8,
            _FullRing_9Handed = 9,
            _FullRing_10Handed = 10,
        }

        [DataMember]
        private SeatTypeEnum seatType;

        private SeatType(int maxPlayers, bool realTypes=false)
        {            
            switch (maxPlayers)
            {
                case -1:
                case 0:
                    seatType = SeatTypeEnum.All;
                    break;   
                case 2:
                    seatType = SeatTypeEnum.HeadsUp;
                    break;
                case 3:
                    seatType = realTypes ? SeatTypeEnum._6Max : SeatTypeEnum._3Handed;
                    break;
                case 4:
                    seatType = realTypes ? SeatTypeEnum._6Max : SeatTypeEnum._4Max;
                    break;
                case 5:
                    seatType = realTypes ? SeatTypeEnum._6Max : SeatTypeEnum._5Handed;
                    break;
                case 6:
                    seatType = SeatTypeEnum._6Max;
                    break;
                case 7:
                    seatType = realTypes ? SeatTypeEnum._FullRing_9Handed : SeatTypeEnum._7Handed;
                    break;
                case 8:
                    seatType = realTypes ? SeatTypeEnum._FullRing_9Handed : SeatTypeEnum._8Handed;
                    break;
                case 9:
                    seatType = SeatTypeEnum._FullRing_9Handed;
                    break;
                case 10:
                    seatType = SeatTypeEnum._FullRing_10Handed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("MaxPlayer, Must be between 0 and 10");
            }
        }

        public bool isEmpty
        {
            get
            {
                return seatType == SeatTypeEnum.Unknown; 
            }
        }

        public int MaxPlayers
        {
            get 
            {
                if (seatType == SeatTypeEnum.All)
                {
                    throw new ArgumentException("MaxPlayers All does not represent a SeatType");
                }
                return (int)seatType; 
            }
        }

        public override string ToString()
        {
            switch (seatType)
            {
                case SeatTypeEnum.Unknown:
                    return "Unknown";
                case SeatTypeEnum.All:
                    return "All";
                case SeatTypeEnum.HeadsUp:
                    return "HeadsUp";
                case SeatTypeEnum._3Handed:
                    return "3 Handed";
                case SeatTypeEnum._4Max:
                    return "4 Max";
                case SeatTypeEnum._5Handed:
                    return "5 Handed";
                case SeatTypeEnum._6Max:
                    return "6 Max";
                case SeatTypeEnum._7Handed:
                    return "7 Handed";
                case SeatTypeEnum._8Handed:
                    return "8 Handed";
                case SeatTypeEnum._FullRing_9Handed:
                    return "Full Ring (9 Handed)";
                case SeatTypeEnum._FullRing_10Handed:
                    return "Full Ring (10 Handed)";
                default:
                    throw new ArgumentOutOfRangeException("seatType");
            }
        }

        public static SeatType AllSeatType()
        {
            return new SeatType(0);
        }

        public static SeatType FromMaxPlayers(int maxPlayers, bool realTypes=false)
        {
            return new SeatType(maxPlayers, realTypes);
        }
     
        public static SeatType Parse(string seatType)
        {
            switch (seatType.ToLower())
            {
                case "hu":
                case "heads up":
                case "headsup":
                case "1-on-1":
                case "2":
                case "2 handed":
                    return SeatType.FromMaxPlayers(2);
                case "3":
                case "3 handed":
                case "3 max":
                case "3max":
                case "3-max":
                    return SeatType.FromMaxPlayers(3);
                case "4 max":
                case "four max":
                case "fourmax":
                case "4max":
                case "4":
                    return SeatType.FromMaxPlayers(4);
                case "5":
                case "5 handed":
                case "5 max":
                case "5max":
                case "5-max":
                    return SeatType.FromMaxPlayers(5);
                case "6 max":
                case "six max":
                case "sixmax":
                case "6max":
                case "6":
                case "6 handed":
                case "3to6":
                    return SeatType.FromMaxPlayers(6);
                case "7":
                case "7 handed":
                case "7 max":
                case "7max":
                case "7-max":
                    return SeatType.FromMaxPlayers(7);
                case "8":
                case "8 handed":
                case "8 max":
                case "8max":
                case "8-max":
                    return SeatType.FromMaxPlayers(8);
                case "full ring (9 handed)":
                case "9":
                case "9 handed":
                    return SeatType.FromMaxPlayers(9);
                case "fr":
                case "full ring":
                case "ring":
                case "full":
                case "full ring (10 handed)":
                case "10":
                case "10 handed":
                    return SeatType.FromMaxPlayers(10);                
                default:
                    return SeatType.AllSeatType();
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
            return seatType.GetHashCode();
        }
    }
}
