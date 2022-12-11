using OceanBattle.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanBattle.Game.Abstractions
{
    public interface IPlayersManager
    {
        public IEnumerable<User> ActivePlayers { get; }

        void AddAsActive(User user);
        void RemoveFromActive(User user);
    }
}
