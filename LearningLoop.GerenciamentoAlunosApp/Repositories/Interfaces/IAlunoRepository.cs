using LearningLoop.GerenciamentoAlunosApp.Arguments;
using LearningLoop.GerenciamentoAlunosApp.Models;

namespace LearningLoop.GerenciamentoAlunosApp.Repositories.Interfaces
{
    public interface IAlunoRepository
    {
        Task<AlunoModel> CriarAlunoAsync(AlunoArgument argument);
        Task<AlunoModel> AtualizarAlunoAsync(AlunoArgument argument);
        Task<AlunoModel?> ObterAlunoPorIdAsync(int id);
        Task<IEnumerable<AlunoModel>> ObterAlunosAsync(FiltrosRequisicaoAlunoArgument filtros);
        Task<AlunoModel> DeletarAlunoAsync(int id);
        Task<bool> EmailExisteAsync(string email);
    }
}
