using LearningLoop.GerenciamentoAlunosApp.Arguments;
using LearningLoop.GerenciamentoAlunosApp.Models;
using LearningLoop.GerenciamentoAlunosApp.Repositories.Base;
using LearningLoop.GerenciamentoAlunosApp.Repositories.Interfaces;

namespace LearningLoop.GerenciamentoAlunosApp.Repositories
{
    public class AlunoRepository : PostgresBaseRepository, IAlunoRepository
    {
        public AlunoRepository(ILogger<AlunoRepository> logger, IConfiguration configuration)
            : base(logger, configuration)
        {
        }
        public Task<AlunoModel> CriarAlunoAsync(AlunoArgument argument)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<AlunoModel>> ObterTodosAlunosAsync()
        {
            throw new NotImplementedException();
        }
        public Task<AlunoModel?> ObterAlunoPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
