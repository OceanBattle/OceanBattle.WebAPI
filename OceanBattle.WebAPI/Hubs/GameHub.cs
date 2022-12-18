using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using OceanBattle.DataModel;
using OceanBattle.DataModel.ClientData;
using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game;
using OceanBattle.DataModel.Game.Abstractions;
using OceanBattle.DataModel.Game.Exceptions;
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
        
        public bool CanBeHit(int x, int y)
        {
            IBattlefield? battlefield = GetBattlefield();

            if (battlefield is null)
                return false;

            return battlefield.CanBeHit(x, y);
        }

        public BattlefieldDto Hit(int x, int y, Weapon weapon)
        {
            IGameSession? session = 
                _sessionsManager.FindSession(Context.UserIdentifier!);

            if (session is null)
                throw new SessionNotFoundException(
                    "Player is not involved in any game session.");

            if (!session.IsActive)
                throw new SessionInactiveException(
                    "Session is not active.");

            IBattlefield? battlefield = 
                session.GetOponentBattlefield(Context.UserIdentifier!);

            if (battlefield is null)
                throw new OponentNotFoundException(
                    "Player does not have any oponent.");

            if (!battlefield.Hit(x, y, weapon))
                throw new InvalidHitException();

            return new BattlefieldDto
            {
                Grid = battlefield.AnonimizedGrid,
                Ships = battlefield.AnonimizedShips
            };
        }

        public void ConfirmReady() 
            => _playersManager.ConfirmReady(Context.UserIdentifier!);
        
        public bool CanPlaceShip(int x, int y, Ship ship)
        {
            IBattlefield? battlefield = GetBattlefield();

            if (battlefield is null)
                return false;

            return battlefield.CanPlaceShip(x, y, ship);
        }

        public BattlefieldDto PlaceShip(int x, int y, Ship ship)
        {
            IBattlefield? battlefield = GetBattlefield();

            if (battlefield is null)
                throw new SessionNotFoundException(
                    "Battlefield does not exist.");

            if (!battlefield.PlaceShip(x, y, ship))
                throw new InvalidPlacementException(
                    "Cannot place unit.");

            return new BattlefieldDto
            {
                Grid = battlefield.Grid,
                Ships = battlefield.Ships
            };
        }

        public IBattlefield AcceptInvite(UserDto inviteSender)
        {
            IBattlefield? battlefield = 
                _playersManager.AcceptInvite(Context.UserIdentifier!, inviteSender);

            if (battlefield is null)
                throw new SessionNotFoundException(
                    "Session does not exist.");

            return battlefield;
        }

        public void InvitePlayer(UserDto player)
        {
            _playersManager.InvitePlayer(player, Context.UserIdentifier!);
            //await Clients.User(invitedPlayer.Id)
            //    .InviteAsync(new UserDto 
            //    { 
            //        Email = sender.Email, 
            //        UserName = sender.UserName 
            //    });
        }

        public IBattlefield CreateSession(Level level)
        {         
            IGameSession? session = 
                _sessionsManager.CreateSession(Context.UserIdentifier!, level);

            if (session is null)
                throw new PlayerInvolvedInAnotherSessionException(
                    "Player is involved in another session.");

            return session.Battlefields[0]!;
        }

        public async Task MakeActive()
        {
            User user = await GetCurrentUserAsync();
            _playersManager.AddAsActive(user);
            await UpdateActiveUsersAsync();
        }

        public async Task MakeInactive()
        {
            _playersManager.RemoveFromActive(Context.UserIdentifier!);
            await UpdateActiveUsersAsync();
        }

        #region private helpers

        private IBattlefield? GetBattlefield()
        {
            IGameSession? session = 
                _sessionsManager.FindSession(Context.UserIdentifier!);

            if (session is null)
                return null;

            IBattlefield? battlefield = 
                session.GetBattlefield(Context.UserIdentifier!);

            return battlefield;
        }

        private async Task UpdateActiveUsersAsync() 
            => await Clients.Others.UpdateActiveUsersAsync(_playersManager.ActivePlayers.Select(u => new UserDto 
            {
                Email = u.Email,
                UserName = u.UserName 
            }));
        
        private async Task<User> GetCurrentUserAsync()
        {
            if (Context.User is null)
                throw new UnauthorizedAccessException();

            User? user = await _userManager.GetUserAsync(Context.User);

            if (user is null)
                throw new UnauthorizedAccessException();

            return user;
        }

        #endregion
    }
}
