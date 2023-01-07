using Microsoft.AspNetCore.SignalR;
using OceanBattle.DataModel;
using OceanBattle.DataModel.ClientData;
using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game.Abstractions;
using OceanBattle.Game.Abstractions;
using OceanBattle.Game.Models;
using OceanBattle.WebAPI.Hubs;

namespace OceanBattle.WebAPI.Game
{
    public class GameInterface : IGameInterface
    {
        private readonly IHubContext<GameHub, IGameClient> _hubContext;

        public GameInterface(IHubContext<GameHub, IGameClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task FinishDeployment(IGameSession session)
        {
            IGameClient? clients = GetGameClients(session);

            if (clients is null)
                return;

            await clients.FinishDeploymentAsync();
        }

        public async Task StartDeployment(IGameSession session)
        {
            IGameClient? clients = GetGameClients(session);

            if (clients is null)
                return;

            LevelDto level = new LevelDto
            {
                Id = session.Level.Id,
                BattlefieldSize = session.Level.BattlefieldSize,
                AvailableTypes = session.Level.AvailableTypes is null ? null :
                session.Level.AvailableTypes.ToDictionary(kvp => kvp.Key.Name, kvp => kvp.Value)
            };

            await clients.StartDeploymentAsync(level);
        }

        public async Task EndGame(IGameSession session)
        {
            IGameClient? clients = GetGameClients(session);

            if (clients is null)
                return;

            await clients.EndGameAsync();
        }

        public async Task StartGame(IGameSession session)
        {
            if (session.Creator is null || 
                session.Oponent is null)
                return;

            IBattlefield? oponentBattlefield = 
                session.GetOponentBattlefield(session.Creator.Id);

            if (oponentBattlefield is null)
                return;

            IBattlefield? creatorBattlefield =
                session.GetOponentBattlefield(session.Oponent.Id);

            if (creatorBattlefield is null)
                return;

            await _hubContext.Clients.User(session.Creator.Id)
                .StartGameAsync(new BattlefieldDto
                {
                    Grid = oponentBattlefield.AnonimizedGrid,
                    Ships = oponentBattlefield.AnonimizedShips
                });

            await _hubContext.Clients.User(session.Oponent.Id)
                .StartGameAsync(new BattlefieldDto
                {
                    Grid = creatorBattlefield.AnonimizedGrid,
                    Ships = creatorBattlefield.AnonimizedShips
                });
        }

        public async Task GotHit(IGameSession session, User hitPlayer, (int x, int y) coordinates)
        {
            IBattlefield? battlefield = session.GetBattlefield(hitPlayer);
            
            if (battlefield is null) 
                return;

            BattlefieldDto battlefieldDto = new BattlefieldDto
            {
                Grid = battlefield.Grid,
                Ships = battlefield.Ships
            };

            await _hubContext.Clients.User(hitPlayer.Id)
                .GotHitAsync(battlefieldDto, coordinates.x, coordinates.y);
        }

        public async Task SendInvite(string recieverId, User sender) 
            => await _hubContext.Clients.User(recieverId).InviteAsync(new UserDto
            {
                UserName = sender.UserName,
                Email = sender.Email
            });

        #region private helpers
        
        private IGameClient? GetGameClients(IGameSession session)
        {
            if (session.Oponent is null)
                return null;

            return _hubContext.Clients.Users(session.Creator.Id, session.Oponent.Id);
        }

        #endregion private helpers
    }
}
