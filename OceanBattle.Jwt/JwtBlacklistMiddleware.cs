using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using OceanBattle.Jwt.Abstractions;
using System.Net;

namespace OceanBattle.Jwt
{
    /// <summary>
    /// Middleware for checking JSON Web Tokens against blacklist.
    /// </summary>
    public class JwtBlacklistMiddleware : IMiddleware
    {
        private readonly IDistributedCache _cache;
        private readonly IJwtService _jwtService;

        public JwtBlacklistMiddleware(
            IDistributedCache cache,
            IJwtService jwtService) 
        { 
            _cache = cache;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Performs check on blacklist for current JSON Web Token.
        /// </summary>
        /// <param name="context">Current <see cref="HttpContext"/>.</param>
        /// <param name="next">Next middleware in chain.</param>
        /// <returns><see cref="Task"/> representing <see langword="async"/> operation.</returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (await _jwtService.IsTokenBlacklistedAsync(context.User.Claims.ToList()))
            { 
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return;
            }

            await next(context);
        }
    }
}
