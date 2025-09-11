namespace LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils
{
    public static class TratativaEmail
    {
        public static string NormalizarEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }
    }
}
