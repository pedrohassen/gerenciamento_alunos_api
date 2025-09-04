using LearningLoop.GerenciamentoAlunosApp.Extensions;

namespace LearningLoop.GerenciamentoAlunosApp.DI
{
    public static class RegistrationDependencyInjection
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRepositories();
            services.AddServices();
            services.AddAutoMapper(config => config.AddMaps(typeof(Program).Assembly));
            services.AddJwtAuthentication(configuration);
            services.AddCorsConfiguration();
            services.AddSwaggerConfiguration();
            return services;
        }
    }
}
