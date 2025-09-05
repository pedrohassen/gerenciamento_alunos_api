using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Middlewares;

namespace LearningLoop.GerenciamentoAlunosApp.Extensions
{
    public static class ApiExtensions
    {
        public static IApplicationBuilder UseApiLayer(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("PermitirSwagger");

            return app;
        }

        public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllers();

            return endpoints;
        }
    }
}
