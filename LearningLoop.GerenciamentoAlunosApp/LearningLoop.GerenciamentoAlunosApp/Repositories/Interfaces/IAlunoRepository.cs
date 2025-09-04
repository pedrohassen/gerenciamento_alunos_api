using LearningLoop.GerenciamentoAlunosApp.Arguments;
using LearningLoop.GerenciamentoAlunosApp.Models;

namespace LearningLoop.GerenciamentoAlunosApp.Repositories.Interfaces
{
    public interface IAlunoRepository
    {
        Task<AlunoModel> CriarAlunoAsync(AlunoArgument argument);
        Task<IEnumerable<AlunoModel>> ObterTodosAlunosAsync();
        Task<AlunoModel?> ObterAlunoPorIdAsync(int id);
    }
}
