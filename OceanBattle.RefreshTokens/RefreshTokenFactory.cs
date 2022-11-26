using Microsoft.Extensions.Options;
using OceanBattle.DataModel;
using OceanBattle.RefreshTokens.Abstractions;
using OceanBattle.RefreshTokens.DataModel;
using System.Security.Cryptography;

namespace OceanBattle.RefreshTokens
{
    /// <summary>
    /// Generating refresh tokens.
    /// </summary>
    public class RefreshTokenFactory : IRefreshTokenFactory
    {
        private readonly RefreshTokenOptions _options;

        public RefreshTokenFactory(IOptions<RefreshTokenOptions> options)
        {
            _options = options.Value;
        }

        /// <summary>
        /// Creates new refresh token.
        /// </summary>
        /// <param name="jti">ID of JSON Web Token to connect with this refresh token.</param>
        /// <param name="userId">ID of user that token is generated for.</param>
        /// <returns>Newly created refresh token.</returns>
        public RefreshToken CreateToken(Guid jti, string userId) => 
            new RefreshToken
            {
                Token = CreateToken(),
                ExpirationDate = DateTime.Now.Add(_options.Expires),
                Jti = jti,
                UserId = userId
            };

        /// <summary>
        /// Creates refresh token.
        /// </summary>
        /// <returns>Newly created refresh token.</returns>
        private string CreateToken()
        {
            var randomNumber = new byte[64];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}