namespace LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils
{
    public static class TratarDados
    {
        public static string TratarEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }
    }
}
