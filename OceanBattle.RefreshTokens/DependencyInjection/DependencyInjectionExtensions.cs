using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OceanBattle.RefreshTokens.Abstractions;
using OceanBattle.RefreshTokens.DataModel;

namespace OceanBattle.RefreshTokens.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds services for creation and handling of <see cref="RefreshToken"/> refresh tokens.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> services container.</param>
        /// <param name="refreshTokenOptions"><see cref="IConfigurationSection"/> containing <see cref="RefreshTokenOptions"/>.</param>
        /// <returns></returns>
        public static RefreshTokensBuilder AddRefreshTokens(
            this IServiceCollection services, 
            IConfigurationSection refreshTokenOptions)
        {
            services.Configure<RefreshTokenOptions>(refreshTokenOptions);
            services.AddTransient<IRefreshTokenFactory, RefreshTokenFactory>();
            return new RefreshTokensBuilder(services);
        }
    }
}
