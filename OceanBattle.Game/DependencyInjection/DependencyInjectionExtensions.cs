using Microsoft.Extensions.DependencyInjection;
using OceanBattle.Game.Abstractions;
using OceanBattle.Game.Services;
using AspNetCoreInjection.TypedFactories;
using OceanBattle.DataModel.Game;
using OceanBattle.Game.Models;
using OceanBattle.Game.Repositories;

namespace OceanBattle.Game.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddOceanBattleGame<Tinterface>(this IServiceCollection services) 
            where Tinterface : class, IGameInterface
        {
            services.AddSingleton<IPlayersManager, PlayersManager>();
            services.AddSingleton<ISessionsManager, SessionsManager>();
            services.RegisterTypedFactory<IBattlefieldFactory>().ForConcreteType<Battlefield>();
            services.AddTransient<IGameInterface, Tinterface>();
            services.RegisterTypedFactory<ISessionFactory>().ForConcreteType<Session>();
            services.AddTransient<ILevelsRepository, LevelsRepository>();
            services.AddTransient<IShipsRepository, ShipsRepository>();
            services.AddTransient<PlayersManagerFactory>(provider => 
            () => provider.GetRequiredService<IPlayersManager>());

            return services;
        }
    }
}
