using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace OceanBattle.Jwt.Abstractions
{
    /// <summary>
    /// Creating JSON Web Tokens.
    /// </summary>
    public interface IJwtFactory
    {
        /// <summary>
        /// Creates JSON Web Token.
        /// </summary>
        /// <param name="user">User that token is generated for.</param>
        /// <returns>Generated <see cref="JwtSecurityToken"/> representing JSON Web Token.</returns>
        JwtSecurityToken CreateToken(IdentityUser uesr);
    }
}