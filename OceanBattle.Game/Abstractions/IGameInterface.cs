using OceanBattle.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanBattle.Game.Abstractions
{
    /// <summary>
    /// Represents part of game that interfaces user (eg. desktop UI or browser client).
    /// </summary>
    public interface IGameInterface
    {
        /// <summary>
        /// Sends information about game end to game interface.
        /// </summary>
        /// <param name="session">Session of this game.</param>
        Task GameEnded(IGameSession session);

        /// <summary>
        /// Sends information about being hit to game interface.
        /// </summary>
        /// <param name="session">Session of this game.</param>
        /// <param name="hitPlayer">Player that has been hit.</param>
        /// <param name="coordinates">Coordinates of hit.</param>
        Task GotHit(
            IGameSession session, 
            User hitPlayer, 
            (int x, int y) coordinates);

        /// <summary>
        /// Sends information about start of game to game interface.
        /// </summary>
        /// <param name="session">Session of this game.</param>
        Task GameStarted(IGameSession session);

        /// <summary>
        /// Sends information about start of deployment of units on battlefield
        /// to game interface.
        /// </summary>
        /// <param name="session">Session of this game.</param>
        /// <returns></returns>
        Task DeploymentStarted(IGameSession session);

        /// <summary>
        /// Sends information about completion of deployment of units on battlefield
        /// to game interface.
        /// </summary>
        /// <param name="session">Session of this game.</param>
        /// <returns></returns>
        Task DeploymentFinished(IGameSession session);

        /// <summary>
        /// Sends invite to player.
        /// </summary>
        /// <param name="recieverId">Uniqe Id of <see cref="User"/> that is invited.</param>
        /// <param name="sender"><see cref="User"/> that sent an invite.</param>
        /// <returns></returns>
        Task SendInvite(string recieverId, User sender);
    }
}
