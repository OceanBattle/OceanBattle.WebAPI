using OceanBattle.DataModel;
using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game.Abstractions;
using OceanBattle.Game.Abstractions;

namespace OceanBattle.Game.Services
{
    public class PlayersManager : IPlayersManager
    {
        private readonly IGameInterface _gameInterface;
        private readonly ISessionsManager _sessionsManager;

        private List<User> _activePlayers = new List<User>();
        public IEnumerable<User> ActivePlayers => _activePlayers.AsEnumerable();

        public PlayersManager(
            IGameInterface gameInterface,
            ISessionsManager sessionsManager)
        {
            _gameInterface = gameInterface;
            _sessionsManager = sessionsManager;
        }

        public void AddAsActive(User user)
        {
            if (!_activePlayers.Any(u => u.Id == user.Id))
                _activePlayers.Add(user);
        }

        public bool IsPlayerActive(string Id)
            => _activePlayers.Any(u => u.Id == Id);

        public User? GetPlayer(string Id)
            => _activePlayers.FirstOrDefault(u => u.Id == Id);

        public void RemoveFromActive(User player)
            => RemoveFromActive(player.Id);

        public void RemoveFromActive(string playerId)
        {
            _sessionsManager.EndSessions(playerId);
            _activePlayers.RemoveAll(u => u.Id == playerId);
        }           

        public void InvitePlayer(User player, User sender)
            => InvitePlayer(player.Id, sender.Id);

        public void InvitePlayer(UserDto player, string senderId)
        {
            User? activePlayer = _activePlayers.FirstOrDefault(u => u.UserName == player.UserName);

            if (activePlayer is null)
                return;

            InvitePlayer(activePlayer.Id, senderId); 
        }

        public void InvitePlayer(string playerId, string senderId)
        {
            if (!_activePlayers.Any(u => u.Id == playerId))
                return;

            User? sender = GetPlayer(senderId);

            if (sender is null)
                return;

            IGameSession? session = _sessionsManager.FindSession(senderId);

            if (session is null)
                return;

            if (!session.InvitedPlayersIDs.Contains(playerId))
                session.InvitedPlayersIDs.Add(playerId);

            _gameInterface.SendInvite(playerId, sender);
        }

        public IBattlefield? AcceptInvite(User player, User sender)
            => AcceptInvite(player.Id, sender.Id);

        public IBattlefield? AcceptInvite(string playerId, UserDto sender)
        {
            User? activeSender = _activePlayers.FirstOrDefault(u => u.UserName == sender.UserName);

            if (activeSender is null)
                return null;

            return AcceptInvite(playerId, activeSender.Id);
        }

        public void ConfirmReady(User player)
            => ConfirmReady(player.Id);

        public void ConfirmReady(string playerId)
        {
            IGameSession? session = _sessionsManager.FindSession(playerId);

            if (session is null)
                return;

            session.Battlefields
                .First(b => b!.Owner!.Id == playerId)!
                .IsReady = true;
        }

        public IBattlefield? AcceptInvite(string playerId, string senderId)
        {
            if (GetPlayer(senderId) is null)
                return null;

            if (_sessionsManager.FindSession(playerId) is not null)
                return null;

            IGameSession? gameSession = _sessionsManager.FindSession(senderId);

            if (gameSession is null || 
                !gameSession.InvitedPlayersIDs.Contains(playerId) ||
                gameSession.Oponent is not null)
                return null;

            User? player = GetPlayer(playerId);

            if (player is null)
                return null;

            gameSession.AddOponent(player);

            return gameSession.Battlefields[1];
        }
    }
}
