using System;
using System.Linq;

namespace HandHistories.Objects.GameDescription
{   
    public class GameTypeUtils
    {                
        public static GameType ParseGameString(string gameString)
        {          
            switch (gameString.ToLower())
            {
                case "nl":
                case "nlh":
                case "nl holdem":
                case "nl hold'em":
                case "no limit hold'em":
                case "no limit holdem":
                case "no limit":
                case "holdem no limit":
                    return GameType.NoLimitHoldem;
                case "pl":
                case "pot limit":
                case "pl holdem":
                case "pot limit holdem":
                case "pot-limit holdem":
                    return GameType.PotLimitHoldem;                    
                case "fl":
                case "fixed limit":
                case "fl holdem":
                case "fl hold'em":
                case "fixed holdem":
                case "fixed hold'em":
                case "limit hold'em":
                    return GameType.FixedLimitHoldem;
                case "pl omaha":
                case "plo":
                case "omaha":
                case "pot limit omaha":
                case "pot-limit omaha":
                case "omaha pot limit":
                    return GameType.PotLimitOmaha;
                case "pl omaha h/l":
                case "plo hi-lo":
                    return GameType.PotLimitOmahaHiLo;
                case "omaha hi-lo no limit":
                case "no limit omaha hi-lo":
                    return GameType.NoLimitOmahaHiLo;     
                case "fl omaha hi-lo":
                    return GameType.FixedLimitOmahaHiLo;
                case "fixed limit omaha":
                    return GameType.FixedLimitOmaha;
                case "no limit omaha":
                    return GameType.NoLimitOmaha;
                case "pot limit five card omaha hi-lo":
                case "5 card omaha hi/lo pot limit":
                    return GameType.FiveCardPotLimitOmahaHiLo;
                case "pot limit five card omaha":
                case "5 card omaha pot limit":
                    return GameType.FiveCardPotLimitOmaha;
                default:
                    string match = Enum.GetNames(typeof(GameType)).FirstOrDefault(g => g.ToLower().Equals(gameString.ToLower()));
                    return match == null ? GameType.Unknown : (GameType)Enum.Parse(typeof(GameType), match,true);
            }
        }
       
        //public static string GetGameName(GameType gameType)
        //{
        //    switch (gameType)
        //    {
        //        case GameType.NoLimitHoldem:
        //            return "NL Holdem";
        //        case GameType.FixedLimitHoldem:
        //            return "FL Holdem";
        //        case GameType.PotLimitOmaha:
        //            return "PLO";
        //        case GameType.PotLimitOmahaHiLo:
        //            return "PLO Hi-Lo";
        //        case GameType.PotLimitHoldem:
        //            return "Pot Limit Holdem";                
        //        case GameType.Any:
        //            return "Any";
        //        case GameType.CapNoLimitHoldem:
        //            return "Cap NL Holdem";
        //        case GameType.CapPotLimitOmaha:
        //            return "Cap Pot Limit Omaha";
        //        case GameType.Unknown:
        //            return "Unknown";
        //        case GameType.FixedLimitOmahaHiLo:
        //            return "FL Omaha Hi-Lo";
        //        case GameType.NoLimitOmahaHiLo:
        //            return "No Limit Omaha Hi-Lo";      
        //        case GameType.NoLimitOmaha:
        //            return "No Limit Omaha";      
        //        case GameType.FiveCardPotLimitOmahaHiLo:
        //            return "Pot Limit Five Card Omaha Hi-Lo";
        //        case GameType.FiveCardPotLimitOmaha:
        //            return "Pot Limit Five Card Omaha";
        //        case GameType.FixedLimitOmaha:
        //            return "Fixed Limit Omaha";
        //        default:
        //            throw new NotImplementedException("GetGameName: Not implemented for " + gameType);
        //    }
        //}

        public static string GetShortName(GameType gameType)
        {
            return "";

            //switch (gameType)
            //{
            //    case GameType.NoLimitHoldem:
            //        return "NLH";
            //    case GameType.FixedLimitHoldem:
            //        return "FLH";
            //    case GameType.PotLimitOmaha:
            //        return "PLO";
            //    case GameType.PotLimitOmahaHiLo:
            //        return "PLOHiLo";
            //    case GameType.PotLimitHoldem:
            //        return "PLH";                
            //    case GameType.Any:
            //        return "Any";
            //    case GameType.CapNoLimitHoldem:
            //        return "CapNLH";
            //    case GameType.CapPotLimitOmaha:
            //        return "CapPLO";
            //    case GameType.Unknown:
            //        return "Unknown";
            //    case GameType.FixedLimitOmahaHiLo:
            //        return "FLOmahaHiLo";              
            //    case GameType.NoLimitOmahaHiLo:
            //        return "NLOmahaHiLo";
            //    case GameType.NoLimitOmaha:
            //        return "NLOmaha";
            //    case GameType.FiveCardPotLimitOmahaHiLo:
            //        return "5Card-PLOHiLo";
            //    case GameType.FiveCardPotLimitOmaha:
            //        return "5Card-PLO";
            //    case GameType.FixedLimitOmaha:
            //        return "FLOmaha";
            //    default:
            //        throw new NotImplementedException("GetGameName: Not implemented for " + gameType);
            //}
        }

        //public static string GetDisplayName(GameType gameType)
        //{
        //    switch (gameType)
        //    {
        //        case GameType.NoLimitHoldem:
        //            return "No Limit Holdem";
        //        case GameType.FixedLimitHoldem:
        //            return "Fixed Limit Holdem";
        //        case GameType.PotLimitOmaha:
        //            return "Pot Limit Omaha";
        //        case GameType.PotLimitOmahaHiLo:
        //            return "Pot Limit Omaha Hi-Lo";
        //        case GameType.FixedLimitOmahaHiLo:
        //            return "Fixed Limit Omaha Hi-Lo";
        //        case GameType.PotLimitHoldem:
        //            return "Pot Limit Holdem";
        //        case GameType.Any:
        //            return "Any";
        //        case GameType.CapNoLimitHoldem:
        //            return "Cap NL Holdem";
        //        case GameType.CapPotLimitOmaha:
        //            return "Cap Pot Limit Omaha";
        //        case GameType.Unknown:
        //            return "Unknown";
        //        default:
        //            return gameType.ToString();
        //    }
        //}
    }
}
