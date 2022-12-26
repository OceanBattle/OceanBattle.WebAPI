using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OceanBattle.Jwks;
using OceanBattle.Jwks.Abstractions;
using OceanBattle.Jwks.DependencyInjection;
using OceanBattle.Jwt.Abstractions;

namespace OceanBattle.Jwt.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Enables JWT-bearer authentication using the default scheme <see cref="JwtBearerDefaults.AuthenticationScheme"/>.
        /// <para>
        /// JWT bearer authentication performs authentication by extracting and validating a JWT token from the <c>Authorization</c> request header.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="jwtOptions">Configuration section being bound.</param>
        /// <param name="configuration"><see cref="IConfiguration"/> of application.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddJwtBearer(
            this AuthenticationBuilder builder, 
            IConfigurationSection jwtOptions, 
            IConfiguration configuration
            )
        {
            builder.Services.AddJwks(configuration.GetSection(nameof(JwksOptions)));
            builder.Services.Configure<JwtOptions>(jwtOptions);
            builder.Services.TryAddTransient<IJwtFactory, JwtFactory>();

            builder.AddJwtBearer(options =>
            {
                JwtOptions jwtOptions = configuration.GetSection(nameof(JwtOptions))
                                                     .Get<JwtOptions>()!;

                IOptions<JwksOptions> jwksOptions = Options.Create(
                    configuration.GetSection(nameof(JwksOptions))
                                 .Get<JwksOptions>()!);

                IJwksFactory? keyFactory = (IJwksFactory?)Activator.CreateInstance(
                    typeof(JwksFactory),
                    LoggerFactory.Create(builder => { })
                                 .CreateLogger<JwksFactory>(),
                    jwksOptions);

                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAlgorithms = new List<string?> { jwtOptions.SecurityAlgorithm },
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKeys = keyFactory!.GetPublicKeys(),
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    RequireAudience = true,
                    RequireSignedTokens = true,
                    ValidateTokenReplay = true,
                    RequireExpirationTime = true
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hubs/chat")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.TryAddTransient<IJwtService, JwtService>();

            // TODO: Configure distributed cache.
            builder.Services.AddDistributedMemoryCache();
            
            builder.Services.TryAddTransient<JwtBlacklistMiddleware>();

            return builder;
        }
    }
}
