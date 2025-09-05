using System.Net;
using System.Runtime.Serialization;

namespace LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions.Base
{
    [Serializable]
    public class BaseException : Exception
    {
        public string[] Mensagens { get; set; }
        public string Titulo { get; set; }
        public string MensagemLog { get; set; }
        public HttpStatusCode StatusCode { get; set; }

        public BaseException(
            string[] mensagens,
            string titulo,
            HttpStatusCode statusCode,
            string mensagemLog = null,
            Exception innerException = null)
            : base(mensagens.FirstOrDefault(), innerException)
        {
            Mensagens = mensagens ?? new[] { "Ocorreu um erro inesperado." };
            Titulo = titulo;
            StatusCode = statusCode;
            MensagemLog = string.IsNullOrWhiteSpace(mensagemLog) ? Message : mensagemLog;
        }

        protected BaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
