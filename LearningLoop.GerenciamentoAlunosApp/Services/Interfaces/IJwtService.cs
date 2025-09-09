namespace LearningLoop.GerenciamentoAlunosApp.Services.Interfaces
{
    public interface IJwtService
    {
        string GerarToken(int id, string email, string perfilNome);
    }
}
