using Microsoft.Extensions.DependencyInjection;
using OceanBattle.Rsa.Abstractions;

namespace OceanBattle.Rsa.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddRsaKeyFactory(this IServiceCollection services)
        {
            services.AddTransient<IRsaKeyFactory, RsaKeyFactory>();
            return services;
        }
    }
}
