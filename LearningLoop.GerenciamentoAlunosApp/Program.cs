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
            // Containers compartilhados (ex.: Render free tier) às vezes já esgotaram o
            // limite de instâncias de inotify da máquina host — o watch automático de
            // appsettings.json (reloadOnChange, ligado por padrão) tenta criar mais uma e
            // crasha a app na inicialização com IOException antes de qualquer código nosso
            // rodar. Precisa vir ANTES de CreateBuilder, que é onde o watcher é criado.
            Environment.SetEnvironmentVariable("DOTNET_hostBuilder__reloadConfigOnChange", "false");

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
