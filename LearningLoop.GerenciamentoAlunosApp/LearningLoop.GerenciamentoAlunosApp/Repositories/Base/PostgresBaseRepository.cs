using System.Data;
using Dapper;
using Npgsql;
using Polly;
using Polly.Retry;

namespace LearningLoop.GerenciamentoAlunosApp.Repositories.Base
{
    public abstract class PostgresBaseRepository
    {
        protected readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        protected RetryPolicy PostgresRetryPolicy { get; private set; }
        protected PostgresBaseRepository(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            SetRetryPolicy();
        }

        protected NpgsqlConnection CreateConnection()
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("PostgresConnection");
                return new NpgsqlConnection(connectionString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar conexão");
                throw;
            }
        }

        protected IDbConnection GetConnection()
        {
            IDbConnection connection = null;

            PostgresRetryPolicy.Execute(() =>
            {
                string connectionString = _configuration.GetConnectionString("FeedbackDatabase");
                connection = new NpgsqlConnection(connectionString);
                connection.Open();
            });

            return connection;
        }

        private void SetRetryPolicy()
        {
            PostgresRetryPolicy = Policy
                .Handle<NpgsqlException>()
                .Or<Exception>()
                .Retry(3, (exception, retryCount) =>
                {
                    _logger.LogWarning("Tentativa {RetryCount} falhou: {Message}", retryCount, exception.Message);
                });
        }

        protected virtual int Execute(string sql, IDbConnection connection, object param = null, CommandType? commandType = null)
        {
            int result = 0;
            PostgresRetryPolicy.Execute(() =>
            {
                result = connection.Execute(sql, param, commandType: commandType);
            });
            return result;
        }

        protected virtual IEnumerable<T> Query<T>(string sql, IDbConnection connection, object param = null, CommandType? commandType = null)
        {
            IEnumerable<T> result = null;
            PostgresRetryPolicy.Execute(() =>
            {
                result = connection.Query<T>(sql, param, commandType: commandType);
            });
            return result;
        }

        protected virtual async Task<IEnumerable<T>> QueryAsync<T>(string sql, IDbConnection connection, object param = null, CommandType? commandType = null)
        {
            IEnumerable<T> result = null;
            await PostgresRetryPolicy.Execute(async () =>
            {
                result = await connection.QueryAsync<T>(sql, param, commandType: commandType);
            });
            return result;
        }

        protected virtual T ExecuteScalar<T>(string sql, IDbConnection connection, object param = null, CommandType? commandType = null)
        {
            T result = default;
            PostgresRetryPolicy.Execute(() =>
            {
                result = connection.ExecuteScalar<T>(sql, param, commandType: commandType);
            });
            return result;
        }

        protected virtual T QueryFirstOrDefault<T>(string sql, IDbConnection connection, object param = null, CommandType? commandType = null)
        {
            T result = default;
            PostgresRetryPolicy.Execute(() =>
            {
                result = connection.QueryFirstOrDefault<T>(sql, param, commandType: commandType);
            });
            return result;
        }

        protected virtual T QuerySingleOrDefault<T>(string sql, IDbConnection connection, object param = null, CommandType? commandType = null)
        {
            T result = default;
            PostgresRetryPolicy.Execute(() =>
            {
                result = connection.QuerySingleOrDefault<T>(sql, param, commandType: commandType);
            });
            return result;
        }

        protected virtual int ExecuteSemRetry(string sql, IDbConnection connection, object param = null, CommandType? commandType = null)
        {
            int result = connection.Execute(sql, param, commandType: commandType);

            return result;
        }
    }
}
