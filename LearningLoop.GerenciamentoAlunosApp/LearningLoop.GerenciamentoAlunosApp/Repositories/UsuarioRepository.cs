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
        public Task<UsuarioModel> CriarUsuarioAsync(UsuarioArgument argument)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<UsuarioModel>> ObterTodosUsuariosAsync()
        {
            throw new NotImplementedException();
        }
        public Task<UsuarioModel?> ObterUsuarioPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
