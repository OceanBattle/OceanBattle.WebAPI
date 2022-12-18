using OceanBattle.DataModel;
using OceanBattle.DataModel.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanBattle.Game.Abstractions
{
    /// <summary>
    /// Manages game sessions.
    /// </summary>
    public interface ISessionsManager
    {
        /// <summary>
        /// All currently running game sessions.
        /// </summary>
        IEnumerable<IGameSession> Sessions { get; }

        /// <summary>
        /// Creates new game session.
        /// </summary>
        /// <param name="creator"><see cref="User"/> that creates this session.</param>
        /// <param name="level"><see cref="Level"/> at which game is to be played.</param>
        /// <returns><see cref="IGameSession"/> if no session is already running for <paramref name="creator"/>.
        /// Otherwise <see langword="null"/>.</returns>
        IGameSession? CreateSession(User creator, Level level);

        /// <summary>
        /// Creates new game session.
        /// </summary>
        /// <param name="creatorId">Unique Id of <see cref="User"/> that creates this session.</param>
        /// <param name="level"><see cref="Level"/> at which game is to be played.</param>
        /// <returns><see cref="IGameSession"/> if no session is already running for <paramref name="creator"/>.
        /// Otherwise <see langword="null"/>.</returns>
        IGameSession? CreateSession(string creatorId, Level level);

        /// <summary>
        /// Finds game session that <paramref name="participant"/> is involved in. 
        /// </summary>
        /// <param name="participant"><see cref="User"/> player that is involved in game session.</param>
        /// <returns><see cref="IGameSession"/> if any game session is found.
        /// Otherwise <see langword="null"/>.</returns>
        IGameSession? FindSession(User participant);

        /// <summary>
        /// Finds game session that participant with matching <paramref name="participantId"/> is involved in. 
        /// </summary>
        /// <param name="participantId">Unique Id of <see cref="User"/> participant in searched game session.</param>
        /// <returns><see cref="IGameSession"/> if any game session is found.
        /// Otherwise <see langword="null"/>.</returns>
        IGameSession? FindSession(string participantId);
    
    }
}
