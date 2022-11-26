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
        /// ID of JSON Web Token corresponding to this refresh token.
        /// </summary>
        public Guid Jti { get; set; }

        /// <summary>
        /// ID of User that token is assigned to.
        /// </summary>
        public string? UserId { get; set; }
    }
}