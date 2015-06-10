using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandHistories.Objects.Hand
{
    public class RunItTwice
    {
        /// <summary>
        /// The second board
        /// </summary>
        public BoardCards Board;

        /// <summary>
        /// All actions that occur during the second showdown
        /// </summary>
        public List<HandAction> Actions = new List<HandAction>();
    }
}
