using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Enum;
using LearningLoop.GerenciamentoAlunosApp.Models;
using LearningLoop.GerenciamentoAlunosApp.Repositories.Interfaces;
using LearningLoop.GerenciamentoAlunosApp.Tests.TestUtils;
using Moq;

namespace LearningLoop.GerenciamentoAlunosApp.Tests.Controllers
{
    public class UsuarioControllerTests : IClassFixture<ApiFactory>
    {
        private readonly ApiFactory _factory;
        private readonly HttpClient _client;

        public UsuarioControllerTests(ApiFactory factory)
        {
            _factory = factory;
            _factory.UsuarioRepositoryMock.Reset();
            _factory.AlunoRepositoryMock.Reset();
            _client = factory.CreateClient();
        }

        private static UsuarioModel UsuarioComSenha(string senhaHash, int id = 1, PerfilEnum perfil = PerfilEnum.USER) => new()
        {
            Id = id,
            Nome = "Ana Silva",
            Email = "ana@teste.com",
            Senha = senhaHash,
            Perfil = perfil,
            Status = true,
            DataCriacao = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow,
        };

        private void AutenticarComo(int id, string email, PerfilEnum perfil)
        {
            string token = TokenHelper.GerarToken(_factory, id, email, perfil);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // ---------- Login ----------

        [Fact]
        public async Task Login_ComCredenciaisValidas_Retorna200ComToken()
        {
            string hash = TokenHelper.HashSenha(_factory, "Senha123!");
            _factory.UsuarioRepositoryMock
                .Setup(r => r.ObterUsuarioPorEmailAsync("ana@teste.com"))
                .ReturnsAsync(UsuarioComSenha(hash));

            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/Usuario/login", new
            {
                email = "ana@teste.com",
                senha = "Senha123!",
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            JsonDocument body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Assert.True(body.RootElement.TryGetProperty("token", out JsonElement tokenProp));
            Assert.False(string.IsNullOrWhiteSpace(tokenProp.GetString()));
        }

        [Fact]
        public async Task Login_ComSenhaErrada_Retorna401ComMensagem()
        {
            string hash = TokenHelper.HashSenha(_factory, "Senha123!");
            _factory.UsuarioRepositoryMock
                .Setup(r => r.ObterUsuarioPorEmailAsync("ana@teste.com"))
                .ReturnsAsync(UsuarioComSenha(hash));

            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/Usuario/login", new
            {
                email = "ana@teste.com",
                senha = "SenhaErrada1!",
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            string body = await response.Content.ReadAsStringAsync();
            Assert.Contains("Credenciais inválidas", body);
        }

        [Fact]
        public async Task Login_ComEmailInexistente_Retorna401()
        {
            _factory.UsuarioRepositoryMock
                .Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((UsuarioModel?)null);

            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/Usuario/login", new
            {
                email = "naoexiste@teste.com",
                senha = "Senha123!",
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // ---------- Registrar ----------

        [Fact]
        public async Task Registrar_ComDadosValidos_Retorna200SemSenha()
        {
            _factory.UsuarioRepositoryMock.Setup(r => r.EmailExisteAsync(It.IsAny<string>())).ReturnsAsync(false);
            _factory.UsuarioRepositoryMock
                .Setup(r => r.CriarUsuarioAsync(It.IsAny<Arguments.UsuarioArgument>()))
                .ReturnsAsync(UsuarioComSenha("hash-persistido"));

            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/Usuario/registrar", new
            {
                nome = "Ana Silva",
                email = "ana@teste.com",
                senha = "Senha123!",
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            JsonDocument body = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            Assert.True(body.RootElement.TryGetProperty("senha", out JsonElement senhaProp));
            Assert.Equal(JsonValueKind.Null, senhaProp.ValueKind);
        }

        [Fact]
        public async Task Registrar_ComSenhaFraca_Retorna400()
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync("/api/Usuario/registrar", new
            {
                nome = "Ana Silva",
                email = "ana@teste.com",
                senha = "fraca",
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // ---------- Autenticação / Autorização ----------

        [Fact]
        public async Task ObterTodos_SemToken_Retorna401()
        {
            HttpResponseMessage response = await _client.GetAsync("/api/Usuario");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ObterTodos_ComTokenDeUsuarioComum_Retorna403()
        {
            AutenticarComo(1, "ana@teste.com", PerfilEnum.USER);

            HttpResponseMessage response = await _client.GetAsync("/api/Usuario");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task ObterTodos_ComTokenDeAdmin_Retorna200ComLista()
        {
            AutenticarComo(1, "admin@teste.com", PerfilEnum.ADMIN);
            _factory.UsuarioRepositoryMock
                .Setup(r => r.ObterTodosUsuariosAsync())
                .ReturnsAsync(new[] { UsuarioComSenha("hash", 1), UsuarioComSenha("hash", 2) });

            HttpResponseMessage response = await _client.GetAsync("/api/Usuario");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ObterPorId_ComTokenDeUsuarioComum_Retorna200()
        {
            // UserOrAdmin: usuário comum também pode consultar por id.
            AutenticarComo(1, "ana@teste.com", PerfilEnum.USER);
            _factory.UsuarioRepositoryMock
                .Setup(r => r.ObterUsuarioPorIdAsync(1))
                .ReturnsAsync(UsuarioComSenha("hash", 1));

            HttpResponseMessage response = await _client.GetAsync("/api/Usuario/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ObterPorId_Inexistente_Retorna404()
        {
            AutenticarComo(1, "ana@teste.com", PerfilEnum.USER);
            _factory.UsuarioRepositoryMock
                .Setup(r => r.ObterUsuarioPorIdAsync(999))
                .ReturnsAsync((UsuarioModel?)null);

            HttpResponseMessage response = await _client.GetAsync("/api/Usuario/999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Deletar_ComTokenDeAdmin_Retorna200()
        {
            AutenticarComo(1, "admin@teste.com", PerfilEnum.ADMIN);
            _factory.UsuarioRepositoryMock
                .Setup(r => r.ObterUsuarioPorIdAsync(1))
                .ReturnsAsync(UsuarioComSenha("hash", 1));
            _factory.UsuarioRepositoryMock
                .Setup(r => r.DeletarUsuarioAsync(1))
                .ReturnsAsync(UsuarioComSenha("hash", 1));

            HttpResponseMessage response = await _client.DeleteAsync("/api/Usuario/1");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Deletar_ComTokenDeUsuarioComum_Retorna403()
        {
            AutenticarComo(1, "ana@teste.com", PerfilEnum.USER);

            HttpResponseMessage response = await _client.DeleteAsync("/api/Usuario/1");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
