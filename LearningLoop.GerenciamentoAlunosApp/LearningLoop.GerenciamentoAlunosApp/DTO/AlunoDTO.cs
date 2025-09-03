namespace LearningLoop.GerenciamentoAlunosApp.DTO
{
    public class AlunoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Curso { get; set; }
        public bool Status { get; set; }
        public DateOnly DataCriacao { get; set; }
        public DateOnly DataAtualizacao { get; set; }
    }
}
