using OceanBattle.DataModel;
using OceanBattle.DataModel.DTOs;
using OceanBattle.DataModel.Game.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanBattle.Game.Abstractions
{
    public interface IPlayersManager
    {
        /// <summary>
        /// List of players that are currently active.
        /// </summary>
        public IEnumerable<User> ActivePlayers { get; }

        /// <summary>
        /// Checks if player is in active players list.
        /// </summary>
        /// <param name="Id">Unique Id of player.</param>
        /// <returns><see langword="true"/> if player is active,
        /// <see langword="false"/> otherwise.</returns>
        public bool IsPlayerActive(string Id);

        /// <summary>
        /// Gets <see cref="User"/> from active players list by Id.
        /// </summary>
        /// <param name="Id">Unique Id of <see cref="User"/>.</param>
        /// <returns><see cref="User"/> that has matching <paramref name="Id"/>.
        /// <see langword="null"/> if no such <see cref="User"/> found.</returns>
        public User? GetPlayer(string Id);

        /// <summary>
        /// Adds player to the list of currently active <see cref="User"/>s.
        /// </summary>
        /// <param name="player"><see cref="User"/> to add to list of active players.</param>
        void AddAsActive(User player);

        /// <summary>
        /// Confirms that <see cref="User"/> is ready to start game.
        /// </summary>
        /// <param name="player"><see cref="User"/> that is ready.</param>
        public void ConfirmReady(User player);

        /// <summary>
        /// Confirms that <see cref="User"/> is ready to start game.
        /// </summary>
        /// <param name="playerId">Unique Id of <see cref="User"/> that is ready.</param>
        public void ConfirmReady(string playerId);

        /// <summary>
        /// Removes player from the list of currently active <see cref="User"/>s.
        /// </summary>
        /// <param name="player"><see cref="User"/> to add to list of active players.</param>
        void RemoveFromActive(User player);

        /// <summary>
        /// Removes player from the list of currently active <see cref="User"/>s.
        /// </summary>
        /// <param name="playerId">Unique Id of <see cref="User"/> to add to list of active players.</param>
        void RemoveFromActive(string playerId);

        /// <summary>
        /// Invites player to the game session.
        /// </summary>
        /// <param name="player"><see cref="User"/> to invite.</param>
        /// <param name="sender"><see cref="User"/> that sends the invite.</param>
        void InvitePlayer(User player, User sender);

        /// <summary>
        /// Invites player to the game session.
        /// </summary>
        /// <param name="player"><see cref="UserDto"/> object representing <see cref="User"/> to invite.</param>
        /// <param name="senderId">Unique Id of <see cref="User"/> that sends the invite.</param>
        void InvitePlayer(UserDto player, string senderId);

        /// <summary>
        /// Invites player to the game session.
        /// </summary>
        /// <param name="playerId">Unique Id of <see cref="User"/> to invite.</param>
        /// <param name="senderId">Unique Id of <see cref="User"/> that sends the invite.</param>
        void InvitePlayer(string playerId, string senderId);

        /// <summary>
        /// Accepts invite from antother <see cref="User"/>.
        /// </summary>
        /// <param name="player"><see cref="User"/> that accepted invite.</param>
        /// <param name="sender"><see cref="User"/> that sent invite.</param>
        /// <returns><see cref="IBattlefield"/> associated with invited player.</returns>
        IBattlefield? AcceptInvite(User player, User sender);

        /// <summary>
        /// Accepts invite from antother <see cref="User"/>.
        /// </summary>
        /// <param name="playerId">Unique Id of <see cref="User"/> that accepted invite.</param>
        /// <param name="sender"><see cref="UserDto"/> object representing <see cref="User"/> that sent invite.</param>
        /// <returns><see cref="IBattlefield"/> associated with invited player.</returns>
        IBattlefield? AcceptInvite(string playerId, UserDto sender);

        /// <summary>
        /// Accepts invite from antother <see cref="User"/>.
        /// </summary>
        /// <param name="playerId">Unique Id of <see cref="User"/> that accepted invite.</param>
        /// <param name="senderId">Unique Id of <see cref="User"/> that sent invite.</param>
        /// <returns><see cref="IBattlefield"/> associated with invited player.</returns>
        IBattlefield? AcceptInvite(string playerId, string senderId);
    }
}
