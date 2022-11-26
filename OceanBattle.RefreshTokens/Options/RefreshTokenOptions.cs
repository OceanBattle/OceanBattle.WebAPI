using OceanBattle.RefreshTokens.DataModel;

namespace OceanBattle.RefreshTokens
{
    /// <summary>
    /// Configuration of <see cref="RefreshToken"/> refresh tokens.
    /// </summary>
    public class RefreshTokenOptions
    {
        /// <summary>
        /// <see cref="RefreshToken"/> life span.
        /// </summary>
        public TimeSpan Expires { get; set; }
    }
}
