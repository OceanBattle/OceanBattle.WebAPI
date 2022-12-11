using OceanBattle.DataModel;
using OceanBattle.DataModel.Game;
using OceanBattle.Game.Abstractions;
using OceanBattle.Game.Models;

namespace OceanBattle.Game.Services
{
    public class SessionsManager : ISessionsManager
    {
        private readonly ISessionFactory _sessionFactory;
        
        private List<ISession> _sessions = new List<ISession>();
        public IEnumerable<ISession> Sessions => _sessions.AsEnumerable();

        public SessionsManager(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ISession CreateSession(User creator, Level level)
        {
            ISession session = _sessionFactory.Create(creator, level.BattlefieldSize);
            _sessions.Add(session);

            return session;
        }
    }
}