namespace OceanBattle.Jwt
{
    /// <summary>
    /// Configuration of JSON Web Token.
    /// </summary>
    public class JwtOptions
    {
        /// <summary>
        /// Valid issuer of JWT.
        /// </summary>
        public string? Issuer { get; set; }

        /// <summary>
        /// Valid audience of JWT.
        /// </summary>
        public string? Audience { get; set; }

        /// <summary>
        /// Alghoritm encrypting JWT.
        /// </summary>
        public string? SecurityAlgorithm { get; set; }

        /// <summary>
        /// JWT lifespan.
        /// </summary>
        public TimeSpan Expires { get; set; }
    }
}
