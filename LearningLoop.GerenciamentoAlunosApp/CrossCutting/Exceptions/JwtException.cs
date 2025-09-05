using System.Net;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions.Base;

namespace LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions
{
    public class JwtException : BaseException
    {
        public JwtException(
            string[] mensagens,
            string titulo = "Erro de autenticação JWT",
            HttpStatusCode statusCode = HttpStatusCode.Unauthorized,
            string mensagemLog = null,
            Exception innerException = null)
            : base(mensagens, titulo, statusCode, mensagemLog, innerException)
        {
        }

        public static JwtException SecretNaoConfigurada()
        {
            string[] mensagens = new string[] { "JWT Secret não configurada." };
            return new JwtException(mensagens, statusCode: HttpStatusCode.InternalServerError);
        }

        public static JwtException TokenInvalido()
        {
            string[] mensagens = new string[] { "Token inválido ou expirado." };
            return new JwtException(mensagens, statusCode: HttpStatusCode.Unauthorized);
        }

        public static JwtException TokenAusente()
        {
            string[] mensagens = new string[] { "Token ausente." };
            return new JwtException(mensagens, statusCode: HttpStatusCode.Unauthorized);
        }
    }
}
