using System;
using System.Linq;
using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using System.Runtime.Serialization;

namespace HandHistories.Objects.Hand
{
    public sealed class HandHistory : HandHistorySummary
    {
        public HandHistory(GameDescriptor gameDescription) : base()
        {
            HandActions = new List<HandAction>();
            Players = new PlayerList();

            CommunityCards = BoardCards.ForPreflop();
            GameDescription = gameDescription;
        }

        public HandHistory() : this(new GameDescriptor())
        {
            
        }

        public List<HandAction> HandActions { get; set; }

        public List<WinningsAction> Winners { get; set; }

        public BoardCards CommunityCards { get; set; }

        public PlayerList Players { get; set; }

        public Player Hero { get; set; }

        public RunItTwice RunItTwiceData { get; set;  }

        public int NumPlayersActive
        {
            get { return Players.Count(p => p.IsSittingOut == false); }
        }
               
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            HandHistory hand = obj as HandHistory;
            if (hand == null) return false;

            return ToString().Equals(hand.ToString());
        }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", HandId, GameDescription.ToString());
        }
    }
}
