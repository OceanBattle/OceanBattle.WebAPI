using OceanBattle.DataModel.Game.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanBattle.Game.Abstractions
{
    public interface IBattlefieldFactory
    {
        IBattlefield Create(int m, int n);
    }
}
