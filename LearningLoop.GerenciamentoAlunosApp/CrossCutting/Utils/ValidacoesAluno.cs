using System.Net;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils.Constants;
using LearningLoop.GerenciamentoAlunosApp.Requests;
using static LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils.Constants.MensagemErro;

namespace LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils
{
    public static class ValidacoesAluno
    {
        private static void ValidarNullRequest(AlunoRequest request, string mensagemErro)
        {
            if (request is null)
            {
                throw new AlunosErrosException(mensagemErro, HttpStatusCode.BadRequest, RequisicaoInvalida);
            }
        }

        private static void ValidarDadosAluno(string valor, string mensagemErro)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new AlunosErrosException(mensagemErro, HttpStatusCode.BadRequest, ErroValidacao);
            }
        }

        private static void ValidarDataNascimento(DateTime dataNascimento)
        {
            if (dataNascimento == default)
            {
                throw new AlunosErrosException(DataNascimentoInvalida, HttpStatusCode.BadRequest, ErroValidacao);
            }

            if (dataNascimento > DateTime.Now)
            {
                throw new AlunosErrosException(DataNascimentoFutura, HttpStatusCode.BadRequest, ErroValidacao);
            }
        }

        private static void ValidarId(int id)
        {
            if (id <= 0)
            {
                throw new AlunosErrosException(IdInvalido, HttpStatusCode.BadRequest, ErroValidacao);
            }
        }

        public static void ValidarRequest(AlunoRequest request, TipoValidacao tipo)
        {
            ValidarNullRequest(request, RequestNula);
            request.Email = ValidacoesUsuario.ValidarEmail(request.Email);
            ValidarDadosAluno(request.Nome, NomeObrigatorio);
            ValidarDadosAluno(request.Curso, CursoObrigatorio);
            ValidarDataNascimento(request.DataNascimento);

            if (tipo == TipoValidacao.Atualizacao)
            {
                ValidarId(request.Id);
            }
        }

        public static void ValidarFiltrosEPaginacao(FiltrosRequisicaoAlunoRequest filtros)
        {
            if (filtros.Pular < 0)
            {
                throw new AlunosErrosException(PularNaoPodeSerNegativo, HttpStatusCode.BadRequest, ErroValidacao);
            }

            if (filtros.Quantidade <= 0 || filtros.Quantidade > 100)
            {
                throw new AlunosErrosException(QuantidadeDeveEstarEntre1E100, HttpStatusCode.BadRequest, ErroValidacao);
            }

            if (filtros.Nome != null && filtros.Nome.Length > 150)
            {
                throw new AlunosErrosException(NomeNaoPodeSerMaiorQue150, HttpStatusCode.BadRequest, ErroValidacao);
            }

            if (filtros.Curso != null && filtros.Curso.Length > 200)
            {
                throw new AlunosErrosException(CursoNaoPodeSerMaiorQue200, HttpStatusCode.BadRequest, ErroValidacao);
            }
        }
    }
}
