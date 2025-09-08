using LearningLoop.GerenciamentoAlunosApp.Requests;
using LearningLoop.GerenciamentoAlunosApp.Responses;

namespace LearningLoop.GerenciamentoAlunosApp.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioResponse> CriarUsuarioAsync(UsuarioRequest request);
        Task<IEnumerable<UsuarioResponse>> ObterTodosUsuariosAsync();
        Task<UsuarioResponse> AtualizarUsuarioAsync(UsuarioRequest request);
        Task<UsuarioResponse> ObterUsuarioPorIdAsync(int id);
        Task<UsuarioResponse> DeletarUsuarioAsync(int id);
        Task<string> LoginAsync(UsuarioRequest request);
    }
}
