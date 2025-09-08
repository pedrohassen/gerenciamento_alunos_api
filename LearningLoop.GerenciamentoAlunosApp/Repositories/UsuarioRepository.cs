using System.Data;
using Dapper;
using LearningLoop.GerenciamentoAlunosApp.Arguments;
using LearningLoop.GerenciamentoAlunosApp.Models;
using LearningLoop.GerenciamentoAlunosApp.Repositories.Base;
using LearningLoop.GerenciamentoAlunosApp.Repositories.Interfaces;

namespace LearningLoop.GerenciamentoAlunosApp.Repositories
{
    public class UsuarioRepository : PostgresBaseRepository, IUsuarioRepository
    {
        private const string QueryCriarUsuario = @"
                                    INSERT INTO usuarios (nome, email, senha, perfil)
                                    VALUES (@Nome, @Email, @Senha, @Perfil::perfil_usuario)
                                    RETURNING id, nome, email, perfil, status, data_criacao, data_atualizacao;";

        private const string QueryEmailExiste = @"
                                    SELECT 1
                                    FROM usuarios
                                    WHERE email = @Email
                                    LIMIT 1;";

        private const string QueryObterTodosUsuarios = @"
                                    SELECT id, nome, email, perfil, status, data_criacao, data_atualizacao
                                    FROM usuarios
                                    ORDER BY nome;";

        private const string QueryAtualizarUsuario = @"
                                    UPDATE usuarios
                                    SET nome = @Nome,
                                        email = @Email,
                                        senha = @Senha,
                                        perfil = @Perfil::perfil_usuario,
                                        status = @Status,
                                        data_atualizacao = NOW()
                                    WHERE id = @Id
                                    RETURNING id, nome, email, perfil, status, data_criacao, data_atualizacao;";

        private const string QueryObterUsuarioPorId = @"
                                    SELECT id, nome, email, perfil, status, data_criacao, data_atualizacao
                                    FROM usuarios
                                    WHERE id = @Id;";

        private const string QueryDeletarUsuario = @"
                                    UPDATE usuarios
                                    SET status = false,
                                        data_atualizacao = NOW()
                                    WHERE id = @Id
                                    RETURNING id, nome, email, perfil, status, data_criacao, data_atualizacao;";

        private const string QueryObterUsuarioPorEmail = @"
                                    SELECT id, nome, email, senha, perfil, status, data_criacao, data_atualizacao
                                    FROM usuarios
                                    WHERE email = @Email AND status = true;";

        public UsuarioRepository(
            ILogger<UsuarioRepository> logger,
            IConfiguration configuration)
            : base(logger, configuration)
        {
        }

        public async Task<UsuarioModel> CriarUsuarioAsync(UsuarioArgument argument)
        {
            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QuerySingleAsync<UsuarioModel>(QueryCriarUsuario, argument);
            }
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            using (IDbConnection connection = CreateConnection())
            {
                return (await connection.QueryFirstOrDefaultAsync<int?>(QueryEmailExiste, new { Email = email })).HasValue;
            }
        }

        public async Task<IEnumerable<UsuarioModel>> ObterTodosUsuariosAsync()
        {
            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QueryAsync<UsuarioModel>(QueryObterTodosUsuarios);
            }
        }

        public async Task<UsuarioModel> AtualizarUsuarioAsync(UsuarioArgument argument)
        {
            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QuerySingleAsync<UsuarioModel>(QueryAtualizarUsuario, argument);
            }
        }

        public async Task<UsuarioModel?> ObterUsuarioPorIdAsync(int id)
        {
            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<UsuarioModel>(QueryObterUsuarioPorId, new { Id = id });
            }
        }

        public async Task<UsuarioModel> DeletarUsuarioAsync(int id)
        {
            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QuerySingleAsync<UsuarioModel>(QueryDeletarUsuario, new { Id = id });
            }
        }

        public async Task<UsuarioModel?> ObterUsuarioPorEmailAsync(string email)
        {
            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<UsuarioModel>(QueryObterUsuarioPorEmail, new { Email = email });
            }
        }
    }
}
