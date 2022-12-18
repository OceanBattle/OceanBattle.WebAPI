using OceanBattle.DataModel;
using OceanBattle.DataModel.Game;
using OceanBattle.DataModel.Game.Abstractions;
using OceanBattle.Game.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace OceanBattle.Game.Models
{
    public class Session : IGameSession
    {
        private readonly IBattlefieldFactory _battleFieldFactory;
        private readonly IGameInterface _gameInterface;

        private Subject<IGameSession> _completed = new();
        public IObservable<IGameSession> Completed => _completed.AsObservable();

        public Level Level { get; private set; }

        public bool IsActive =>
            Oponent != null &&
            Battlefields.All(b => b!.IsReady && b!.Ships.Any(ship => !ship.IsDestroyed));

        public int BattlefieldSize { get; private set; }
        public IBattlefield?[] Battlefields { get; private set; }

        public User Creator { get; private set; }
        public User? Oponent { get; private set; }

        public Session(
            User creator, 
            Level level,
            IBattlefieldFactory battlefieldFactory,
            IGameInterface gameInterface)
        {
            Level = level;
            Creator = creator;
            BattlefieldSize = level.BattlefieldSize;
            _battleFieldFactory = battlefieldFactory;
            _gameInterface = gameInterface;

            Battlefields = new IBattlefield?[2] 
            { 
                CreateBattlefield(creator, BattlefieldSize), 
                null 
            };
        }

        public void AddOponent(User oponent)
        {
            if (IsActive)
                return;

            Oponent = oponent;
            Battlefields[1] = CreateBattlefield(oponent, BattlefieldSize);

            _gameInterface.DeploymentStarted(this);
        }

        public IBattlefield? GetBattlefield(User player)
            => GetBattlefield(player.Id);

        public IBattlefield? GetBattlefield(string playerId)
            => Battlefields.First(b => 
            b is not null && 
            b.Owner is not null && 
            b.Owner.Id == playerId);

        public IBattlefield? GetOponentBattlefield(User player)
            => GetOponentBattlefield(player.Id);

        public IBattlefield? GetOponentBattlefield(string playerId) 
            => Battlefields.First(b => 
            b is not null && 
            b.Owner is not null && 
            b.Owner.Id != playerId); 

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
            _gameInterface.GotHit(this, battlefield.Owner!, coordinates);
        }

        private void OnBattlefieldDestroyed(IBattlefield battlefield)
        {
            _completed.OnNext(this);
            _completed.OnCompleted();

            _gameInterface.GameEnded(this);
        }

        #endregion
    }
}
