using System.Net;
using System.Runtime.Serialization;

namespace LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions
{
    public class AlunosErrosException : RegraNegocioException
    {
        public AlunosErrosException(string[] mensagens, string titulo, string mensagemLog = null) : base(mensagens, titulo, mensagemLog) { }
        public AlunosErrosException(string mensagem, string titulo, string mensagemLog = null) : base(mensagem, titulo, mensagemLog) { }
        public AlunosErrosException(string mensagem) : base(mensagem, "Aluno", mensagem) { }
        public AlunosErrosException(string[] mensagens, string titulo, string mensagemLog, Exception e = null) : base(mensagens, titulo, mensagemLog, e) { }
        public AlunosErrosException(string mensagem, HttpStatusCode statusCode, string mensagemLog = null, Exception e = null) : base(mensagem, "Aluno", statusCode, mensagemLog, e) { }
        protected AlunosErrosException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
