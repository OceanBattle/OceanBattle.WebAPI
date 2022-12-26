using Microsoft.AspNetCore.SignalR;
using OceanBattle.DataModel;
using OceanBattle.DataModel.ClientData;
using OceanBattle.DataModel.DTOs;
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

            await clients.StartDeploymentAsync();
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
            IGameClient? clients = GetGameClients(session);

            if (clients is null)
                return;

            await clients.StartGameAsync();
        }

        public async Task GotHit(IGameSession session, User hitPlayer, (int x, int y) coordinates)
        {
            IGameClient? clients = GetGameClients(session);

            if (clients is null)
                return;

            await clients.GotHitAsync();
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
