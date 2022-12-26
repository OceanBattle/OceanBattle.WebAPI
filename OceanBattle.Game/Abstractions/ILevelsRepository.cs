using OceanBattle.DataModel.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanBattle.Game.Abstractions
{
    public interface ILevelsRepository
    {
        IEnumerable<Level> GetLevels(); 
    }
}
