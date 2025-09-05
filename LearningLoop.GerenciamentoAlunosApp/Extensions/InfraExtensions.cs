namespace LearningLoop.GerenciamentoAlunosApp.Extensions
{
    public static class InfraExtensions
    {
        public static IApplicationBuilder UseInfra(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            return app;
        }
    }
}
