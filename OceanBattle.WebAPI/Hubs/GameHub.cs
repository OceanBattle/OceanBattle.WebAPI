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
using OceanBattle.DataModel.Game.Ships;
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


            if (session.Next is null ||
                session.Next.Id != Context.UserIdentifier!)
                return false;

            return battlefield.CanBeHit(x, y);
        }

        public BattlefieldDto? Hit(int x, int y, Weapon weapon)
        {
            IGameSession? session = 
                _sessionsManager.FindSession(Context.UserIdentifier!);

            if (session is null)
                //throw new SessionNotFoundException();
                return null;

            IBattlefield? battlefield = 
                session.GetBattlefield(Context.UserIdentifier!);

            if (battlefield is null)
                //throw new SessionNotFoundException();
                return null;

            // Validate weapon used.
            if (!ValidateWeapon(weapon, battlefield.Ships))
                //throw new InvalidHitException();
                return null;

            if (!session.IsActive)
                //throw new SessionInactiveException();
                return null;

            if (session.Next is null ||
                session.Next.Id != Context.UserIdentifier!)
                //throw new NotYourTurnException();
                return null;

            battlefield = 
                session.GetOponentBattlefield(Context.UserIdentifier!);

            if (battlefield is null)
                //throw new OponentNotFoundException();
                return null;

            if (!battlefield.Hit(x, y, weapon))
                //throw new InvalidHitException();
                return null;

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

        public BattlefieldDto? PlaceShip(int x, int y, Ship ship)
        {
            if (!ValidateShip(ship))
                //throw new ShipNotAvailableException();
                return null;

            IBattlefield? battlefield = GetBattlefield();

            if (battlefield is null)
                throw new SessionNotFoundException();

            if (!battlefield.PlaceShip(x, y, ship))
                //throw new InvalidPlacementException();
                return null;

            return new BattlefieldDto
            {
                Grid = battlefield.Grid,
                Ships = battlefield.Ships
            };
        }

        public BattlefieldDto? AcceptInvite(UserDto inviteSender)
        {
            IBattlefield? battlefield = 
                _playersManager.AcceptInvite(Context.UserIdentifier!, inviteSender);

            if (battlefield is null)
                //throw new SessionNotFoundException();
                return null;

            return new BattlefieldDto 
            { 
                Grid = battlefield.Grid, 
                Ships = battlefield.Ships 
            };
        }

        public void InvitePlayer(UserDto player) 
            => _playersManager.InvitePlayer(player, Context.UserIdentifier!);

        public BattlefieldDto? CreateSession(LevelDto levelDto)
        {
            Level level = new Level
            {
                Id = levelDto.Id,
                BattlefieldSize = levelDto.BattlefieldSize,
                AvailableTypes = levelDto.AvailableTypes is null ? new Dictionary<Type, int>() : 
                levelDto.AvailableTypes.ToDictionary(kvp => typesReversed[kvp.Key], kvp => kvp.Value)
            };

            IGameSession? session = 
                _sessionsManager.CreateSession(Context.UserIdentifier!, level);

            if (session is null)
                //throw new PlayerInvolvedInAnotherSessionException();
                return null;

            return new BattlefieldDto
            {
                Grid = session.Battlefields[0]!.Grid,
                Ships = session.Battlefields[0]!.Ships
            };
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

        private readonly Dictionary<string, Type> typesReversed = new Dictionary<string, Type>
        {
            { nameof(Corvette), typeof(Corvette) },
            { nameof(Frigate), typeof(Frigate) },
            { nameof(Destroyer), typeof(Destroyer) },
            { nameof(Cruiser), typeof(Cruiser) },
            { nameof(Battleship), typeof(Battleship) }
        };

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
