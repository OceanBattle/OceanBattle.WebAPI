using OceanBattle.DataModel;
using OceanBattle.DataModel.Game;
using OceanBattle.DataModel.Game.Abstractions;
using OceanBattle.Game.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace OceanBattle.Game.Models
{
    public class Session : ISession
    {
        private readonly IBattlefieldFactory _battleFieldFactory;

        private Subject<ISession> _completed = new();
        public IObservable<ISession> Completed => _completed.AsObservable();

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

            Battlefields = new IBattlefield?[2] 
            { 
                CreateBattlefield(creator, BattlefieldSize), 
                null 
            };
        }

        public void AddOponent(User oponent)
        {
            Oponent = oponent;
            Battlefields[1] = CreateBattlefield(oponent, BattlefieldSize);
        }

        #region private helpers
       
        private IBattlefield CreateBattlefield(User player, int size)
        {
            IBattlefield battlefield = _battleFieldFactory.Create(size, size);
            battlefield.Owner = player;

            battlefield.GotHit.Subscribe(
                    (coordinates) => OnBattlefieldHit(coordinates, battlefield), 
                    () => OnBattlefieldDestroyed(battlefield));

            return battlefield;
        }

        private void OnBattlefieldHit(
            (int x, int y) coordinates, 
            IBattlefield battlefield)
        {
        }

        private void OnBattlefieldDestroyed(IBattlefield battlefield)
        {
            _completed.OnNext(this);
            _completed.OnCompleted();
        }

        #endregion
    }
}
