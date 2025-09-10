using System.Threading.Tasks;
using LearningLoop.GerenciamentoAlunosApp.Arguments;
using LearningLoop.GerenciamentoAlunosApp.Models;

namespace LearningLoop.GerenciamentoAlunosApp.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<UsuarioModel> CriarUsuarioAsync(UsuarioArgument argument);
        Task<UsuarioModel?> ObterUsuarioPorIdAsync(int id);
        Task<bool> EmailExisteAsync(string email);
        Task<IEnumerable<UsuarioModel>> ObterTodosUsuariosAsync();
        Task<UsuarioModel> AtualizarUsuarioAsync(UsuarioArgument argument);
        Task<UsuarioModel> DeletarUsuarioAsync(int id);
        Task<UsuarioModel?> ObterUsuarioPorEmailAsync(string email);
    }
}
