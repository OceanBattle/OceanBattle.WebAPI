using OceanBattle.DataModel;
using OceanBattle.Game.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanBattle.Game.Services
{
    public class PlayersManager : IPlayersManager
    {
        private List<User> _players = new List<User>();
        public IEnumerable<User> ActivePlayers => _players.AsEnumerable();

        public void AddAsActive(User user)
        {
            if (!_players.Contains(user))
                _players.Add(user);
        }

        public void RemoveFromActive(User user)
        {
            if (_players.Contains(user))
                _players.Remove(user);
        }
    }
}
