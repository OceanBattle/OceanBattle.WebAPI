namespace OceanBattle.Jwks
{
    /// <summary>
    /// JSON Web Key Set configuration.
    /// </summary>
    public class JwksOptions
    {
        /// <summary>
        /// Private key of JWKS.
        /// </summary>
        public string? SecretKey { get; set; }

        /// <summary>
        /// Public key of JWKS.
        /// </summary>
        public string? PublicKey { get; set; }
        
    }
}
