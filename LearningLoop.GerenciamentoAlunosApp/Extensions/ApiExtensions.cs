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

            // Health check do Render: por padrão ele bate em "/" pra saber se pode rotear
            // tráfego pra essa instância — sem essa rota, o deploy sobe mas o Render nunca
            // conecta o domínio público a ele (fica retornando 404 "no-server" na borda).
            endpoints.MapGet("/", () => Results.Ok(new { status = "healthy" }));

            return endpoints;
        }
    }
}
