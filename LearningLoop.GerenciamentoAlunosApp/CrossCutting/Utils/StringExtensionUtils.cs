namespace LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils
{
    public static class StringExtensionUtils
    {
        public static string[] ToStringArray(this string value)
        {
            return new string[1] { value };
        }
    }
}
