using OceanBattle.DataModel;
using OceanBattle.DataModel.Game;
using OceanBattle.Game.Abstractions;
using System.Reactive.Linq;

namespace OceanBattle.Game.Services
{
    public class SessionsManager : ISessionsManager
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly IPlayersManager _playersManager;

        private List<IGameSession> _sessions = new List<IGameSession>();
        public IEnumerable<IGameSession> Sessions => _sessions.AsEnumerable();

        public SessionsManager(
            ISessionFactory sessionFactory,
            IPlayersManager playersManager)
        {
            _sessionFactory = sessionFactory;
            _playersManager = playersManager;
        }

        public void EndSessions(User creator)
            => EndSessions(creator.Id);

        public void EndSessions(string creatorId)
        {
            _sessions.RemoveAll(s => s.Creator.Id == creatorId);
        }

        public IGameSession? CreateSession(string creatorId, Level level)
        {
            if (_sessions.Any(s => s.Creator.Id == creatorId || 
                (s.Oponent is not null && s.Oponent.Id == creatorId)))
                return null;

            User? creator = _playersManager.GetPlayer(creatorId);

            if (creator is null ||
                !_playersManager.ActivePlayers.Contains(creator))
                return null;

            IGameSession session = _sessionFactory.Create(creator, level);

            session.Completed
                .Take(1)
                .Subscribe(OnSessionCompleted);

            _sessions.Add(session);

            return session;
        }

        public IGameSession? CreateSession(User creator, Level level)
            => CreateSession(creator.Id, level);

        public IGameSession? FindSession(User participant)
            => FindSession(participant.Id);

        public IGameSession? FindSession(string participantId) 
            => _sessions.FirstOrDefault(session 
                => session.Battlefields.Any(battlefield 
                    => battlefield is not null && 
                       battlefield.Owner is not null && 
                       battlefield.Owner.Id == participantId));
        

        #region private helpers

        private void OnSessionCompleted(IGameSession session)
        {
            _sessions.Remove(session);
        }

        #endregion
    }
}