using LearningLoop.GerenciamentoAlunosApp.Arguments;
using LearningLoop.GerenciamentoAlunosApp.Models;

namespace LearningLoop.GerenciamentoAlunosApp.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<UsuarioModel> CriarUsuarioAsync(UsuarioArgument argument);
        Task<IEnumerable<UsuarioModel>> ObterTodosUsuariosAsync();
        Task<UsuarioModel?> ObterUsuarioPorIdAsync(int id);
    }
}
