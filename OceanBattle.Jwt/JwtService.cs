using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OceanBattle.Jwt.Abstractions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OceanBattle.Jwt
{
    /// <summary>
    /// Service for performing basic operations on JSON Web Tokens.
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly TokenValidationParameters _tokenValidationParameters;

        public JwtService(IOptionsMonitor<JwtBearerOptions> options)
        {
            _tokenValidationParameters = 
                options.Get(JwtBearerDefaults.AuthenticationScheme)
                       .TokenValidationParameters;
        }

        /// <summary>
        /// Manually validates JSON Web Token that can be expired (ignores token lifetime).
        /// </summary>
        /// <param name="token">String representing JSON Web Token.</param>
        /// <returns><see cref="TokenValidationResult"/> of validation.</returns>
        public async Task<TokenValidationResult> ValidateExpiredTokenAsync(string token)
        {
            TokenValidationParameters validationParameters = 
                _tokenValidationParameters.Clone();
            validationParameters.ValidateLifetime = false;

            var tokenHandler = new JwtSecurityTokenHandler();

            return await tokenHandler.ValidateTokenAsync(token, validationParameters);
        }

        /// <summary>
        /// Manually validates JSON Web Token.
        /// </summary>
        /// <param name="token">String representing JSON Web Token.</param>
        /// <returns><see cref="TokenValidationResult"/> of validation.</returns>
        public async Task<TokenValidationResult> ValidateTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            return await tokenHandler.ValidateTokenAsync(token, _tokenValidationParameters);
        }
    }
}
