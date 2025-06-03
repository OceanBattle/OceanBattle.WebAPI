using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OceanBattle.Data;
using OceanBattle.DataModel;
using OceanBattle.Game.DependencyInjection;
using OceanBattle.Jwt;
using OceanBattle.Jwt.DependencyInjection;
using OceanBattle.RefreshTokens;
using OceanBattle.RefreshTokens.DependencyInjection;
using OceanBattle.WebAPI.Game;
using OceanBattle.WebAPI.Hubs;

namespace OceanBattle
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string? connectionString = builder.Configuration.GetConnectionString("OceanBattle.Database");

            // Add services to the container.

            if (builder.Environment.IsProduction())
            {
                var connection = new SqliteConnection(connectionString);
                connection.Open();
                builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connection));
            }

            if (builder.Environment.IsDevelopment())
            {
                var connection = new SqliteConnection(connectionString);
                connection.Open();
                builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connection));
            }
            
            builder.Services.AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                
                // TODO: Set up SendGrid.

                // TODO: Configure email confirmation.
                
                //options.SignIn.RequireConfirmedEmail = true;
                
                // TODO: Configure account confirmation.

                //options.SignIn.RequireConfirmedAccount = true;

                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;

            }).AddEntityFrameworkStores<AppDbContext>()
              .AddDefaultTokenProviders();

            builder.Services.AddControllers()
                            .AddNewtonsoftJson(options => 
                            {
                                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                                options.SerializerSettings.Formatting = Formatting.None;
                                options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                                options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                                options.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                            });

            builder.Services.AddAuthentication()
                            .AddJwtBearer(builder.Configuration.GetSection(nameof(JwtOptions)), 
                                          builder.Configuration);

            builder.Services.AddRefreshTokens(builder.Configuration.GetSection(nameof(RefreshTokenOptions)))
                            .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.Configure<PasswordHasherOptions>(
                options => options.IterationCount = 500000);

            builder.Services.AddAuthorizationCore();
            builder.Services.AddOceanBattleGame<GameInterface>();

            builder.Services.AddSignalR()
                            .AddNewtonsoftJsonProtocol(options =>
                            {
                                options.PayloadSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                                options.PayloadSerializerSettings.Formatting = Formatting.None;
                                options.PayloadSerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                                options.PayloadSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                                options.PayloadSerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                                options.PayloadSerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                                options.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver();
                            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>()!;

                if (app.Environment.IsProduction())
                    dbContext.Database.Migrate();

                if (app.Environment.IsDevelopment())
                {
                    dbContext.Database.EnsureDeleted();
                    dbContext.Database.EnsureCreated();
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
          
            app.UseHttpsRedirection();

            app.UseHsts();
            app.UseAuthentication();
            app.UseJwtBlacklist();
            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<GameHub>("/gameHub");

            app.Run();
        }
    }
}
