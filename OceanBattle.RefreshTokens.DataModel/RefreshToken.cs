using OceanBattle.DataModel;

namespace OceanBattle.RefreshTokens.DataModel
{
    /// <summary>
    /// Data model representation of refresh token.
    /// </summary>
    public class RefreshToken : BaseModel
    {
        /// <summary>
        /// Randomly generated string representing refresh token.
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// Date when refresh token expires.
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Foreign key to <see cref="OceanBattle.DataModel.User"/> that this token belongs to.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Navigation property to <see cref="OceanBattle.DataModel.User"/> that this token belongs to.
        /// </summary>
        public User? User { get; set; }
    }
}