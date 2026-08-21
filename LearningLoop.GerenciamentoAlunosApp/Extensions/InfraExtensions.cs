using Microsoft.AspNetCore.HttpOverrides;

namespace LearningLoop.GerenciamentoAlunosApp.Extensions
{
    public static class InfraExtensions
    {
        public static IApplicationBuilder UseInfra(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();

            return app;
        }
    }
}
