using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace HandHistories.Objects.GameDescription
{
    [Serializable]
    [DataContract]
    public sealed class Limit
    {
        private Limit()
        {

        }

        private Limit(decimal smallBlind, decimal bigBlind, Currency currency, bool isAnteTable, decimal ante)
        {           
            Currency = currency;
            SmallBlind = smallBlind;
            BigBlind = bigBlind;
            IsAnteTable = isAnteTable;
            Ante = ante;
        }       

        public static Limit FromSmallBlindBigBlind(decimal smallBlind,
                                                   decimal bigBlind, 
                                                   Currency currency, 
                                                   bool isAnteTable = false, 
                                                   decimal anteAmount = 0)
        {
            return new Limit(smallBlind, bigBlind, currency, isAnteTable, anteAmount);
        }

        public  static Limit FromLimitEnum(LimitEnum limitEnum,
                                           Currency currency,
                                           bool isAnteTable = false, 
                                           decimal anteAmount = 0)
        {
            if (limitEnum == LimitEnum.Any)
            {
                return new Limit(0, 0, Currency.All, isAnteTable, anteAmount);
            }

            string smallBlindString = limitEnum.ToString().Split('_')[1].Replace("c", "");
            decimal smallBlind = decimal.Parse(smallBlindString, System.Globalization.CultureInfo.InvariantCulture) / 100.0m;

            string bigBlindString = limitEnum.ToString().Split('_')[2].Replace("c", "");
            decimal bigBlind = decimal.Parse(bigBlindString, System.Globalization.CultureInfo.InvariantCulture) / 100;

            return FromSmallBlindBigBlind(smallBlind, bigBlind, currency, isAnteTable, anteAmount);
        }

        [DataMember]
        public Currency Currency { get; set; }

        [DataMember]
        public decimal SmallBlind { get; private set; }

        [DataMember]
        public decimal BigBlind { get; private set; }

        [DataMember]
        public bool IsAnteTable { get; set; }

        [DataMember]
        public decimal Ante { get; set; }

        public string GetCurrencySymbol()
        {
            switch (Currency)
            {
                case Currency.USD:
                    return @"$";
                case Currency.EURO:
                    return @"€";
                case Currency.GBP:
                    return @"£";
                case Currency.SEK:
                    return "SEK";
                case Currency.CHIPS:
                    return "";
                case Currency.CNY:
                    return @"¥";
                case Currency.All:
                    return @"";
                default:
                    throw new Exception("Unrecognized currency " + Currency);
            }
        }

        public LimitGrouping GetLimitGrouping()
        {           
            if (BigBlind < 1)
            {
                return LimitGrouping.Micro;
            }
            else if (BigBlind < 2)
            {
                return LimitGrouping.Small;
            }
            else if (BigBlind < 10)
            {
                return LimitGrouping.Mid;
            }
            else if (BigBlind <= 50)
            {
                return LimitGrouping.High;
            }
            else
            {
                return LimitGrouping.NoseBleeds;
            }
        }

        public LimitEnum GetLimitEnum()
        {
            if (SmallBlind == 0 && BigBlind == 0)
            {
                return LimitEnum.Any;
            }

            int smallBlindCents = (int)(SmallBlind * 100);
            int bigBlindCents = (int)(BigBlind * 100);

            string limit = string.Format("Limit_{0}c_{1}c", smallBlindCents, bigBlindCents);

            LimitEnum limitEnum;
            bool result = Enum.TryParse(limit, out limitEnum);

            if (result == false)
            {
                throw new Exception("Limit " + SmallBlind + " / " + BigBlind + " does not have a matching LimitEnum.");
            }

            return limitEnum;
        }        

        public string ToDbSafeString(bool skipCurrency = false)
        {
            if (GetLimitEnum() == LimitEnum.Any)
            {
                return "Any";
            }

            string anteString = (IsAnteTable)
                                    ? string.Format("-Ante{0}", (int) (Ante*100))
                                    : string.Empty;                

            string limit = string.Format("L{0}c-{1}c{2}", (int)(SmallBlind * 100), (int)(BigBlind * 100), anteString);

            if (skipCurrency == false)
            {
                limit = limit + "-" + Currency;
            }

            return limit;
        }

        public static Limit ParseDbSafeString(string limitString)
        {
            if (limitString == "Any")
            {
                return Limit.AllLimit();
            }

            if (limitString[0] == 'L' || limitString[0] == 'l') limitString = limitString.Substring(1);
            string[] split = limitString.Replace("Ante", "").Replace("ante", "").Replace("L", "").Replace("c", "").Split('-');

            decimal smallBlind = Int32.Parse(split[0])/100.0m;
            decimal bigBlind = Int32.Parse(split[1]) / 100.0m;

            decimal ante = (split.Length == 4) ? Int32.Parse(split[2])/100.0m : 0;

            string currencyString = (split.Length == 4) ? split[3] : split[2];
            Currency currency = (Currency) Enum.Parse(typeof (Currency), currencyString, true);

            return Limit.FromSmallBlindBigBlind(smallBlind, bigBlind, currency, ante != 0, ante);
        }

        public static Limit AllLimit()
        {
            return FromSmallBlindBigBlind(0, 0, Currency.All);
        }

        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture, false, false);
        }

        public string ToString(IFormatProvider format, bool ignoreCurrencyStrings = false, bool ignoreAntes = false, string seperatorCharacter = "-")
        {
            string currencySymbol = (ignoreCurrencyStrings ? string.Empty : GetCurrencySymbol());

            return GetLimitString(currencySymbol, seperatorCharacter, ignoreAntes, format);
        }        

        private string GetLimitString(string currencySymbol, string seperatorCharacter, bool ignoreAntes, IFormatProvider format)
        {
            string smallBlindString = (SmallBlind != Math.Round(SmallBlind)) ? SmallBlind.ToString("N2", format) : SmallBlind.ToString("N0", format);
            string bigBlindString = (BigBlind != Math.Round(BigBlind)) ? BigBlind.ToString("N2", format) : BigBlind.ToString("N0", format);

            string limit = string.Format("{0}{1}{3}{0}{2}", currencySymbol, smallBlindString, bigBlindString, seperatorCharacter);

            if (IsAnteTable && ignoreAntes == false)
            {
                limit = limit + "-Ante-" + currencySymbol + ((Ante < 1) ? Ante.ToString("N2", format) : Ante.ToString("N0", format));
            }

            return limit;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return obj.ToString().Equals(ToString());
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
