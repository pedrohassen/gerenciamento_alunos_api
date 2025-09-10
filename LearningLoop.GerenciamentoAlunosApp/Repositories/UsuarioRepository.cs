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
        public UsuarioRepository(
            ILogger<UsuarioRepository> logger,
            IConfiguration configuration)
            : base(logger, configuration)
        {
        }

        public async Task<UsuarioModel> CriarUsuarioAsync(UsuarioArgument argument)
        {
            const string QueryCriarUsuario = @"
                                INSERT INTO usuarios (nome, email, senha, id_perfil)
                                VALUES (@Nome, @Email, @Senha, @Perfil)
                                RETURNING id, nome, email, senha, id_perfil AS Perfil, status, data_criacao AS DataCriacao, data_atualizacao AS DataAtualizacao;";

            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QuerySingleAsync<UsuarioModel>(QueryCriarUsuario, argument);
            }
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            const string QueryEmailExiste = @"
                                SELECT 1
                                FROM usuarios
                                WHERE email = @Email
                                LIMIT 1;";

            using (IDbConnection connection = CreateConnection())
            {
                return (await connection.QueryFirstOrDefaultAsync<int?>(QueryEmailExiste, new { Email = email })).HasValue;
            }
        }

        public async Task<IEnumerable<UsuarioModel>> ObterTodosUsuariosAsync()
        {
            const string QueryObterTodosUsuarios = @"
                                SELECT u.id, u.nome, u.email, u.senha, u.id_perfil AS Perfil, u.status, u.data_criacao AS DataCriacao, u.data_atualizacao AS DataAtualizacao
                                FROM usuarios u
                                WHERE u.status = true
                                ORDER BY u.nome;";

            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QueryAsync<UsuarioModel>(QueryObterTodosUsuarios);
            }
        }

        public async Task<UsuarioModel> AtualizarUsuarioAsync(UsuarioArgument argument)
        {
            const string QueryAtualizarUsuario = @"
                                UPDATE usuarios
                                SET nome = @Nome,
                                    email = @Email,
                                    senha = @Senha,
                                    id_perfil = @Perfil,
                                    status = @Status,
                                    data_atualizacao = NOW()
                                WHERE id = @Id
                                RETURNING id, nome, email, senha, id_perfil AS Perfil, status, data_criacao AS DataCriacao, data_atualizacao AS DataAtualizacao;";

            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QuerySingleAsync<UsuarioModel>(QueryAtualizarUsuario, argument);
            }
        }

        public async Task<UsuarioModel?> ObterUsuarioPorIdAsync(int id)
        {
            const string QueryObterUsuarioPorId = @"
                                SELECT u.id, u.nome, u.email, u.senha, u.id_perfil AS Perfil, u.status, u.data_criacao AS DataCriacao, u.data_atualizacao AS DataAtualizacao
                                FROM usuarios u
                                WHERE u.id = @Id AND u.status = true;";

            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<UsuarioModel>(QueryObterUsuarioPorId, new { Id = id });
            }
        }

        public async Task<UsuarioModel> DeletarUsuarioAsync(int id)
        {
            const string QueryDeletarUsuario = @"
                                UPDATE usuarios
                                SET status = false, data_atualizacao = NOW()
                                WHERE id = @Id
                                RETURNING id, nome, email, senha, id_perfil AS Perfil, status, data_criacao AS DataCriacao, data_atualizacao AS DataAtualizacao;";

            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QuerySingleAsync<UsuarioModel>(QueryDeletarUsuario, new { Id = id });
            }
        }

        public async Task<UsuarioModel?> ObterUsuarioPorEmailAsync(string email)
        {
            const string QueryObterUsuarioPorEmail = @"
                                SELECT u.id, u.nome, u.email, u.senha, u.id_perfil AS Perfil, u.status, u.data_criacao AS DataCriacao, u.data_atualizacao AS DataAtualizacao
                                FROM usuarios u
                                WHERE u.email = @Email AND u.status = true;";

            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<UsuarioModel>(QueryObterUsuarioPorEmail, new { Email = email });
            }
        }
    }
}
