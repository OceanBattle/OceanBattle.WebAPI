using OceanBattle.DataModel;
using OceanBattle.DataModel.Game;
using OceanBattle.DataModel.Game.Abstractions;
using OceanBattle.Game.Abstractions;

namespace OceanBattle.Game.Models
{
    public class Session : ISession
    {
        private readonly IBattlefieldFactory _battleFieldFactory;

        public bool IsActive => 
            Oponent != null &&
            Battlefields[0]!.Ships.Any(ship => !ship.IsDestroyed) && 
            Battlefields[1]!.Ships.Any(ship => !ship.IsDestroyed);

        public int BattlefieldSize { get; private set; }
        public IBattlefield?[] Battlefields { get; private set; }

        public User Creator { get; private set; }
        public User? Oponent { get; private set; }

        public Session(
            User creator, 
            int battlefieldSize,
            IBattlefieldFactory battlefieldFactory)
        {
            Creator = creator;
            BattlefieldSize = battlefieldSize;
            _battleFieldFactory = battlefieldFactory;

            IBattlefield battlefield = _battleFieldFactory.Create(battlefieldSize, battlefieldSize);
            battlefield.Owner = Creator;

            Battlefields = new IBattlefield?[2] { battlefield, null };
        }

        public void AddOponent(User oponent)
        {
            Oponent = oponent;

            IBattlefield battlefield = _battleFieldFactory.Create(BattlefieldSize, BattlefieldSize);
            battlefield.Owner = oponent;

            Battlefields[1] = battlefield;
        } 
    }
}
