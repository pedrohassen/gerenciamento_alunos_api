namespace LearningLoop.GerenciamentoAlunosApp.Services.Interfaces
{
    public interface IBCryptPasswordHasher
    {
        string EncriptaSenha(string password);
        bool VerificaSenha(string password, string hashed);
    }
}
