using OceanBattle.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanBattle.Game.Abstractions
{
    public interface ISessionFactory
    {
        ISession Create(User creator, int battleFieldSize);
    }
}
