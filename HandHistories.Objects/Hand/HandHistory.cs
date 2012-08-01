using System;
using System.Linq;
using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;

namespace HandHistories.Objects.Hand
{
    public class HandHistory : HandHistorySummary
    {
        public HandHistory(GameDescriptor gameDescription) : base()
        {
            HandActions = new List<HandAction>();
            Players = new PlayerList();

            ComumnityCards = BoardCards.ForPreflop();
            GameDescription = gameDescription;
        }

        public HandHistory() : this(new GameDescriptor())
        {
            
        }

        public List<HandAction> HandActions { get; set; }        

        public BoardCards ComumnityCards { get; set; }

        public PlayerList Players { get; set; }

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
