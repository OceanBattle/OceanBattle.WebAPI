using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OceanBattle.Jwks.Abstractions;
using OceanBattle.Jwt.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OceanBattle.Jwt
{
    /// <summary>
    /// Creating JSON Web Tokens.
    /// </summary>
    public class JwtFactory : IJwtFactory
    {
        private readonly ILogger<JwtFactory> _logger;
        private readonly JwtOptions _options;
        private readonly IJwksFactory _jwksFactory;

        public JwtFactory(
            IOptions<JwtOptions> options,
            IJwksFactory jwksFactory,
            ILogger<JwtFactory> logger)
        {
            _options = options.Value;
            _jwksFactory = jwksFactory;
            _logger = logger;
        }

        /// <summary>
        /// Creates <see cref="JwtSecurityToken"/> JSON Web Token.
        /// </summary>
        /// <param name="claims">Security claims for new <see cref="JwtSecurityToken"/> JSON Web Token.</param>
        /// <returns>Generated <see cref="JwtSecurityToken"/> representing JSON Web Token.</returns>
        public JwtSecurityToken CreateToken(List<Claim> claims)
        {
            claims.RemoveAll(c => c.Type == JwtRegisteredClaimNames.Jti);
            claims.Insert(0, new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")));

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.Add(_options.Expires),
                SigningCredentials = new SigningCredentials(
                    _jwksFactory.GetSecretKeys().FirstOrDefault(),
                    _options.SecurityAlgorithm),
                Audience = _options.Audience,
                Issuer = _options.Issuer
            };

            return tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        }

        /// <summary>
        /// Creates JSON Web Token.
        /// </summary>
        /// <param name="user">User that token is generated for.</param>
        /// <returns>Generated <see cref="JwtSecurityToken"/> representing JSON Web Token.</returns>
        public JwtSecurityToken CreateToken(IdentityUser user)
        {
            List<Claim> defaultClaims = new List<Claim>
            {
                new Claim(
                    JwtRegisteredClaimNames.Jti, 
                    Guid.NewGuid().ToString("N")),

                new Claim(
                    JwtRegisteredClaimNames.NameId, 
                    user.Id),

                new Claim(
                    JwtRegisteredClaimNames.UniqueName, 
                    user.UserName!)
            };

            List<Claim> roleClaims = new List<Claim>();
            //(await _userManager.GetRolesAsync(request.User))
            //    .Select(role => new Claim(ClaimTypes.Role, role))
            //    .ToList();

            return CreateToken(defaultClaims.Concat(roleClaims).ToList());
        }
    }
}