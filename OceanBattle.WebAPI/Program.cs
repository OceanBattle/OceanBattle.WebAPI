using OceanBattle.DataModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OceanBattle.Data;
using OceanBattle.Rsa.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OceanBattle.Rsa;
using OceanBattle.Rsa.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;

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
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddRsaKeyFactory();
            builder.Services.AddControllers();

            builder.Services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    IRsaKeyFactory keyFactory = (IRsaKeyFactory)Activator.CreateInstance(
                        typeof(RsaKeyFactory),
                        builder.Configuration)!;
                    
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAlgorithms = new List<string> { SecurityAlgorithms.RsaSha512 },
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKeys = keyFactory.GetPublicKeys(),
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateAudience = true,
                        RequireAudience = true,
                        RequireSignedTokens = true,
                        ValidateTokenReplay = true,
                        RequireExpirationTime = true
                    };
                });

            builder.Services.Configure<PasswordHasherOptions>(options =>
            {
                options.IterationCount = 500000;
            });

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
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}