using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OceanBattle.Rsa.Abstractions;
using System.Security.Cryptography;

namespace OceanBattle.Rsa
{
    public class RsaKeyFactory : IRsaKeyFactory
    {
        private readonly IConfiguration _configuration;

        public RsaKeyFactory(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<JsonWebKey> GetSecretKeys()
        {
            RSA rsa = RSA.Create();
            
            rsa.ImportRSAPrivateKey(
                source: Convert.FromBase64String(_configuration["Jwt:Jwks:SecretKey"]!),
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

        public IEnumerable<JsonWebKey> GetPublicKeys()
        {
            RSA rsa = RSA.Create();

            rsa.ImportRSAPublicKey(
                source: Convert.FromBase64String(_configuration["Bearer:Jwt:Jwks:PublicKey"]!),
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