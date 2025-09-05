using LearningLoop.GerenciamentoAlunosApp.DI;
using LearningLoop.GerenciamentoAlunosApp.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace LearningLoop.GerenciamentoAlunosApp
{
    public class Program
    {
        protected Program()
        {
        }

        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            ConfigureServices(builder);

            WebApplication app = builder.Build();

            ConfigureApp(app);

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddDependencyInjection(builder.Configuration);
        }

        private static void ConfigureApp(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseInfra();
            app.UseApiLayer();
            app.MapApiEndpoints();
        }
    }
}
