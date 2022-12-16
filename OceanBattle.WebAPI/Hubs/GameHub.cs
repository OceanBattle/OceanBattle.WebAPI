using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client;
using OceanBattle.DataModel;
using OceanBattle.DataModel.ClientData;
using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game;
using OceanBattle.DataModel.Game.Abstractions;
using OceanBattle.Game.Abstractions;

namespace OceanBattle.WebAPI.Hubs
{
    [Authorize]
    public class GameHub : Hub<IGameClient>
    {
        private readonly ISessionsManager _sessionsManager;
        private readonly IPlayersManager _playersManager;
        private readonly UserManager<User> _userManager;

        public GameHub(
            ISessionsManager sessionsManager, 
            IPlayersManager playersManager,
            UserManager<User> userManager)
        {
            _playersManager = playersManager;
            _sessionsManager = sessionsManager;
            _userManager = userManager;
        }
        
        public async Task Hit(int x, int y, Weapon weapon)
        {

        }

        public async Task PlaceShip(int x, int y, Ship ship)
        {

        }

        public async Task<IBattlefield?> AcceptInvite(UserDto? inviteSender)
        {
            if (Context.User is null)
                throw new UnauthorizedAccessException();

            if (inviteSender is null)
                return null;

            User? user = await _userManager.GetUserAsync(Context.User);

            if (user is null)
                throw new NotImplementedException();

            IGameSession? session = 
                _sessionsManager.Sessions.FirstOrDefault(s => s.Creator.Email == inviteSender.Email);

            if (session is null)
                return null;

            session.AddOponent(user);

            return session.Battlefields[1];
        }

        public async Task InvitePlayer(UserDto player)
        {
            if (Context.User is null)
                throw new UnauthorizedAccessException();

            User? invitedPlayer = 
                _playersManager.ActivePlayers.FirstOrDefault(p => p.Email == player.Email);
            User? sender = await _userManager.GetUserAsync(Context.User);

            if (sender is null)
                throw new UnauthorizedAccessException();

            if (invitedPlayer is null)
                return;

            await Clients.User(invitedPlayer.Id)
                .InviteAsync(new UserDto 
                { 
                    Email = sender.Email, 
                    UserName = sender.UserName 
                });
        }

        public async Task<IBattlefield> CreateSession(Level level)
        {
            if (Context.User is null)
                throw new UnauthorizedAccessException();

            User? user = await _userManager.GetUserAsync(Context.User);

            if (user is null)
                throw new UnauthorizedAccessException();

            IGameSession session = _sessionsManager.CreateSession(user, level);

            return session.Battlefields[0]!;
        }

        public async Task MakeActive()
        {
            if (Context.User is null)
                throw new UnauthorizedAccessException();

            User? user = await _userManager.GetUserAsync(Context.User);

            if (user is null)
                throw new UnauthorizedAccessException();

            _playersManager.AddAsActive(user);

            await Clients.Others.UpdateActiveUsersAsync(
                _playersManager.ActivePlayers.Select(u => new UserDto 
                { 
                    Email = u.Email, 
                    UserName = u.UserName 
                }));
        }

        public async Task MakeInactive()
        {
            if (Context.User is null)
                throw new UnauthorizedAccessException();

            User? user = await _userManager.GetUserAsync(Context.User);

            if (user is null)
                throw new UnauthorizedAccessException();

            _playersManager.RemoveFromActive(user);

            await Clients.Others.UpdateActiveUsersAsync(
                _playersManager.ActivePlayers.Select(u => new UserDto 
                {
                    Email = u.Email,
                    UserName = u.UserName
                }));
        }
    }
}
