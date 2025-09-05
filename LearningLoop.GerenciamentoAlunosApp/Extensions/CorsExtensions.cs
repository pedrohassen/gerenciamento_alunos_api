using Microsoft.Extensions.DependencyInjection;

namespace LearningLoop.GerenciamentoAlunosApp.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("PermitirSwagger", policy =>
                {
                    policy.WithOrigins(
                        "https://localhost:7235",
                        "http://localhost:5233"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
