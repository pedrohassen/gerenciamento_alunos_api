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

        public ApiFactory()
        {
            // appsettings.json não tem mais Jwt:Secret (segredo real, não pode ir pro repo —
            // ver README/TAREFAS 2.8). `Program.cs` usa `Main` clássico, não minimal API
            // top-level, então `ConfigureServices(builder)` (que registra o AddJwtAuthentication)
            // já roda e já lê a config ANTES do hook `ConfigureWebHost` do WebApplicationFactory
            // ter chance de aplicar qualquer `ConfigureAppConfiguration` — setar via variável de
            // ambiente é a única forma confiável de injetar o segredo a tempo, porque env vars
            // são lidas direto por `WebApplication.CreateBuilder(args)`, antes de qualquer código
            // do Program rodar. Mesmo valor pros dois lados (assinatura em `JwtService` e
            // validação em `JwtExtensions`), então tokens gerados via `TokenHelper` validam.
            Environment.SetEnvironmentVariable("Jwt__Secret", "chave-fake-somente-para-testes-de-integracao-0123456789");
        }

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
