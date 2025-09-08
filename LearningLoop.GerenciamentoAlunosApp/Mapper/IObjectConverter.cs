namespace LearningLoop.GerenciamentoAlunosApp.Mapper
{
    public interface IObjectConverter
    {
        T Map<T>(object source);
    }
}
