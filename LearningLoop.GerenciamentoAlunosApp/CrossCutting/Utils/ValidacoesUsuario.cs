using System.Net;
using System.Text.RegularExpressions;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions;
using LearningLoop.GerenciamentoAlunosApp.Requests;
using static LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils.Constants;
using static LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils.Constants.MensagemErro;

namespace LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils
{
    public static class ValidacoesUsuario
    {
        public static void ValidarEmail(string email)
        {
            ValidarDadosUsuario(email, EmailObrigatorio);

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase))
            {
                throw new UsuariosErrosException(EmailInvalido, HttpStatusCode.BadRequest, ErroValidacao);
            }
        }

        public static void ValidarForcaSenha(string senha)
        {
            if (senha.Length < 8 ||
                !Regex.IsMatch(senha, @"[A-Z]") ||
                !Regex.IsMatch(senha, @"[a-z]") ||
                !Regex.IsMatch(senha, @"[0-9]") ||
                !Regex.IsMatch(senha, @"[\W_]"))
            {
                throw new UsuariosErrosException(SenhaFraca, HttpStatusCode.BadRequest, ErroValidacao);
            }
        }

        public static void ValidarPerfil(string perfil)
        {
            if (perfil != "USER" && perfil != "ADMIN")
            {
                throw new UsuariosErrosException(
                    PerfilInvalido,
                    HttpStatusCode.BadRequest,
                    ErroValidacao
                );
            }
        }

        public static void ValidarIdUsuario(int id)
        {
            if (id <= 0)
            {
                throw new UsuariosErrosException(IdInvalido, HttpStatusCode.BadRequest, ErroValidacao);
            }
        }

        private static void ValidarDadosUsuario(string valor, string mensagemErro)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new UsuariosErrosException(mensagemErro, HttpStatusCode.BadRequest, ErroValidacao);
            }
        }

        private static void ValidarNullRequest(UsuarioRequest request, string mensagemErro)
        {
            if (request is null)
            {
                throw new UsuariosErrosException(mensagemErro, HttpStatusCode.BadRequest, RequisicaoInvalida);
            }
        }

        public static void ValidarRequest(UsuarioRequest request, TipoValidacao tipo)
        {
            ValidarNullRequest(request, RequestNula);
            ValidarEmail(request.Email);
            ValidarDadosUsuario(request.Senha, SenhaObrigatoria);

            if (tipo == TipoValidacao.Registro || tipo == TipoValidacao.Atualizacao)
            {
                if (tipo == TipoValidacao.Atualizacao)
                {
                    ValidarIdUsuario(request.Id);
                }
                ValidarDadosUsuario(request.Nome, NomeObrigatorio);
                ValidarPerfil(request.Perfil);
            }
        }
    }
}
