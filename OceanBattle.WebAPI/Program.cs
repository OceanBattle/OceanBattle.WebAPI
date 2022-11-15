using OceanBattle.DataModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OceanBattle.Data;
using OceanBattle.Rsa.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OceanBattle.Rsa;
using OceanBattle.Rsa.Abstractions;

namespace OceanBattle
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string? connectionString = builder.Configuration.GetConnectionString("OceanBattle.Database");

            // Add services to the container.

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddIdentityCore<User>()
                            .AddEntityFrameworkStores<DbContext>()
                            .AddDefaultTokenProviders();

            builder.Services.AddRsaKeyFactory();
            builder.Services.AddControllers();

            builder.Services.AddAuthentication().AddJwtBearer(options =>
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
                    RequireExpirationTime = true,
                };
            });

            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

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