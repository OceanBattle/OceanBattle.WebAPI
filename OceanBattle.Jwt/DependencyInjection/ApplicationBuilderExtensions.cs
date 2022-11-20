using Microsoft.AspNetCore.Builder;

namespace OceanBattle.Jwt.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds <see cref="JwtBlacklistMiddleware"/> to <see cref="IApplicationBuilder"/>, 
        /// which enables support for checking JSON Web Tokens against blacklist.
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/> instance.</param>
        /// <returns><see cref="IApplicationBuilder"/> instance for further configuration.</returns>
        public static IApplicationBuilder UseJwtBlacklist(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JwtBlacklistMiddleware>();
        }
    }
}
