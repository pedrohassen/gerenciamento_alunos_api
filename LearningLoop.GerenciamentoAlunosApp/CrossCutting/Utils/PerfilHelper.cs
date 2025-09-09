namespace LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils
{
    public static class PerfilHelper
    {
        public static string GetNomePerfil(int idPerfil) => idPerfil switch
        {
            1 => "USER",
            2 => "ADMIN",
            _ => "USER"
        };
    }
}
