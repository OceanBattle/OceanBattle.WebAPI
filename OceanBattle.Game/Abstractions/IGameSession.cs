using OceanBattle.DataModel;
using OceanBattle.DataModel.Game.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace OceanBattle.Game.Abstractions
{
    public interface IGameSession
    {
        IObservable<IGameSession> Completed { get; }
        bool IsActive { get; }
        User Creator { get; }
        User? Oponent { get; }
        IBattlefield?[] Battlefields { get; }
        void AddOponent(User oponent);    
    }
}
