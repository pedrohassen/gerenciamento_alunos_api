using LearningLoop.GerenciamentoAlunosApp.Models;
using LearningLoop.GerenciamentoAlunosApp.Requests;
using LearningLoop.GerenciamentoAlunosApp.Responses;

namespace LearningLoop.GerenciamentoAlunosApp.Services.Interfaces
{
    public interface IAlunoService
    {
        Task<AlunoResponse> CriarAlunoAsync(AlunoRequest request);
        Task<AlunoResponse> AtualizarAlunoAsync(AlunoRequest request);
        Task<AlunoResponse?> ObterAlunoPorIdAsync(int id);
        Task<IEnumerable<AlunoResponse>> ObterAlunosAsync(FiltrosRequisicaoAlunoModel filtros);
        Task<AlunoResponse> DeletarAlunoAsync(int id);
    }
}
