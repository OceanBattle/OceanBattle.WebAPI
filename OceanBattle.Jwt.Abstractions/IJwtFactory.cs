using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace OceanBattle.Jwt.Abstractions
{
    /// <summary>
    /// Creating JSON Web Tokens.
    /// </summary>
    public interface IJwtFactory
    {
        /// <summary>
        /// Creates <see cref="JwtSecurityToken"/> JSON Web Token.
        /// </summary>
        /// <param name="user">User that token is generated for.</param>
        /// <returns>Generated <see cref="JwtSecurityToken"/> representing JSON Web Token.</returns>
        JwtSecurityToken CreateToken(IdentityUser uesr);

        /// <summary>
        /// Creates <see cref="JwtSecurityToken"/> JSON Web Token.
        /// </summary>
        /// <param name="claims">Security claims for new <see cref="JwtSecurityToken"/> JSON Web Token.</param>
        /// <returns>Generated <see cref="JwtSecurityToken"/> representing JSON Web Token.</returns>
        JwtSecurityToken CreateToken(List<Claim> claims);
    }
}