using OceanBattle.DataModel;
using OceanBattle.DataModel.Game;
using OceanBattle.DataModel.Game.Abstractions;

namespace OceanBattle.Game.Abstractions
{
    public interface IGameSession
    {
        /// <summary>
        /// Level at which game is played.
        /// </summary>
        Level Level { get; }

        /// <summary>
        /// Signals completion of game session 
        /// (when all units of one of players are destroyed)
        /// </summary>
        IObservable<IGameSession> Completed { get; }

        /// <summary>
        /// Represents status of game, 
        /// <see langword="true"/> when gameplay is in progress, 
        /// <see langword="false"/> otherwise.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// <see cref="User"/> whose turn it is to play.
        /// </summary>
        User? Next { get; }

        /// <summary>
        /// <see cref="User"/> creator of <see cref="IGameSession"/> game session.
        /// </summary>
        User Creator { get; }

        /// <summary>
        /// <see cref="User"/> oponent that accepted invitation to join game 
        /// created by <see cref="User"/> <paramref name="Creator"/>.
        /// </summary>
        User? Oponent { get; }

        /// <summary>
        /// Array containing two <see cref="IBattlefield"/> battlefields 
        /// of this <see cref="IGameSession"/> game session.
        /// </summary>
        IBattlefield?[] Battlefields { get; }

        /// <summary>
        /// Gets <see cref="IBattlefield"/> owned by <paramref name="player"/>
        /// </summary>
        /// <param name="player"><see cref="User"/> that owns required <see cref="IBattlefield"/>.</param>
        /// <returns><see cref="IBattlefield"/> that is owned by <paramref name="player"/>.
        /// If no matching <see cref="IBattlefield"/> is found, returns <see langword="null"/>.</returns>
        IBattlefield? GetBattlefield(User player);

        /// <summary>
        /// Gets <see cref="IBattlefield"/> owned by <see cref="User"/> 
        /// with Id equal to <paramref name="playerId"/>
        /// </summary>
        /// <param name="playerId">Unique Id of <see cref="User"/> that owns required <see cref="IBattlefield"/>.</param>
        /// <returns><see cref="IBattlefield"/> that is owned by <see cref="User"/> with <paramref name="playerId"/>.
        /// If no matching <see cref="IBattlefield"/> is found, returns <see langword="null"/>.</returns>
        IBattlefield? GetBattlefield(string playerId);

        /// <summary>
        /// Gets <see cref="IBattlefield"/> owned by oponent of <paramref name="player"/>.
        /// </summary>
        /// <param name="player"><see cref="User"/> whose oponent's <see cref="IBattlefield"/> to get.</param>
        /// <returns><see cref="IBattlefield"/> of <paramref name="player"/>'s oponent.</returns>
        IBattlefield? GetOponentBattlefield(User player);

        /// <summary>
        /// Gets <see cref="IBattlefield"/> owned by oponent of <see cref="User"/> with Id equal to <paramref name="playerId"/>.
        /// </summary>
        /// <param name="playerId">Unique Id of <see cref="User"/> whose oponent's <see cref="IBattlefield"/> to get.</param>
        /// <returns><see cref="IBattlefield"/> of oponent of <see cref="User"/> with <paramref name="playerId"/>.</returns>
        IBattlefield? GetOponentBattlefield(string playerId);

        /// <summary>
        /// Adds oponent to this <see cref="IGameSession"/> game session.
        /// </summary>
        /// <param name="oponent"><see cref="User"/> <paramref name="oponent"/> to be added.</param>
        void AddOponent(User oponent);    
    }
}
