using System.Net;
using System.Text.RegularExpressions;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Enum;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils.Constants;
using LearningLoop.GerenciamentoAlunosApp.Requests;
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
            string pattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{8,}$";
            if (!Regex.IsMatch(senha, pattern))
            {
                throw new UsuariosErrosException(SenhaFraca, HttpStatusCode.BadRequest, ErroValidacao);
            }
        }


        public static void ValidarPerfil(PerfilEnum idPerfil)
        {
            if (idPerfil != PerfilEnum.USER && idPerfil != PerfilEnum.ADMIN)
            {
                throw new UsuariosErrosException(PerfilInvalido, HttpStatusCode.BadRequest, ErroValidacao);
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
                    ValidarPerfil(request.Perfil);
                }
                ValidarDadosUsuario(request.Nome, NomeObrigatorio);
            }
        }
    }
}
