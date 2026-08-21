using LearningLoop.GerenciamentoAlunosApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace LearningLoop.GerenciamentoAlunosApp.Tests.TestUtils
{
    // Sobe a API real em memória (roteamento, model binding, [Authorize], middleware de
    // exceção) trocando só os repositórios por mocks via DI — não precisa de Postgres rodando.
    public class ApiFactory : WebApplicationFactory<Program>
    {
        public Mock<IUsuarioRepository> UsuarioRepositoryMock { get; } = new();
        public Mock<IAlunoRepository> AlunoRepositoryMock { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IUsuarioRepository>();
                services.AddScoped(_ => UsuarioRepositoryMock.Object);

                services.RemoveAll<IAlunoRepository>();
                services.AddScoped(_ => AlunoRepositoryMock.Object);
            });
        }
    }
}
