using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Objects.GameDescription
{
    partial struct GameType
    {
        public static GameType Unknown
        {
            get
            {
                return new GameType();
            }
        }

        public static GameType FixedLimitHoldem
        {
            get
            {
                return new GameType(GameLimitEnum.FixedLimit, GameEnum.Holdem);
            }
        }

        public static GameType NoLimitHoldem
        {
            get
            {
                return new GameType(GameLimitEnum.NoLimit, GameEnum.Holdem);
            }
        }

        public static GameType PotLimitHoldem
        {
            get
            {
                return new GameType(GameLimitEnum.PotLimit, GameEnum.Holdem);
            }
        }

        public static GameType FixedLimitOmaha
        {
            get
            {
                return new GameType(GameLimitEnum.FixedLimit, GameEnum.Omaha);
            }
        }

        public static GameType NoLimitOmaha
        {
            get
            {
                return new GameType(GameLimitEnum.NoLimit, GameEnum.Omaha);
            }
        }

        public static GameType PotLimitOmaha
        {
            get
            {
                return new GameType(GameLimitEnum.PotLimit, GameEnum.Omaha);
            }
        }

        public static GameType FiveCardFixedLimitOmaha
        {
            get
            {
                return new GameType(GameLimitEnum.FixedLimit, GameEnum.FiveCardOmaha);
            }
        }

        public static GameType FiveCardNoLimitOmaha
        {
            get
            {
                return new GameType(GameLimitEnum.NoLimit, GameEnum.FiveCardOmaha);
            }
        }

        public static GameType FiveCardPotLimitOmaha
        {
            get
            {
                return new GameType(GameLimitEnum.PotLimit, GameEnum.FiveCardOmaha);
            }
        }

        public static GameType FixedLimitOmahaHiLo
        {
            get
            {
                return new GameType(GameLimitEnum.FixedLimit, GameEnum.OmahaHiLo);
            }
        }

        public static GameType NoLimitOmahaHiLo
        {
            get
            {
                return new GameType(GameLimitEnum.NoLimit, GameEnum.OmahaHiLo);
            }
        }

        public static GameType PotLimitOmahaHiLo
        {
            get
            {
                return new GameType(GameLimitEnum.PotLimit, GameEnum.OmahaHiLo);
            }
        }

        public static GameType FiveCardFixedLimitOmahaHiLo
        {
            get
            {
                return new GameType(GameLimitEnum.FixedLimit, GameEnum.FiveCardOmahaHiLo);
            }
        }

        public static GameType FiveCardNoLimitOmahaHiLo
        {
            get
            {
                return new GameType(GameLimitEnum.NoLimit, GameEnum.FiveCardOmahaHiLo);
            }
        }

        public static GameType FiveCardPotLimitOmahaHiLo
        {
            get
            {
                return new GameType(GameLimitEnum.PotLimit, GameEnum.FiveCardOmahaHiLo);
            }
        }
    }
}
