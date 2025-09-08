using System.Net;
using System.Text.Json;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions.Base;

namespace LearningLoop.GerenciamentoAlunosApp.CrossCutting.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BaseException bex)
            {
                await HandleBaseExceptionAsync(context, bex);
            }
            catch (Exception ex)
            {
                await HandleUnknownExceptionAsync(context, ex);
            }
        }

        private async Task HandleBaseExceptionAsync(HttpContext context, BaseException exception)
        {
            _logger.LogWarning(exception, "Erro conhecido ocorrido: {MensagemLog}", exception.MensagemLog);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)exception.StatusCode;

            object response = new
            {
                titulo = exception.Titulo,
                mensagens = exception.Mensagens
            };

            string json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }

        private async Task HandleUnknownExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Erro inesperado ocorrido.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            object response = new
            {
                titulo = "Erro Interno",
                mensagens = new[] { "Ocorreu um erro inesperado. Tente novamente mais tarde." }
            };

            string json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
