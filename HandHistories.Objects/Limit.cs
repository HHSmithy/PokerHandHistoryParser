using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace HandHistories.CommonObjects
{
    public enum LimitEnum : byte
    {
        Limit_1c_2c = 0,
        Limit_2c_4c = 3,
        Limit_2c_5c = 6,        
        Limit_4c_8c = 7,
        Limit_5c_10c = 9,
        Limit_6c_12c = 10,
        Limit_8c_16c = 11,
        Limit_10c_20c = 12,
        Limit_12c_25c = 14,
        Limit_10c_25c = 15,        
        Limit_15c_30c = 18,
        Limit_20c_40c = 21,
        Limit_25c_50c = 24,
        Limit_40c_80c = 27,
        Limit_50c_100c = 30,
        Limit_75c_150c = 33,
        Limit_100c_200c = 36,
        Limit_100c_300c = 39,
        Limit_125c_250c = 40,
        Limit_150c_300c = 42,
        Limit_200c_400c = 45,
        Limit_200c_500c = 48,
        Limit_300c_500c = 51,
        Limit_250c_500c = 54,
        Limit_300c_600c = 57,
        Limit_400c_800c = 60,
        Limit_500c_1000c = 63,
        Limit_750c_1500c = 66,
        Limit_800c_1600c = 69,
        Limit_1000c_2000c = 72,
        Limit_1250c_2500c = 73,
        Limit_1500c_3000c = 75,
        Limit_2000c_4000c = 78,
        Limit_2500c_5000c = 81,
        Limit_3000c_6000c = 84,
        Limit_4000c_8000c = 87,
        Limit_5000c_10000c = 90,
        Limit_7500c_15000c = 93,
        Limit_10000c_20000c = 96,
        Limit_15000c_30000c = 99,
        Limit_20000c_40000c = 102,
        Limit_25000c_50000c = 105,
        Limit_30000c_60000c = 108,
        Limit_50000c_100000c = 111,
        Limit_100000c_200000c = 114,
        Limit_150000c_300000c = 117,
        Limit_200000c_400000c = 120,        
        All = 255
    }

    public enum LimitClassEnum
    {
        Micro,
        Small,
        Mid,
        High,
        All
    }

    [Serializable]
    [DataContract]
    public class Limit
    {
        private Limit()
        {
            
        }

        public static bool TryParseLimitClass(string limitString, out LimitClassEnum limitClassEnum)
        {
            if (Enum.TryParse(limitString, out limitClassEnum))
            {
                return true;
            }

            LimitEnum limitValue;
            //If we aren't passed a valid LimitClass (Micro, Small, Mid, High), try parsing as a limit enum and converting
            if (TryParse(limitString, out limitValue))
            {
                //If we are below 50c_100c
                if ((byte)limitValue < 30)
                {
                    limitClassEnum = LimitClassEnum.Micro;
                }
                //If we are below 200c_400c
                else if ((byte)limitValue < 45)
                {
                    limitClassEnum = LimitClassEnum.Small;
                }
                //If we are below 500c_1000c
                else if ((byte)limitValue < 63)
                {
                    limitClassEnum = LimitClassEnum.Mid;
                }
                else
                {
                    limitClassEnum = LimitClassEnum.High;
                }
                return true;
            }
            return false;
        }

        public static bool TryParse(string limitString, out LimitEnum limitEnum)
        {
            if (limitString.ToLower().Equals("all"))
            {
                limitEnum = LimitEnum.All;
                return true;
            }
            string fullLimitString;


            int slashIndex = limitString.IndexOf("/");
            if (slashIndex != -1)
            {
                string[] split = limitString.Split('/');

                double bigBlind = Double.Parse(split[1]);
                double smallBlind = Double.Parse(split[0]);

                fullLimitString = string.Format("Limit_{0}c_{1}c", smallBlind * 100, bigBlind * 100);
            }
            else
            {
                var limitMatch = Regex.Match(limitString, @"[0-9]+c_[0-9]+c");

                if (limitMatch.Success == false)
                {
                    limitMatch = Regex.Match(limitString, @"[0-9]+c");

                    if (limitMatch.Success == false)
                    {
                        limitEnum = LimitEnum.All;
                        return false;
                    }

                    int bigBlind = Int32.Parse(Regex.Match(limitMatch.Value, @"[0-9]+").Value);
                    int smallBlind = bigBlind / 2;

                    fullLimitString = string.Format("Limit_{0}c_{1}c", smallBlind, bigBlind);
                }
                else
                {
                    fullLimitString = "Limit_" + limitMatch.Value;
                }
    
            }
                        
            return Enum.TryParse(fullLimitString, out limitEnum);
        }

        [DataMember]
        private double _smallBlind;

        [DataMember]
        private double _bigBlind;

        [DataMember] 
        private bool _isAnteTable;

        [DataMember]
        private Currency _currency;

        public Currency Currency { get { return _currency; } set { _currency = value; } } 

        private Limit(double bigBlind, Currency currency) : this (GetStandardSmallBlind(bigBlind), bigBlind, currency)
        {
                            
        }

        private Limit(double smallBlind, double bigBlind, Currency currency)
        {
            _currency = currency;
            _bigBlind = bigBlind;
            _smallBlind = smallBlind;
        }

        public static Limit GetLimitFromEnum(LimitEnum limitEnum, Currency currency)
        {
            if (limitEnum == LimitEnum.All)
                return null;

            double bigBlind = GetBigBlindFromLimitEnum(limitEnum);
            double smallBlind = GetSmallBlindFromLimitEnum(limitEnum);

            return Limit.FromSmallBlindBigBlind(smallBlind, bigBlind, currency);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smallBlind">The small blind </param>
        /// <param name="bigBlind"></param>
        /// <returns></returns>
        public static LimitEnum GetLimitEnumFromSmallBlindBigBlind(double smallBlind, double bigBlind)
        {                       
            int smallBlindTarget = (int)(smallBlind*100);
            int bigBlindTarget = (int)(bigBlind*100);

            foreach (string limitEnumString in Enum.GetNames(typeof(LimitEnum)))
            {
                if (limitEnumString == "All")
                {
                    continue;
                }
                
                int smallBlindIterate = Int32.Parse(limitEnumString.Split('_')[1].Replace("c", ""));
                int bigBlindIterate = Int32.Parse(limitEnumString.Split('_')[2].Replace("c", ""));

                if (smallBlindIterate == smallBlindTarget && bigBlindTarget == bigBlindIterate)
                {
                    return (LimitEnum) Enum.Parse(typeof (LimitEnum), limitEnumString);
                }
            }

            throw new Exception("Unrecognized limit " + smallBlind + " / " + bigBlind);
        }

        public static double GetSmallBlindFromLimitEnum(LimitEnum limitEnum)
        {
            string bigBlindString = limitEnum.ToString().Split('_')[1].Replace("c", "");
            return Double.Parse(bigBlindString) / 100;
        }

        public static double GetBigBlindFromLimitEnum(LimitEnum limitEnum)
        {            
            string bigBlindString = limitEnum.ToString().Split('_')[2].Replace("c", "");
            return Double.Parse(bigBlindString)/100;
        }

        public static double GetStandardSmallBlind(double bigBlind)
        {
            if (bigBlind == .25) // special case for 25NL which is 0.10/0.25
                return .10;
            else
                return bigBlind / 2.0;         
        }

        public static Limit FromSmallBlindBigBlind(double smallBlind, double bigBlind, Currency currency, bool isAnteTable = false, decimal anteAmount = 0)
        {
            return new Limit(smallBlind, bigBlind, currency)
                       {
                           Ante = anteAmount,
                           IsAnteTable = isAnteTable
                       };
        }

        public static Limit FromBigBlind(double bigBlind, Currency currency = Currency.USD)
        {
            return new Limit(bigBlind, currency);
        }

        public static Limit FromBuyinAmount(int buyin, Currency currency)
        {           
            switch (buyin)
            {                
                default:
                    return new Limit(buyin / 100.0, currency);
            }
        }
        
        public double SmallBlind
        {
            get { return _smallBlind; }
            set { _smallBlind = value; }
        }

        public double BigBlind
        {
            get { return _bigBlind; }
            set { _bigBlind = value; }
        }

        public bool IsAnteTable
        {
            get { return _isAnteTable; }
            set { _isAnteTable = value; }
        }

        [DataMember]
        public decimal Ante { get; set; }

        public static string GetCurrencySymbol(Currency currency)
        {
            switch (currency)
            {
                case Currency.USD:
                    return @"$";
                case Currency.EURO:
                    return @"€";
                case Currency.GBP:
                    return @"£";
                case Currency.All:
                    return @"";
                default:
                    throw new Exception("Unrecognized currency " + currency);
            }
        }

        public string GetFullLimitString()
        {            
            string currencySymbol = GetCurrencySymbol(_currency);

            string smallBlindString = (SmallBlind < 1) ? SmallBlind.ToString("N2") : SmallBlind.ToString("N0");
            string bigBlindString = (BigBlind < 1) ? BigBlind.ToString("N2") : BigBlind.ToString("N0");

            return string.Format("{0}{1}-{0}{2}", currencySymbol, smallBlindString, bigBlindString);
        }

        public string ToDbSafeString()
        {
            return string.Format("L{0}c-{1}c-{2}", SmallBlind*100, BigBlind*100, _currency);
        }

        public override string ToString()
        {
            return GetFullLimitString();
        }

        /// <summary>
        /// Handles dealing with unformatted limit strings as used for local hand storage.
        /// 
        /// Returns the normal .ToString() concatenation of the blinds unless the big blind is .2 or .25 
        /// then it's a special case where we use 0.10 for the small blind string and 0.20 for the big blind
        /// (if the big blind is .2)
        /// </summary>
        public string ToLegacyLimitString()
        {   
            string smallBlindString = SmallBlind.ToString();
            string bigBlindString = BigBlind.ToString();

            if (BigBlind == .25)
            {
                smallBlindString = "0.10";
            }

            if (BigBlind == .2)
            {
                smallBlindString = "0.10";
                bigBlindString = "0.20";
            }

            return string.Format("{0}-{1}", smallBlindString, bigBlindString);
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
