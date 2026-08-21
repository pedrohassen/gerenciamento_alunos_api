using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions;
using LearningLoop.GerenciamentoAlunosApp.Mapper;
using LearningLoop.GerenciamentoAlunosApp.Models;
using LearningLoop.GerenciamentoAlunosApp.Repositories.Interfaces;
using LearningLoop.GerenciamentoAlunosApp.Requests;
using LearningLoop.GerenciamentoAlunosApp.Responses;
using LearningLoop.GerenciamentoAlunosApp.Services;
using LearningLoop.GerenciamentoAlunosApp.Tests.TestUtils;
using Moq;

namespace LearningLoop.GerenciamentoAlunosApp.Tests.Services
{
    public class AlunoServiceTests
    {
        private readonly Mock<IAlunoRepository> _repositoryMock = new();
        private readonly IObjectConverter _objectConverter = TestMapperFactory.CriarObjectConverterReal();

        private AlunoService CriarService() => new(_repositoryMock.Object, _objectConverter);

        private static AlunoRequest RequestValido() => new()
        {
            Nome = "Carlos Souza",
            Email = "Carlos@Teste.com",
            Curso = "Medicina",
            DataNascimento = new DateTime(2001, 3, 15),
        };

        private static AlunoModel ModelPersistido(int id = 1, string nome = "Carlos Souza") => new()
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

        private static FiltrosRequisicaoAlunoRequest FiltrosValidos() => new();

        // ---------- CriarAlunoAsync ----------

        [Fact]
        public async Task CriarAlunoAsync_ComDadosValidos_RetornaResponse()
        {
            _repositoryMock.Setup(r => r.EmailExisteAsync(It.IsAny<string>())).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.CriarAlunoAsync(It.IsAny<Arguments.AlunoArgument>()))
                .ReturnsAsync(ModelPersistido());

            AlunoService service = CriarService();
            AlunoResponse response = await service.CriarAlunoAsync(RequestValido());

            Assert.Equal("Carlos Souza", response.Nome);
        }

        [Fact]
        public async Task CriarAlunoAsync_NormalizaEmail_Lowercase()
        {
            string? emailVerificado = null;
            _repositoryMock.Setup(r => r.EmailExisteAsync(It.IsAny<string>()))
                .Callback<string>(e => emailVerificado = e)
                .ReturnsAsync(false);
            _repositoryMock.Setup(r => r.CriarAlunoAsync(It.IsAny<Arguments.AlunoArgument>()))
                .ReturnsAsync(ModelPersistido());

            AlunoService service = CriarService();
            AlunoRequest request = RequestValido();
            request.Email = "Carlos@Teste.com";
            await service.CriarAlunoAsync(request);

            Assert.Equal("carlos@teste.com", emailVerificado);
        }

        [Fact]
        public async Task CriarAlunoAsync_ComEmailJaCadastrado_LancaExcecao()
        {
            _repositoryMock.Setup(r => r.EmailExisteAsync(It.IsAny<string>())).ReturnsAsync(true);

            AlunoService service = CriarService();
            AlunosErrosException ex = await Assert.ThrowsAsync<AlunosErrosException>(
                () => service.CriarAlunoAsync(RequestValido()));

            Assert.Equal(System.Net.HttpStatusCode.BadRequest, ex.StatusCode);
            _repositoryMock.Verify(r => r.CriarAlunoAsync(It.IsAny<Arguments.AlunoArgument>()), Times.Never);
        }

        [Fact]
        public async Task CriarAlunoAsync_ComNomeVazio_LancaExcecao()
        {
            AlunoService service = CriarService();
            AlunoRequest request = RequestValido();
            request.Nome = "";

            await Assert.ThrowsAsync<AlunosErrosException>(() => service.CriarAlunoAsync(request));
        }

        [Fact]
        public async Task CriarAlunoAsync_ComCursoVazio_LancaExcecao()
        {
            AlunoService service = CriarService();
            AlunoRequest request = RequestValido();
            request.Curso = "";

            await Assert.ThrowsAsync<AlunosErrosException>(() => service.CriarAlunoAsync(request));
        }

        [Fact]
        public async Task CriarAlunoAsync_ComEmailInvalido_LancaExcecaoDeAluno()
        {
            // Regressão: ValidacoesAluno tinha uma validação de e-mail própria que lançava
            // AlunosErrosException; antes disso reaproveitava ValidacoesUsuario.ValidarEmail
            // (UsuariosErrosException), que o AlunoController não captura — virava 500 em vez de 400.
            AlunoService service = CriarService();
            AlunoRequest request = RequestValido();
            request.Email = "nao-e-email";

            AlunosErrosException ex = await Assert.ThrowsAsync<AlunosErrosException>(() => service.CriarAlunoAsync(request));
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, ex.StatusCode);
        }

        [Fact]
        public async Task CriarAlunoAsync_ComDataNascimentoDefault_LancaExcecao()
        {
            AlunoService service = CriarService();
            AlunoRequest request = RequestValido();
            request.DataNascimento = default;

            await Assert.ThrowsAsync<AlunosErrosException>(() => service.CriarAlunoAsync(request));
        }

        [Fact]
        public async Task CriarAlunoAsync_ComDataNascimentoFutura_LancaExcecao()
        {
            AlunoService service = CriarService();
            AlunoRequest request = RequestValido();
            request.DataNascimento = DateTime.Now.AddDays(1);

            await Assert.ThrowsAsync<AlunosErrosException>(() => service.CriarAlunoAsync(request));
        }

        // ---------- AtualizarAlunoAsync ----------

        [Fact]
        public async Task AtualizarAlunoAsync_ComDadosValidos_RetornaAtualizado()
        {
            _repositoryMock.Setup(r => r.ObterAlunoPorIdAsync(1)).ReturnsAsync(ModelPersistido(1));
            _repositoryMock.Setup(r => r.AtualizarAlunoAsync(It.IsAny<Arguments.AlunoArgument>()))
                .ReturnsAsync(ModelPersistido(1, nome: "Carlos Atualizado"));

            AlunoRequest request = RequestValido();
            request.Id = 1;

            AlunoService service = CriarService();
            AlunoResponse response = await service.AtualizarAlunoAsync(request);

            Assert.Equal("Carlos Atualizado", response.Nome);
        }

        [Fact]
        public async Task AtualizarAlunoAsync_Inexistente_LancaExcecaoNotFound()
        {
            _repositoryMock.Setup(r => r.ObterAlunoPorIdAsync(999)).ReturnsAsync((AlunoModel?)null);

            AlunoRequest request = RequestValido();
            request.Id = 999;

            AlunoService service = CriarService();
            AlunosErrosException ex = await Assert.ThrowsAsync<AlunosErrosException>(
                () => service.AtualizarAlunoAsync(request));

            Assert.Equal(System.Net.HttpStatusCode.NotFound, ex.StatusCode);
            _repositoryMock.Verify(r => r.AtualizarAlunoAsync(It.IsAny<Arguments.AlunoArgument>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAlunoAsync_ComIdInvalido_LancaExcecao()
        {
            AlunoRequest request = RequestValido();
            request.Id = 0;

            AlunoService service = CriarService();
            await Assert.ThrowsAsync<AlunosErrosException>(() => service.AtualizarAlunoAsync(request));
        }

        // ---------- ObterAlunoPorIdAsync ----------

        [Fact]
        public async Task ObterAlunoPorIdAsync_Existente_RetornaResponse()
        {
            _repositoryMock.Setup(r => r.ObterAlunoPorIdAsync(1)).ReturnsAsync(ModelPersistido(1));

            AlunoService service = CriarService();
            AlunoResponse? response = await service.ObterAlunoPorIdAsync(1);

            Assert.NotNull(response);
        }

        [Fact]
        public async Task ObterAlunoPorIdAsync_Inexistente_RetornaNull()
        {
            // Diferente do UsuarioService: aqui não lança exceção, só retorna null
            // (é o Controller quem decide devolver 404).
            _repositoryMock.Setup(r => r.ObterAlunoPorIdAsync(999)).ReturnsAsync((AlunoModel?)null);

            AlunoService service = CriarService();
            AlunoResponse? response = await service.ObterAlunoPorIdAsync(999);

            Assert.Null(response);
        }

        // ---------- ObterAlunosAsync ----------

        [Fact]
        public async Task ObterAlunosAsync_ComResultados_RetornaLista()
        {
            _repositoryMock.Setup(r => r.ObterAlunosAsync(It.IsAny<Arguments.FiltrosRequisicaoAlunoArgument>()))
                .ReturnsAsync(new[] { ModelPersistido(1), ModelPersistido(2) });

            AlunoService service = CriarService();
            IEnumerable<AlunoResponse> resultado = await service.ObterAlunosAsync(FiltrosValidos());

            Assert.Equal(2, resultado.Count());
        }

        [Fact]
        public async Task ObterAlunosAsync_SemResultados_LancaExcecaoNotFound()
        {
            _repositoryMock.Setup(r => r.ObterAlunosAsync(It.IsAny<Arguments.FiltrosRequisicaoAlunoArgument>()))
                .ReturnsAsync(Array.Empty<AlunoModel>());

            AlunoService service = CriarService();
            AlunosErrosException ex = await Assert.ThrowsAsync<AlunosErrosException>(
                () => service.ObterAlunosAsync(FiltrosValidos()));

            Assert.Equal(System.Net.HttpStatusCode.NotFound, ex.StatusCode);
        }

        [Fact]
        public async Task ObterAlunosAsync_ComPularNegativo_LancaExcecao()
        {
            AlunoService service = CriarService();
            FiltrosRequisicaoAlunoRequest filtros = FiltrosValidos();
            filtros.Pular = -1;

            await Assert.ThrowsAsync<AlunosErrosException>(() => service.ObterAlunosAsync(filtros));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [InlineData(101)]
        public async Task ObterAlunosAsync_ComQuantidadeForaDoIntervalo_LancaExcecao(int quantidade)
        {
            AlunoService service = CriarService();
            FiltrosRequisicaoAlunoRequest filtros = FiltrosValidos();
            filtros.Quantidade = quantidade;

            await Assert.ThrowsAsync<AlunosErrosException>(() => service.ObterAlunosAsync(filtros));
        }

        [Fact]
        public async Task ObterAlunosAsync_ComNomeMuitoLongo_LancaExcecao()
        {
            AlunoService service = CriarService();
            FiltrosRequisicaoAlunoRequest filtros = FiltrosValidos();
            filtros.Nome = new string('a', 151);

            await Assert.ThrowsAsync<AlunosErrosException>(() => service.ObterAlunosAsync(filtros));
        }

        [Fact]
        public async Task ObterAlunosAsync_ComCursoMuitoLongo_LancaExcecao()
        {
            AlunoService service = CriarService();
            FiltrosRequisicaoAlunoRequest filtros = FiltrosValidos();
            filtros.Curso = new string('a', 201);

            await Assert.ThrowsAsync<AlunosErrosException>(() => service.ObterAlunosAsync(filtros));
        }

        // ---------- DeletarAlunoAsync ----------

        [Fact]
        public async Task DeletarAlunoAsync_Existente_Desativa()
        {
            _repositoryMock.Setup(r => r.ObterAlunoPorIdAsync(1)).ReturnsAsync(ModelPersistido(1));
            _repositoryMock.Setup(r => r.DeletarAlunoAsync(1)).ReturnsAsync(ModelPersistido(1));

            AlunoService service = CriarService();
            AlunoResponse response = await service.DeletarAlunoAsync(1);

            Assert.NotNull(response);
            _repositoryMock.Verify(r => r.DeletarAlunoAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeletarAlunoAsync_Inexistente_LancaExcecaoNotFound()
        {
            _repositoryMock.Setup(r => r.ObterAlunoPorIdAsync(999)).ReturnsAsync((AlunoModel?)null);

            AlunoService service = CriarService();
            AlunosErrosException ex = await Assert.ThrowsAsync<AlunosErrosException>(
                () => service.DeletarAlunoAsync(999));

            Assert.Equal(System.Net.HttpStatusCode.NotFound, ex.StatusCode);
            _repositoryMock.Verify(r => r.DeletarAlunoAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
