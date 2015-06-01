using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace HandHistories.Objects.GameDescription
{
    [Serializable]
    [DataContract]
    public class Buyin
    {
        private Buyin()
        {

        }

        private Buyin(decimal prizePool, decimal rake, Currency currency, bool isKnockout, decimal knockoutValue)
        {
            Currency = currency;
            PrizePoolValue = prizePool;
            Rake = rake;
            IsKnockout = isKnockout;
            KnockoutValue = knockoutValue;
        }

        public static Buyin FromBuyinRake(decimal prizePoolValue,
                                          decimal rake,
                                          Currency currency,
                                          bool isKnockout = false,
                                          decimal knockoutValue = 0)
        {
            return new Buyin(prizePoolValue, rake, currency, isKnockout, knockoutValue);
        }

        [DataMember]
        public Currency Currency { get; set; }

        [DataMember]
        public decimal PrizePoolValue { get; private set; }

        [DataMember]
        public decimal Rake { get; private set; }

        [DataMember]
        public bool IsKnockout { get; set; }

        [DataMember]
        public decimal KnockoutValue { get; set; }

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
                case Currency.All:
                    return @"";
                default:
                    throw new Exception("Unrecognized currency " + Currency);
            }
        }

        // TODO: adjust these values
        public LimitGrouping GetLimitGrouping()
        {
            var combinedValue = TotalBuyin();

            if (combinedValue < 5m)
            {
                return LimitGrouping.Micro;
            }
            if (combinedValue < 20m)
            {
                return LimitGrouping.Small;
            }
            if (combinedValue < 100m)
            {
                return LimitGrouping.Mid;
            }
            
            return LimitGrouping.High;
        }

        public string ToDbSafeString(bool skipCurrency = false)
        {
            // result should be like B100c-100c-10c ( knockout version )
            //                       B100c-10c ( regular version )

            if(PrizePoolValue == 0 && Rake == 0)
            {
                return "Any";
            }

            string buyin;
            
            if(IsKnockout)
            {
                buyin = string.Format("B{0}c-{1}c-{2}c", (int)(PrizePoolValue * 100), (int)(KnockoutValue * 100), (int)(Rake * 100));
            }
            else
            {
                buyin = string.Format("B{0}c-{1}c", (int)(PrizePoolValue * 100), (int)(Rake * 100));
            }

            if (skipCurrency == false)
            {
                buyin = buyin + "-" + Currency;
            }

            return buyin;
        }

        public static Buyin ParseDbSafeString(string buyinString)
        {
            if (buyinString == "Any")
            {
                return AllBuyin();
            }

            if (buyinString[0] == 'B' || buyinString[0] == 'B') buyinString = buyinString.Substring(1);
            string[] split = buyinString.Replace("c", "").Split('-');
            
            decimal prizePoolValue = Int32.Parse(split[0]) / 100.0m;
            decimal rake = 0m;
            decimal knockoutValue = 0m;
            string currencyString = "All";

            // Format: PrizePool-Knockout-Rake-Currency
            if(split.Length == 4)
            {
                knockoutValue = Int32.Parse(split[1]) / 100.0m;
                rake = Int32.Parse(split[2]) / 100.0m;
                currencyString = split[3];
            }

            // Format: PrizePool-Knockout-Rake OR PrizePool-Rake-Currency
            else if(split.Length == 3)
            {
                int test;
                if(Int32.TryParse(split[2], out test))
                {
                    rake = test / 100.0m;
                    knockoutValue = Int32.Parse(split[1]) / 100.0m;
                }
                else
                {
                    currencyString = split[2];
                    rake = Int32.Parse(split[1]) / 100.0m;
                }
            }

            // Format: PrizePool-Rake
            else if(split.Length == 2)
            {
                rake = Int32.Parse(split[1]) / 100.0m;
            }

            var currency = (Currency)Enum.Parse(typeof(Currency), currencyString, true);

            return FromBuyinRake(prizePoolValue, rake, currency, knockoutValue != 0, knockoutValue);
        }

        public static Buyin AllBuyin()
        {
            return FromBuyinRake(0, 0, Currency.All);
        }

        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture);
        }

        public decimal TotalBuyin()
        {
            return PrizePoolValue + KnockoutValue + Rake;
        }

        public string ToString(IFormatProvider format, bool ignoreCurrencyStrings = false, string seperatorCharacter = "-")
        {
            string currencySymbol = (ignoreCurrencyStrings ? string.Empty : GetCurrencySymbol());

            return GetBuyinString(currencySymbol, seperatorCharacter, format);
        }

        private string GetBuyinString(string currencySymbol, string seperatorCharacter, IFormatProvider format)
        {
            string prizePoolString = (PrizePoolValue != Math.Round(PrizePoolValue)) ? PrizePoolValue.ToString("N2", format) : PrizePoolValue.ToString("N0", format);
            string rakeString = (Rake != Math.Round(Rake)) ? Rake.ToString("N2", format) : Rake.ToString("N0", format);

            if (IsKnockout)
            {
                string knockoutString = (KnockoutValue != Math.Round(KnockoutValue)) ? KnockoutValue.ToString("N2", format) : KnockoutValue.ToString("N0", format);

                return string.Format("{0}{1}{4}{0}{3}{4}{0}{2}", currencySymbol, prizePoolString, rakeString, knockoutString, seperatorCharacter);
            }

            return string.Format("{0}{1}{3}{0}{2}", currencySymbol, prizePoolString, rakeString, seperatorCharacter);
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
