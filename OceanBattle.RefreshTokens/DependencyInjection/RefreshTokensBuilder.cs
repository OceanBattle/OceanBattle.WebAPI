using Microsoft.Extensions.DependencyInjection;
using OceanBattle.RefreshTokens.DataModel;

namespace OceanBattle.RefreshTokens.DependencyInjection
{
    /// <summary>
    /// Builder for creating <see cref="RefreshToken"/> configuration.
    /// </summary>
    public class RefreshTokensBuilder
    {
        /// <summary>
        /// Collection for registering services.
        /// </summary>
        public IServiceCollection Services { get; }

        public RefreshTokensBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
