using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HandHistories.Objects.Players
{
    [DataContract]
    public class PlayerList : IEnumerable<Player>
    {
        private List<Player> _players = new List<Player>(10); 

        public PlayerList()
        {
            
        }

        public PlayerList(IEnumerable<Player> players)
        {
            AddRange(players);
        }

        public void Add(Player player)
        {
            _players.Add(player);

            SortList();
        }

        public void AddRange(IEnumerable<Player> players)
        {
            _players.AddRange(players);

            SortList();
        }

        public void SortList()
        {
            _players = _players.OrderBy(p => p.SeatNumber).ToList();
        }

        public void Remove(int index)
        {
            _players.RemoveAt(index);
        }

        public Player this[int index]
        {
            get { return _players[index]; }
        }        

        public Player this[string name]
        {
            get { return _players.FirstOrDefault(p => p.PlayerName.Equals(name)); }
        }
       
        public int Count
        {
            get { return _players.Count; }
        }

        public IEnumerator<Player> GetEnumerator()
        {
            return _players.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}