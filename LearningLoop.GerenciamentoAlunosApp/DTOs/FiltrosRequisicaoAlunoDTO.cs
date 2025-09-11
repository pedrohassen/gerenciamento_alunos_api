namespace LearningLoop.GerenciamentoAlunosApp.DTOs
{
    public class FiltrosRequisicaoAlunoDTO
    {
        public string? Nome { get; set; }
        public string? Curso { get; set; }
        public int? Pular { get; set; } = 0;
        public int? Quantidade { get; set; } = 50;
    }
}
