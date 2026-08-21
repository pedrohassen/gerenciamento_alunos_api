using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Enum;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions;
using LearningLoop.GerenciamentoAlunosApp.Mapper;
using LearningLoop.GerenciamentoAlunosApp.Models;
using LearningLoop.GerenciamentoAlunosApp.Repositories.Interfaces;
using LearningLoop.GerenciamentoAlunosApp.Requests;
using LearningLoop.GerenciamentoAlunosApp.Responses;
using LearningLoop.GerenciamentoAlunosApp.Services;
using LearningLoop.GerenciamentoAlunosApp.Services.Interfaces;
using LearningLoop.GerenciamentoAlunosApp.Tests.TestUtils;
using Moq;

namespace LearningLoop.GerenciamentoAlunosApp.Tests.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _repositoryMock = new();
        private readonly Mock<IBCryptPasswordHasher> _passwordHasherMock = new();
        private readonly Mock<IJwtService> _jwtServiceMock = new();
        private readonly IObjectConverter _objectConverter = TestMapperFactory.CriarObjectConverterReal();

        private UsuarioService CriarService() =>
            new(_repositoryMock.Object, _passwordHasherMock.Object, _jwtServiceMock.Object, _objectConverter);

        private static UsuarioRequest RequestValido(string senha = "Senha123!") => new()
        {
            Nome = "Ana Silva",
            Email = "ANA@Teste.com",
            Senha = senha,
        };

        private static UsuarioModel ModelPersistido(int id = 1, string nome = "Ana Silva", string email = "ana@teste.com", PerfilEnum perfil = PerfilEnum.USER) => new()
        {
            Id = id,
            Nome = nome,
            Email = email,
            Senha = "hash-persistido",
            Perfil = perfil,
            Status = true,
            DataCriacao = DateTime.UtcNow,
            DataAtualizacao = DateTime.UtcNow,
        };

        // ---------- CriarUsuarioAsync ----------

        [Fact]
        public async Task CriarUsuarioAsync_ComDadosValidos_RetornaResponseComSenhaNula()
        {
            _repositoryMock.Setup(r => r.EmailExisteAsync(It.IsAny<string>())).ReturnsAsync(false);
            _passwordHasherMock.Setup(h => h.EncriptaSenha(It.IsAny<string>())).Returns("hash-gerado");
            _repositoryMock.Setup(r => r.CriarUsuarioAsync(It.IsAny<Arguments.UsuarioArgument>()))
                .ReturnsAsync(ModelPersistido());

            UsuarioService service = CriarService();
            UsuarioResponse response = await service.CriarUsuarioAsync(RequestValido());

            Assert.Null(response.Senha);
            Assert.Equal("Ana Silva", response.Nome);
        }

        [Fact]
        public async Task CriarUsuarioAsync_ForcaPerfilUsuario_MesmoQuandoRequestPedeAdmin()
        {
            // Regra de negócio: ninguém se autopromove a ADMIN no registro público.
            _repositoryMock.Setup(r => r.EmailExisteAsync(It.IsAny<string>())).ReturnsAsync(false);
            _passwordHasherMock.Setup(h => h.EncriptaSenha(It.IsAny<string>())).Returns("hash-gerado");

            Arguments.UsuarioArgument? argumentEnviado = null;
            _repositoryMock.Setup(r => r.CriarUsuarioAsync(It.IsAny<Arguments.UsuarioArgument>()))
                .Callback<Arguments.UsuarioArgument>(arg => argumentEnviado = arg)
                .ReturnsAsync(ModelPersistido());

            UsuarioRequest request = RequestValido();
            request.Perfil = PerfilEnum.ADMIN;

            UsuarioService service = CriarService();
            await service.CriarUsuarioAsync(request);

            Assert.Equal(PerfilEnum.USER, argumentEnviado!.Perfil);
        }

        [Fact]
        public async Task CriarUsuarioAsync_NormalizaEmail_Lowercase()
        {
            // ValidarEmail roda antes do TratarEmail e a regex já rejeita qualquer espaço,
            // então na prática só o ToLowerInvariant() do TratarEmail é alcançável aqui
            // (o Trim() nunca tem o que fazer, já que um e-mail com espaço já cai antes na validação).
            string? emailVerificado = null;
            _repositoryMock.Setup(r => r.EmailExisteAsync(It.IsAny<string>()))
                .Callback<string>(e => emailVerificado = e)
                .ReturnsAsync(false);
            _passwordHasherMock.Setup(h => h.EncriptaSenha(It.IsAny<string>())).Returns("hash-gerado");
            _repositoryMock.Setup(r => r.CriarUsuarioAsync(It.IsAny<Arguments.UsuarioArgument>()))
                .ReturnsAsync(ModelPersistido());

            UsuarioService service = CriarService();
            UsuarioRequest request = RequestValido();
            request.Email = "ANA@Teste.com";
            await service.CriarUsuarioAsync(request);

            Assert.Equal("ana@teste.com", emailVerificado);
        }

        [Fact]
        public async Task CriarUsuarioAsync_ComEmailJaCadastrado_LancaExcecao()
        {
            _repositoryMock.Setup(r => r.EmailExisteAsync(It.IsAny<string>())).ReturnsAsync(true);

            UsuarioService service = CriarService();
            UsuariosErrosException ex = await Assert.ThrowsAsync<UsuariosErrosException>(
                () => service.CriarUsuarioAsync(RequestValido()));

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, ex.StatusCode);
            _repositoryMock.Verify(r => r.CriarUsuarioAsync(It.IsAny<Arguments.UsuarioArgument>()), Times.Never);
        }

        [Fact]
        public async Task CriarUsuarioAsync_ComNomeVazio_LancaExcecao()
        {
            UsuarioService service = CriarService();
            UsuarioRequest request = RequestValido();
            request.Nome = "";

            await Assert.ThrowsAsync<UsuariosErrosException>(() => service.CriarUsuarioAsync(request));
        }

        [Fact]
        public async Task CriarUsuarioAsync_ComEmailInvalido_LancaExcecao()
        {
            UsuarioService service = CriarService();
            UsuarioRequest request = RequestValido();
            request.Email = "nao-e-email";

            await Assert.ThrowsAsync<UsuariosErrosException>(() => service.CriarUsuarioAsync(request));
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("apenasletrasminusculas")]
        [InlineData("SemNumeroOuEspecial1")]
        public async Task CriarUsuarioAsync_ComSenhaFraca_LancaExcecao(string senhaFraca)
        {
            // Regressão do achado: antes da correção, ValidarForcaSenha nunca era chamada
            // e qualquer uma dessas senhas passava direto pela validação do backend.
            UsuarioService service = CriarService();
            UsuarioRequest request = RequestValido(senhaFraca);

            UsuariosErrosException ex = await Assert.ThrowsAsync<UsuariosErrosException>(
                () => service.CriarUsuarioAsync(request));

            _repositoryMock.Verify(r => r.CriarUsuarioAsync(It.IsAny<Arguments.UsuarioArgument>()), Times.Never);
        }

        [Fact]
        public async Task CriarUsuarioAsync_ComSenhaForte_NaoLancaExcecaoDeSenha()
        {
            _repositoryMock.Setup(r => r.EmailExisteAsync(It.IsAny<string>())).ReturnsAsync(false);
            _passwordHasherMock.Setup(h => h.EncriptaSenha(It.IsAny<string>())).Returns("hash-gerado");
            _repositoryMock.Setup(r => r.CriarUsuarioAsync(It.IsAny<Arguments.UsuarioArgument>()))
                .ReturnsAsync(ModelPersistido());

            UsuarioService service = CriarService();
            UsuarioResponse response = await service.CriarUsuarioAsync(RequestValido("Senha123!"));

            Assert.NotNull(response);
        }

        // ---------- ObterTodosUsuariosAsync ----------

        [Fact]
        public async Task ObterTodosUsuariosAsync_ComUsuarios_RetornaListaComSenhaNula()
        {
            _repositoryMock.Setup(r => r.ObterTodosUsuariosAsync())
                .ReturnsAsync(new[] { ModelPersistido(1), ModelPersistido(2) });

            UsuarioService service = CriarService();
            IEnumerable<UsuarioResponse> resultado = await service.ObterTodosUsuariosAsync();

            Assert.Equal(2, resultado.Count());
            Assert.All(resultado, r => Assert.Null(r.Senha));
        }

        [Fact]
        public async Task ObterTodosUsuariosAsync_SemUsuarios_LancaExcecaoNotFound()
        {
            _repositoryMock.Setup(r => r.ObterTodosUsuariosAsync()).ReturnsAsync(Array.Empty<UsuarioModel>());

            UsuarioService service = CriarService();
            UsuariosErrosException ex = await Assert.ThrowsAsync<UsuariosErrosException>(
                () => service.ObterTodosUsuariosAsync());

            Assert.Equal(System.Net.HttpStatusCode.NotFound, ex.StatusCode);
        }

        // ---------- ObterUsuarioPorIdAsync ----------

        [Fact]
        public async Task ObterUsuarioPorIdAsync_Existente_RetornaComSenhaNula()
        {
            _repositoryMock.Setup(r => r.ObterUsuarioPorIdAsync(1)).ReturnsAsync(ModelPersistido(1));

            UsuarioService service = CriarService();
            UsuarioResponse response = await service.ObterUsuarioPorIdAsync(1);

            Assert.Null(response.Senha);
        }

        [Fact]
        public async Task ObterUsuarioPorIdAsync_Inexistente_LancaExcecaoNotFound()
        {
            _repositoryMock.Setup(r => r.ObterUsuarioPorIdAsync(999)).ReturnsAsync((UsuarioModel?)null);

            UsuarioService service = CriarService();
            UsuariosErrosException ex = await Assert.ThrowsAsync<UsuariosErrosException>(
                () => service.ObterUsuarioPorIdAsync(999));

            Assert.Equal(System.Net.HttpStatusCode.NotFound, ex.StatusCode);
        }

        [Fact]
        public async Task ObterUsuarioPorIdAsync_ComIdInvalido_LancaExcecao()
        {
            UsuarioService service = CriarService();
            await Assert.ThrowsAsync<UsuariosErrosException>(() => service.ObterUsuarioPorIdAsync(0));
        }

        // ---------- AtualizarUsuarioAsync ----------

        [Fact]
        public async Task AtualizarUsuarioAsync_ComDadosValidos_RetornaAtualizado()
        {
            _repositoryMock.Setup(r => r.ObterUsuarioPorIdAsync(1)).ReturnsAsync(ModelPersistido(1));
            _passwordHasherMock.Setup(h => h.EncriptaSenha(It.IsAny<string>())).Returns("hash-novo");
            _repositoryMock.Setup(r => r.AtualizarUsuarioAsync(It.IsAny<Arguments.UsuarioArgument>()))
                .ReturnsAsync(ModelPersistido(1, nome: "Ana Atualizada"));

            UsuarioRequest request = RequestValido();
            request.Id = 1;
            request.Perfil = PerfilEnum.USER;

            UsuarioService service = CriarService();
            UsuarioResponse response = await service.AtualizarUsuarioAsync(request);

            Assert.Equal("Ana Atualizada", response.Nome);
        }

        [Fact]
        public async Task AtualizarUsuarioAsync_ComIdInvalido_LancaExcecao()
        {
            UsuarioRequest request = RequestValido();
            request.Id = 0;
            request.Perfil = PerfilEnum.USER;

            UsuarioService service = CriarService();
            await Assert.ThrowsAsync<UsuariosErrosException>(() => service.AtualizarUsuarioAsync(request));
        }

        [Fact]
        public async Task AtualizarUsuarioAsync_ComPerfilForaDoEnumValido_LancaExcecao()
        {
            UsuarioRequest request = RequestValido();
            request.Id = 1;
            request.Perfil = (PerfilEnum)0;

            UsuarioService service = CriarService();
            await Assert.ThrowsAsync<UsuariosErrosException>(() => service.AtualizarUsuarioAsync(request));
        }

        [Fact]
        public async Task AtualizarUsuarioAsync_ComSenhaFraca_LancaExcecao()
        {
            UsuarioRequest request = RequestValido("fraca");
            request.Id = 1;
            request.Perfil = PerfilEnum.USER;

            UsuarioService service = CriarService();
            await Assert.ThrowsAsync<UsuariosErrosException>(() => service.AtualizarUsuarioAsync(request));
        }

        // ---------- DeletarUsuarioAsync ----------

        [Fact]
        public async Task DeletarUsuarioAsync_Existente_Desativa()
        {
            _repositoryMock.Setup(r => r.ObterUsuarioPorIdAsync(1)).ReturnsAsync(ModelPersistido(1));
            _repositoryMock.Setup(r => r.DeletarUsuarioAsync(1)).ReturnsAsync(ModelPersistido(1));

            UsuarioService service = CriarService();
            UsuarioResponse response = await service.DeletarUsuarioAsync(1);

            Assert.NotNull(response);
            _repositoryMock.Verify(r => r.DeletarUsuarioAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeletarUsuarioAsync_Inexistente_LancaExcecaoNotFound()
        {
            _repositoryMock.Setup(r => r.ObterUsuarioPorIdAsync(999)).ReturnsAsync((UsuarioModel?)null);

            UsuarioService service = CriarService();
            UsuariosErrosException ex = await Assert.ThrowsAsync<UsuariosErrosException>(
                () => service.DeletarUsuarioAsync(999));

            Assert.Equal(System.Net.HttpStatusCode.NotFound, ex.StatusCode);
            _repositoryMock.Verify(r => r.DeletarUsuarioAsync(It.IsAny<int>()), Times.Never);
        }

        // ---------- LoginAsync ----------

        [Fact]
        public async Task LoginAsync_ComCredenciaisValidas_RetornaToken()
        {
            UsuarioModel usuario = ModelPersistido(1, perfil: PerfilEnum.ADMIN);
            _repositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            _passwordHasherMock.Setup(h => h.VerificaSenha("Senha123!", usuario.Senha)).Returns(true);
            _jwtServiceMock.Setup(j => j.GerarToken(usuario.Id, usuario.Email, usuario.Perfil)).Returns("token-jwt");

            UsuarioService service = CriarService();
            string token = await service.LoginAsync(RequestValido());

            Assert.Equal("token-jwt", token);
        }

        [Fact]
        public async Task LoginAsync_ComEmailInexistente_LancaCredenciaisInvalidas()
        {
            _repositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync((UsuarioModel?)null);

            UsuarioService service = CriarService();
            UsuariosErrosException ex = await Assert.ThrowsAsync<UsuariosErrosException>(
                () => service.LoginAsync(RequestValido()));

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, ex.StatusCode);
        }

        [Fact]
        public async Task LoginAsync_ComSenhaErrada_LancaCredenciaisInvalidas()
        {
            UsuarioModel usuario = ModelPersistido(1);
            _repositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            _passwordHasherMock.Setup(h => h.VerificaSenha(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            UsuarioService service = CriarService();
            UsuariosErrosException ex = await Assert.ThrowsAsync<UsuariosErrosException>(
                () => service.LoginAsync(RequestValido()));

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, ex.StatusCode);
            _jwtServiceMock.Verify(j => j.GerarToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<PerfilEnum>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_NaoExigeSenhaForte_LoginComSenhaAntigaFracaFunciona()
        {
            // Login não deve chamar ValidarForcaSenha — só valida existência do valor.
            // Isso importa pra não travar usuários que já existiam antes dessa correção.
            UsuarioModel usuario = ModelPersistido(1);
            _repositoryMock.Setup(r => r.ObterUsuarioPorEmailAsync(It.IsAny<string>())).ReturnsAsync(usuario);
            _passwordHasherMock.Setup(h => h.VerificaSenha("123", usuario.Senha)).Returns(true);
            _jwtServiceMock.Setup(j => j.GerarToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<PerfilEnum>())).Returns("token-jwt");

            UsuarioService service = CriarService();
            string token = await service.LoginAsync(RequestValido("123"));

            Assert.Equal("token-jwt", token);
        }
    }
}
