using OceanBattle.DataModel;
using OceanBattle.RefreshTokens.DataModel;

namespace OceanBattle.RefreshTokens.Abstractions
{
    /// <summary>
    /// Generating refresh tokens.
    /// </summary>
    public interface IRefreshTokenFactory
    {
        /// <summary>
        /// Creates new refresh token.
        /// </summary>
        /// <param name="user">User for whom token is to be created.</param>
        /// <returns>Newly created refresh token.</returns>
        RefreshToken CreateToken(User user);
    }
}