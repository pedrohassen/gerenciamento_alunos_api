using System.Text;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Enum;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Extensions;
using static LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils.Constants.Policies;

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
                    // Não lança aqui: isso roda pra QUALQUER requisição que traga um header
                    // Authorization, mesmo em endpoints anônimos (login/registrar, health
                    // check) — um token velho/inválido no navegador do cliente não pode
                    // quebrar rotas que nem exigem autenticação. Deixa a falha de
                    // autenticação seguir normalmente; quem decide se isso é um problema é
                    // a autorização (OnChallenge/OnForbidden abaixo), que só dispara pra
                    // endpoints que reamente exigem [Authorize].
                    OnAuthenticationFailed = context => Task.CompletedTask,
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
                options.AddPolicy(AdminOnly, policy =>
                    policy.RequireRole(PerfilEnum.ADMIN.GetDisplayName()));

                options.AddPolicy(UserOrAdmin, policy =>
                    policy.RequireRole(PerfilEnum.USER.GetDisplayName(), PerfilEnum.ADMIN.GetDisplayName()));
            });

            return services;
        }
    }
}
