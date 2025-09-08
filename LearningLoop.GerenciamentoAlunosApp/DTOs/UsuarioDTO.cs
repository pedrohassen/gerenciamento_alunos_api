using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Enum;

namespace LearningLoop.GerenciamentoAlunosApp.DTOs
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Perfil { get; set; }
        public bool Status { get; set; }
        public DateOnly DataCriacao { get; set; }
        public DateOnly DataAtualizacao { get; set; }
    }
}
