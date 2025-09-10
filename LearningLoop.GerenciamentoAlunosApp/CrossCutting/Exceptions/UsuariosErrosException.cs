using System.Net;
using System.Runtime.Serialization;

namespace LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions
{
    [Serializable]
    public class UsuariosErrosException : RegraNegocioException
    {
        public UsuariosErrosException(string[] mensagens, string titulo, string mensagemLog = null) : base(mensagens, titulo, mensagemLog) { }
        public UsuariosErrosException(string mensagem, string titulo, string mensagemLog = null) : base(mensagem, titulo, mensagemLog) { }
        public UsuariosErrosException(string mensagem) : base(mensagem, "Usuário", mensagem) { }
        public UsuariosErrosException(string[] mensagens, string titulo, string mensagemLog, Exception e = null) : base(mensagens, titulo, mensagemLog, e) { }
        public UsuariosErrosException(string mensagem, HttpStatusCode statusCode, string mensagemLog = null, Exception e = null) : base(mensagem, "Usuário", statusCode, mensagemLog, e) { }
        protected UsuariosErrosException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
