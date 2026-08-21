using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LearningLoop.GerenciamentoAlunosApp.Arguments;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Enum;
using LearningLoop.GerenciamentoAlunosApp.Models;
using LearningLoop.GerenciamentoAlunosApp.Tests.TestUtils;
using Moq;

namespace LearningLoop.GerenciamentoAlunosApp.Tests.Controllers
{
    public class AlunoControllerTests : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory _factory;
        private readonly HttpClient _client;

        public AlunoControllerTests(ApiFactory factory)
        {
            _factory = factory;
            _factory.UsuarioRepositoryMock.Reset();
            _factory.AlunoRepositoryMock.Reset();
            _client = factory.CreateClient();
        }

        private static AlunoModel AlunoPersistido(int id = 1, string nome = "Carlos Souza") => new()
        {
            Id = id,
            Nome = nome,
            Email = "carlos@teste.com",
            Curso = "Medicina",
            DataNascimento = new DateTime(2001, 3, 15),
            Status = true,
            DataCriacao = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow,
        };

        private void AutenticarComo(int id, string email, PerfilEnum perfil)
        {
            string token = TokenHelper.GerarToken(_factory, id, email, perfil);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // ---------- Criar ----------

        [Fact]
        public async Task Criar_SemToken_Retorna401()
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/Aluno", new
            {
                nome = "Carlos Souza",
                email = "carlos@teste.com",
                curso = "Medicina",
                dataNascimento = "2001-03-15",
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Criar_ComTokenDeUsuarioComum_Retorna403()
        {
            AutenticarComo(1, "ana@teste.com", PerfilEnum.USER);

            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/Aluno", new
            {
                nome = "Carlos Souza",
                email = "carlos@teste.com",
                curso = "Medicina",
                dataNascimento = "2001-03-15",
            });

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task Criar_ComTokenDeAdminEDadosValidos_Retorna200()
        {
            AutenticarComo(1, "admin@teste.com", PerfilEnum.ADMIN);
            _factory.AlunoRepositoryMock.Setup(r => r.EmailExisteAsync(It.IsAny<string>())).ReturnsAsync(false);
            _factory.AlunoRepositoryMock
                .Setup(r => r.CriarAlunoAsync(It.IsAny<AlunoArgument>()))
                .ReturnsAsync(AlunoPersistido());

            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/Aluno", new
            {
                nome = "Carlos Souza",
                email = "carlos@teste.com",
                curso = "Medicina",
                dataNascimento = "2001-03-15",
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Criar_ComEmailInvalido_Retorna400NaoInterno()
        {
            // Regressão do bug corrigido nessa mesma branch: antes, isso retornava 500.
            AutenticarComo(1, "admin@teste.com", PerfilEnum.ADMIN);

            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/Aluno", new
            {
                nome = "Carlos Souza",
                email = "nao-e-email",
                curso = "Medicina",
                dataNascimento = "2001-03-15",
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // ---------- Obter por id ----------

        [Fact]
        public async Task ObterPorId_Existente_Retorna200()
        {
            AutenticarComo(1, "admin@teste.com", PerfilEnum.ADMIN);
            _factory.AlunoRepositoryMock.Setup(r => r.ObterAlunoPorIdAsync(1)).ReturnsAsync(AlunoPersistido(1));

            HttpResponseMessage response = await _client.GetAsync("/api/Aluno/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ObterPorId_Inexistente_Retorna404()
        {
            AutenticarComo(1, "admin@teste.com", PerfilEnum.ADMIN);
            _factory.AlunoRepositoryMock.Setup(r => r.ObterAlunoPorIdAsync(999)).ReturnsAsync((AlunoModel?)null);

            HttpResponseMessage response = await _client.GetAsync("/api/Aluno/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // ---------- Listar ----------

        [Fact]
        public async Task Listar_ComResultados_Retorna200()
        {
            AutenticarComo(1, "admin@teste.com", PerfilEnum.ADMIN);
            _factory.AlunoRepositoryMock
                .Setup(r => r.ObterAlunosAsync(It.IsAny<FiltrosRequisicaoAlunoArgument>()))
                .ReturnsAsync(new[] { AlunoPersistido(1), AlunoPersistido(2) });

            HttpResponseMessage response = await _client.GetAsync("/api/Aluno");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Listar_SemResultados_Retorna404()
        {
            AutenticarComo(1, "admin@teste.com", PerfilEnum.ADMIN);
            _factory.AlunoRepositoryMock
                .Setup(r => r.ObterAlunosAsync(It.IsAny<FiltrosRequisicaoAlunoArgument>()))
                .ReturnsAsync(Array.Empty<AlunoModel>());

            HttpResponseMessage response = await _client.GetAsync("/api/Aluno");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Listar_ComQuantidadeForaDoIntervalo_Retorna400()
        {
            AutenticarComo(1, "admin@teste.com", PerfilEnum.ADMIN);

            HttpResponseMessage response = await _client.GetAsync("/api/Aluno?quantidade=0");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // ---------- Atualizar ----------

        [Fact]
        public async Task Atualizar_ComTokenDeAdminEDadosValidos_Retorna200()
        {
            AutenticarComo(1, "admin@teste.com", PerfilEnum.ADMIN);
            _factory.AlunoRepositoryMock.Setup(r => r.ObterAlunoPorIdAsync(1)).ReturnsAsync(AlunoPersistido(1));
            _factory.AlunoRepositoryMock
                .Setup(r => r.AtualizarAlunoAsync(It.IsAny<AlunoArgument>()))
                .ReturnsAsync(AlunoPersistido(1, "Carlos Atualizado"));

            HttpResponseMessage response = await _client.PutAsJsonAsync("/api/Aluno", new
            {
                id = 1,
                nome = "Carlos Atualizado",
                email = "carlos@teste.com",
                curso = "Medicina",
                dataNascimento = "2001-03-15",
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Atualizar_Inexistente_Retorna404()
        {
            AutenticarComo(1, "admin@teste.com", PerfilEnum.ADMIN);
            _factory.AlunoRepositoryMock.Setup(r => r.ObterAlunoPorIdAsync(999)).ReturnsAsync((AlunoModel?)null);

            HttpResponseMessage response = await _client.PutAsJsonAsync("/api/Aluno", new
            {
                id = 999,
                nome = "Carlos",
                email = "carlos@teste.com",
                curso = "Medicina",
                dataNascimento = "2001-03-15",
            });

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        // ---------- Deletar ----------

        [Fact]
        public async Task Deletar_ComTokenDeAdmin_Retorna200()
        {
            AutenticarComo(1, "admin@teste.com", PerfilEnum.ADMIN);
            _factory.AlunoRepositoryMock.Setup(r => r.ObterAlunoPorIdAsync(1)).ReturnsAsync(AlunoPersistido(1));
            _factory.AlunoRepositoryMock.Setup(r => r.DeletarAlunoAsync(1)).ReturnsAsync(AlunoPersistido(1));

            HttpResponseMessage response = await _client.DeleteAsync("/api/Aluno/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Deletar_ComTokenDeUsuarioComum_Retorna403()
        {
            AutenticarComo(1, "ana@teste.com", PerfilEnum.USER);

            HttpResponseMessage response = await _client.DeleteAsync("/api/Aluno/1");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
