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

        List<HandAction> _handActions = new List<HandAction>();
        public List<HandAction> HandActions
        {
            get { return _handActions; }
            set
            {
                _handActions = value;
                CreateMetaData();
            }
        }

        #region MetaData
        public int FlopStartIndex = -1, TurnStartIndex = -1, RiverStartIndex = -1;
        private void CreateMetaData()
        {
            for (int i = 0; i < _handActions.Count; i++)
            {
                _handActions[i].ActionNumber = i;
                if (FlopStartIndex == -1 && _handActions[i].Street == Street.Flop)
                {
                    FlopStartIndex = i;
                }
                else if (TurnStartIndex == -1 && _handActions[i].Street == Street.Turn)
                {
                    TurnStartIndex = i;
                }
                else if (RiverStartIndex == -1 && _handActions[i].Street == Street.River)
                {
                    RiverStartIndex = i;
                }
            }
        }
        #endregion

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

        public List<HandAction> GetPlayerHandActions(string PlayerName)
        {
            List<HandAction> _actions = new List<HandAction>();
            foreach (var item in HandActions)
            {
                if (item.PlayerName == PlayerName && (item.IsGameAction || item.IsBlinds || item.IsShowdownAction))
                {
                    _actions.Add(item);
                }
            }
            return _actions;
        }
    }
}
