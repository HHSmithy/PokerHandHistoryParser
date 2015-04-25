using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HandHistories.Objects.GameDescription
{
    [DataContract]
    public class SitAndGoTriple
    {
        [DataMember]
        public GameType GameType { get; private set; }
        [DataMember]
        public Buyin Buyin { get; private set; }
        [DataMember]
        public TableType TableType { get; private set; }

        public SitAndGoTriple(Buyin buyin, TableType tableType, GameType gameType)
        {
            Buyin = buyin;
            TableType = tableType;
            GameType = gameType;
        }

        public static SitAndGoTriple Parse(string sitAndGoTriple)
        {
            string buyinString = sitAndGoTriple.Split('_')[0];
            string tableTypeString = sitAndGoTriple.Split('_')[1];
            string gameTypeString = sitAndGoTriple.Split('_')[2];

            GameType gameType = (GameType) Enum.Parse(typeof (GameType), gameTypeString,true);
            
            TableType tableType = TableType.Parse(tableTypeString);
            Buyin buyin = Buyin.ParseDbSafeString(buyinString);

            return new SitAndGoTriple(buyin, tableType, gameType);
        }

        public override string ToString()
        {
            return Buyin.ToDbSafeString() + "_" + TableType.ToString() + "_" + GameType;
        }

        public string ToDisplayName()
        {
            return Buyin.ToString(CultureInfo.CurrentCulture,false,"+") + " " + GameTypeUtils.GetShortName(GameType) + " [" + TableType.ToString().Replace("-",", ") + "] ";
        }

        public string ToBuyinFormatString(bool includeAdditionalInfos = true)
        {
            var output = String.Format("{0}{1} ({0}{2}+{0}{3})", Buyin.GetCurrencySymbol(),
                                       (Buyin.PrizePoolValue + Buyin.Rake), Buyin.PrizePoolValue, Buyin.Rake);

            if (Buyin.KnockoutValue > 0.0m)
                output = String.Format("{0}{1} ({0}{2}+{0}{3}+{0}{4})", Buyin.GetCurrencySymbol(),
                                       (Buyin.PrizePoolValue + Buyin.KnockoutValue + Buyin.Rake), Buyin.PrizePoolValue, Buyin.KnockoutValue, Buyin.Rake);


            if (includeAdditionalInfos)
                output = String.Format("{0} {1} [{2}] ", output, GameType.ToString(),
                                       TableType.ToString());

            return output;
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
