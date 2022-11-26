using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OceanBattle.Jwks.Abstractions;

namespace OceanBattle.Jwks.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds services for creation of JSON Web Token Sets.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options">Configuratipn section being bound.</param>
        /// <returns><see cref="IServiceCollection"/> so additional calls can be made.</returns>
        public static IServiceCollection AddJwks(this IServiceCollection services, IConfigurationSection options)
        {
            services.Configure<JwksOptions>(options);
            services.AddTransient<IJwksFactory, JwksFactory>();
            return services;
        }
    }
}
