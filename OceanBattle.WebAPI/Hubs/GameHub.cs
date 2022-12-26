using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OceanBattle.Data;
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
        private readonly AppDbContext _dbContext;

        public GameHub(
            ISessionsManager sessionsManager, 
            IPlayersManager playersManager,
            UserManager<User> userManager,
            AppDbContext dbContext)
        {
            _playersManager = playersManager;
            _sessionsManager = sessionsManager;
            _userManager = userManager;
            _dbContext = dbContext;
        }
        
        public bool CanBeHit(int x, int y)
        {
            IGameSession? session =
               _sessionsManager.FindSession(Context.UserIdentifier!);

            if (session is null)
                return false;

            IBattlefield? battlefield = 
                session.GetOponentBattlefield(Context.UserIdentifier!);

            if (battlefield is null)
                return false;

            return battlefield.CanBeHit(x, y);
        }

        public BattlefieldDto Hit(int x, int y, Weapon weapon)
        {
            IGameSession? session = 
                _sessionsManager.FindSession(Context.UserIdentifier!);

            if (session is null)
                throw new SessionNotFoundException();

            IBattlefield? battlefield = 
                session.GetBattlefield(Context.UserIdentifier!);

            if (battlefield is null)
                throw new SessionNotFoundException();

            // Validate weapon used.
            if (!ValidateWeapon(weapon, battlefield.Ships))
                throw new InvalidHitException();

            if (!session.IsActive)
                throw new SessionInactiveException();

            if (session.Next is null || 
                session.Next.Id != Context.UserIdentifier!)
                throw new NotYourTurnException();

            battlefield = 
                session.GetOponentBattlefield(Context.UserIdentifier!);

            if (battlefield is null)
                throw new OponentNotFoundException();

            if (!battlefield.Hit(x, y, weapon))
                throw new InvalidHitException();

            return new BattlefieldDto
            {
                Grid = battlefield.AnonimizedGrid,
                Ships = battlefield.AnonimizedShips
            };
        }

        public void ConfirmReady()
        {
            bool result = _playersManager.ConfirmReady(Context.UserIdentifier!);

            if (!result)
                throw new NotAllShipsPlacedException();
        }            
        
        public bool CanPlaceShip(int x, int y, Ship ship)
        {
            if (!ValidateShip(ship))
                return false;

            IBattlefield? battlefield = GetBattlefield();

            if (battlefield is null)
                return false;

            return battlefield.CanPlaceShip(x, y, ship);
        }

        public BattlefieldDto PlaceShip(int x, int y, Ship ship)
        {
            if (!ValidateShip(ship))
                throw new ShipNotAvailableException();

            IBattlefield? battlefield = GetBattlefield();

            if (battlefield is null)
                throw new SessionNotFoundException();

            if (!battlefield.PlaceShip(x, y, ship))
                throw new InvalidPlacementException();

            return new BattlefieldDto
            {
                Grid = battlefield.Grid,
                Ships = battlefield.Ships
            };
        }

        public BattlefieldDto AcceptInvite(UserDto inviteSender)
        {
            IBattlefield? battlefield = 
                _playersManager.AcceptInvite(Context.UserIdentifier!, inviteSender);

            if (battlefield is null)
                throw new SessionNotFoundException();

            return new BattlefieldDto 
            { 
                Grid = battlefield.Grid, 
                Ships = battlefield.Ships 
            };
        }

        public void InvitePlayer(UserDto player) 
            => _playersManager.InvitePlayer(player, Context.UserIdentifier!);

        public IBattlefield CreateSession(Level level)
        {         
            IGameSession? session = 
                _sessionsManager.CreateSession(Context.UserIdentifier!, level);

            if (session is null)
                throw new PlayerInvolvedInAnotherSessionException();

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

        private bool ValidateShip(Ship ship)
        {
            User user = _dbContext.Users.Where(u => u.Id == Context.UserIdentifier)
                                        .Include(u => u.OwnedVessels)
                                        .FirstOrDefault()!;

            if (user.OwnedVessels is null)
                return false;

            Ship? shipToPlace = user.OwnedVessels.FirstOrDefault(s => s.Id == ship.Id);

            if (shipToPlace is null) 
                return false;

            ship = shipToPlace;

            return true;
        }

        private bool ValidateWeapon(Weapon weapon, IEnumerable<Ship> ships)
        {
            bool weaponIsValid = false;

            foreach (Warship warship in ships.Where(s => !s.IsDestroyed))
            {
                if (warship.Weapons is not null)
                {
                    weaponIsValid =
                        warship.Weapons.Any(
                            w => w.GetType() == weapon.GetType() &&
                                 w.Damage == weapon.Damage &&
                                 w.DamageRadius == weapon.DamageRadius);
                }

                if (weaponIsValid)
                    return true;
            }

            return false;
        }

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
                throw new UnauthorizedAccessException("UnauthorizedAccessException");

            User? user = await _userManager.GetUserAsync(Context.User);

            if (user is null)
                throw new UnauthorizedAccessException("UnauthorizedAccessException");

            return user;
        }

        #endregion
    }
}
