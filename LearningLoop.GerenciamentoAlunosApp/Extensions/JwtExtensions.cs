using System.Text;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.IdentityModel.Tokens;

namespace LearningLoop.GerenciamentoAlunosApp.Extensions
{
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            string key = configuration["Jwt:Secret"];
            if (string.IsNullOrEmpty(key))
            {
                throw JwtException.SecretNaoConfigurada();
            }

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        throw JwtException.TokenInvalido();
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        throw JwtException.TokenAusente();
                    },
                    OnForbidden = context =>
                    {
                        throw JwtException.AcessoNegado();
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("ADMIN"));

                options.AddPolicy("UserOrAdmin", policy =>
                    policy.RequireRole("USER", "ADMIN"));
            });

            return services;
        }
    }
}
