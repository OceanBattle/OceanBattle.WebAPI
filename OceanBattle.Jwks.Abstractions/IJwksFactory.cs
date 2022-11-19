using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace OceanBattle.Jwks.Abstractions
{
    /// <summary>
    /// Creating JSON Web Key Sets.
    /// </summary>
    public interface IJwksFactory
    {
        /// <summary>
        /// Creates secret keys based on RSA key in <see cref="IConfiguration"/>.
        /// </summary>
        /// <returns><see cref="IEnumerable{JsonWebKey}>"/> containing generated keys.</returns>
        IEnumerable<JsonWebKey> GetSecretKeys();

        /// <summary>
        /// Creates public keys based on RSA key in <see cref="IConfiguration"/>.
        /// </summary>
        /// <returns><see cref="IEnumerable{JsonWebKey}>"/> containing generated keys.</returns>
        IEnumerable<JsonWebKey> GetPublicKeys();
    }
}