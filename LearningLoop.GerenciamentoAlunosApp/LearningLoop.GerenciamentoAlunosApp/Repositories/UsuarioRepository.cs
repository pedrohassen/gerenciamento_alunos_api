using LearningLoop.GerenciamentoAlunosApp.Arguments;
using LearningLoop.GerenciamentoAlunosApp.Models;
using LearningLoop.GerenciamentoAlunosApp.Repositories.Base;
using LearningLoop.GerenciamentoAlunosApp.Repositories.Interfaces;

namespace LearningLoop.GerenciamentoAlunosApp.Repositories
{
    public class UsuarioRepository : PostgresBaseRepository, IUsuarioRepository
    {
        public UsuarioRepository(ILogger<UsuarioRepository> logger, IConfiguration configuration)
            : base(logger, configuration)
        {
        }
    }
}
