using OceanBattle.DataModel;
using OceanBattle.DataModel.Game;
using OceanBattle.Game.Abstractions;
using OceanBattle.Game.Models;
using System.Reactive.Linq;

namespace OceanBattle.Game.Services
{
    public class SessionsManager : ISessionsManager
    {
        private readonly ISessionFactory _sessionFactory;
        
        private List<IGameSession> _sessions = new List<IGameSession>();
        public IEnumerable<IGameSession> Sessions => _sessions.AsEnumerable();

        public SessionsManager(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public IGameSession CreateSession(User creator, Level level)
        {
            IGameSession session = _sessionFactory.Create(creator, level.BattlefieldSize);

            session.Completed
                .Take(1)
                .Subscribe(OnSessionCompleted);

            _sessions.Add(session);

            return session;
        }

        #region private helpers

        private void OnSessionCompleted(IGameSession session)
        {
            _sessions.Remove(session);
        }

        #endregion
    }
}