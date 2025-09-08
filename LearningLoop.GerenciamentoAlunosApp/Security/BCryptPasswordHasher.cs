using LearningLoop.GerenciamentoAlunosApp.Services.Interfaces;
using static BCrypt.Net.BCrypt;

namespace LearningLoop.GerenciamentoAlunosApp.Security
{
    public class BCryptPasswordHasher : IBCryptPasswordHasher
    {
        private const int WorkFactor = 12;

        public string EncriptaSenha(string password)
        {
            return HashPassword(password, WorkFactor);
        }

        public bool VerificaSenha(string password, string hashed)
        {
            return Verify(password, hashed);
        }
    }
}
