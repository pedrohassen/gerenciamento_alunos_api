using System.Data;
using Dapper;
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

        public async Task<AlunoModel> CriarAlunoAsync(AlunoArgument argument)
        {
            const string QueryCriarAluno = @"
                INSERT INTO alunos (nome, email, data_nascimento, curso)
                VALUES (@Nome, @Email, @DataNascimento, @Curso)
                RETURNING id, nome, email, data_nascimento AS DataNascimento, curso, status, data_criacao AS DataCriacao, data_atualizacao AS DataAtualizacao;";

            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QuerySingleAsync<AlunoModel>(QueryCriarAluno, argument);
            }
        }

        public async Task<AlunoModel> AtualizarAlunoAsync(AlunoArgument argument)
        {
            const string QueryAtualizarAluno = @"
                UPDATE alunos
                SET nome = @Nome,
                    email = @Email,
                    data_nascimento = @DataNascimento,
                    curso = @Curso,
                    status = @Status,
                    data_atualizacao = NOW()
                WHERE id = @Id
                RETURNING id, nome, email, data_nascimento AS DataNascimento, curso, status, data_criacao AS DataCriacao, data_atualizacao AS DataAtualizacao;";

            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QuerySingleAsync<AlunoModel>(QueryAtualizarAluno, argument);
            }
        }

        public async Task<AlunoModel?> ObterAlunoPorIdAsync(int id)
        {
            const string QueryObterAlunoPorId = @"
                SELECT id, nome, email, data_nascimento AS DataNascimento, curso, status, data_criacao AS DataCriacao, data_atualizacao AS DataAtualizacao
                FROM alunos
                WHERE id = @Id AND status = true;";

            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<AlunoModel>(QueryObterAlunoPorId, new { Id = id });
            }
        }

        public async Task<IEnumerable<AlunoModel>> ObterAlunosAsync(FiltrosRequisicaoAlunoModel filtros)
        {
            const string query = @"
                SELECT id, nome, email, data_nascimento AS DataNascimento, curso, status, data_criacao AS DataCriacao, data_atualizacao AS DataAtualizacao
                FROM alunos
                WHERE status = true
                /**filtros**/
                ORDER BY nome
                OFFSET @Pular LIMIT @Quantidade;";

            string QueryObterAlunosComFiltros = query.Replace("/**filtros**/",
                (filtros.Nome != null ? "AND nome ILIKE '%' || @Nome || '%'" : "") +
                (filtros.Curso != null ? " AND curso ILIKE '%' || @Curso || '%'" : ""));

            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QueryAsync<AlunoModel>(QueryObterAlunosComFiltros, new { filtros.Nome, filtros.Curso, filtros.Pular, filtros.Quantidade });
            }
        }

        public async Task<AlunoModel> DeletarAlunoAsync(int id)
        {
            const string QueryDeletarAluno = @"
                UPDATE alunos
                SET status = false, data_atualizacao = NOW()
                WHERE id = @Id
                RETURNING id, nome, email, data_nascimento AS DataNascimento, curso, status, data_criacao AS DataCriacao, data_atualizacao AS DataAtualizacao;";

            using (IDbConnection connection = CreateConnection())
            {
                return await connection.QuerySingleAsync<AlunoModel>(QueryDeletarAluno, new { Id = id });
            }
        }

        public async Task<bool> EmailExisteAsync(string email)
        {
            const string QueryEmailExiste = "SELECT 1 FROM alunos WHERE email = @Email LIMIT 1;";
            using (IDbConnection connection = CreateConnection())
            {
                return (await connection.QueryFirstOrDefaultAsync<int?>(QueryEmailExiste, new { Email = email })).HasValue;
            }
        }
    }
}
