using OceanBattle.DataModel.Game.Abstractions;

namespace OceanBattle.Game.Abstractions
{
    public interface IShipsRepository
    {
        IEnumerable<Ship> GetShips(); 
    }
}
