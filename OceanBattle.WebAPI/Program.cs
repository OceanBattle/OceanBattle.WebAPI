using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OceanBattle.Data;
using OceanBattle.DataModel;
using OceanBattle.Jwt;
using OceanBattle.Jwt.DependencyInjection;
using OceanBattle.RefreshTokens;
using OceanBattle.RefreshTokens.DependencyInjection;

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
                builder.Services.AddDbContext<AppDbContext>(options => 
                options.UseSqlServer(connectionString));
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

            builder.Services.AddControllers();

            builder.Services.AddAuthentication()
                            .AddJwtBearer(builder.Configuration.GetSection(nameof(JwtOptions)), 
                                          builder.Configuration);

            builder.Services.AddRefreshTokens(builder.Configuration.GetSection(nameof(RefreshTokenOptions)))
                            .AddEntityFrameworkStores<AppDbContext>();

            builder.Services.Configure<PasswordHasherOptions>(
                options => options.IterationCount = 500000);

            builder.Services.AddAuthorizationCore();

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

            app.Run();
        }
    }
}