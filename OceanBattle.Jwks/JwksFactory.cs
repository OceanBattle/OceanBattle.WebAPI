using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OceanBattle.Jwks.Abstractions;
using System.Security.Cryptography;

namespace OceanBattle.Jwks
{
    /// <summary>
    /// Creating JSON Web Key Sets.
    /// </summary>
    public class JwksFactory : IJwksFactory
    {
        private readonly ILogger<JwksFactory> _logger;
        private readonly JwksOptions _options;

        public JwksFactory(
            ILogger<JwksFactory> logger,
            IOptions<JwksOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        /// <summary>
        /// Creates secret keys based on RSA key in <see cref="IConfiguration"/>.
        /// </summary>
        /// <returns><see cref="IEnumerable{JsonWebKey}>"/> containing generated keys.</returns>
        public IEnumerable<JsonWebKey> GetSecretKeys()
        {
            RSA rsa = RSA.Create();
            
            rsa.ImportRSAPrivateKey(
                source: Convert.FromBase64String(_options.SecretKey!),
                bytesRead: out int _);

            RsaSecurityKey privateKey = new RsaSecurityKey(rsa)
            {
                KeyId = "0"
            };

            List<JsonWebKey> privateKeys = new List<JsonWebKey>
            {
                JsonWebKeyConverter.ConvertFromRSASecurityKey(privateKey)
            };

            return privateKeys;
        }

        /// <summary>
        /// Creates public keys based on RSA key in <see cref="IConfiguration"/>.
        /// </summary>
        /// <returns><see cref="IEnumerable{JsonWebKey}>"/> containing generated keys.</returns>
        public IEnumerable<JsonWebKey> GetPublicKeys()
        {
            RSA rsa = RSA.Create();

            rsa.ImportRSAPublicKey(
                source: Convert.FromBase64String(_options.PublicKey!),
                bytesRead: out int _);

            RsaSecurityKey publicKey = new RsaSecurityKey(rsa)
            {
                KeyId = "0"
            };

            List<JsonWebKey> publicKeys = new List<JsonWebKey>
            {
                JsonWebKeyConverter.ConvertFromRSASecurityKey(publicKey)
            };

            return publicKeys;
        }
    }
}