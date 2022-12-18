using OceanBattle.DataModel;
using OceanBattle.DataModel.Game;

namespace OceanBattle.Game.Abstractions
{
    public interface ISessionFactory
    {
        IGameSession Create(User creator, Level level);
    }
}
