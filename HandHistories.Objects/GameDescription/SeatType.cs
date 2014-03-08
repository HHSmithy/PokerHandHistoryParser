using System;
using System.Linq;
using System.Runtime.Serialization;

namespace HandHistories.Objects.GameDescription
{
    [Serializable]
    [DataContract]
    public class SeatType
    {
        [DataMember]
        private int _maxPlayers;

        private SeatType()
        {
            
        }

        private SeatType(int maxPlayers, bool realTypes=false)
        {            
            if (maxPlayers > 10)
            {
                throw new ArgumentException("SeatType: Max players can't be more than 10. It is " + maxPlayers);
            }
            if (maxPlayers == 1)
            {
                throw new ArgumentException("SeatType: Max players can't be 1 player. It is " + maxPlayers);
            }

            _maxPlayers = maxPlayers;

            switch (maxPlayers)
            {
                case -1:
                case 0:
                    _seatType = "All";
                    break;   
                case 2:
                    _seatType = "HeadsUp";
                    break;
                case 3:
                    _seatType = realTypes ? "6 Max" : "3 handed";
                    break;
                case 4:
                    _seatType = realTypes ? "6 Max" : "4 Max";
                    break;
                case 5:
                    _seatType = realTypes ? "6 Max" : "5 Handed";
                    break;
                case 6:
                    _seatType = "6 Max";
                    break;
                case 7:
                    _seatType = realTypes ? "Full Ring (9 Handed)" : "7 Handed";
                    break;
                case 8:
                    _seatType = realTypes ? "Full Ring (9 Handed)" : "8 Handed";
                    break;
                case 9:
                    _seatType = "Full Ring (9 Handed)";
                    break;
                case 10:
                    _seatType = "Full Ring (10 Handed)";
                    break;
                default:
                    _seatType = maxPlayers + " Handed";
                    break;
            }
        }

        [DataMember]
        private string _seatType;
       
        public int MaxPlayers
        {
            get { return _maxPlayers; }
            set { _maxPlayers = value; }
        }

        public override string ToString()
        {
            return _seatType;
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
            return ToString().GetHashCode();
        }
    }
}
