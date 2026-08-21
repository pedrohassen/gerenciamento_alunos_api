using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Enum;
using LearningLoop.GerenciamentoAlunosApp.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace LearningLoop.GerenciamentoAlunosApp.Tests.TestUtils
{
    public static class TokenHelper
    {
        public static string GerarToken(ApiFactory factory, int id, string email, PerfilEnum perfil)
        {
            using IServiceScope scope = factory.Services.CreateScope();
            IJwtService jwtService = scope.ServiceProvider.GetRequiredService<IJwtService>();
            return jwtService.GerarToken(id, email, perfil);
        }

        public static string HashSenha(ApiFactory factory, string senha)
        {
            using IServiceScope scope = factory.Services.CreateScope();
            IBCryptPasswordHasher hasher = scope.ServiceProvider.GetRequiredService<IBCryptPasswordHasher>();
            return hasher.EncriptaSenha(senha);
        }
    }
}
