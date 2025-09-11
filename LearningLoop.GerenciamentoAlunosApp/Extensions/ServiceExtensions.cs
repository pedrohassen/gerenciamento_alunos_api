using LearningLoop.GerenciamentoAlunosApp.Mapper;
using LearningLoop.GerenciamentoAlunosApp.Security;
using LearningLoop.GerenciamentoAlunosApp.Services;
using LearningLoop.GerenciamentoAlunosApp.Services.Interfaces;

namespace LearningLoop.GerenciamentoAlunosApp.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IAlunoService, AlunoService>();
            services.AddSingleton<IJwtService, JwtService>();
            services.AddSingleton<IBCryptPasswordHasher,BCryptPasswordHasher>();
            services.AddSingleton<IObjectConverter, ObjectConverter>();
            return services;
        }
    }
}
