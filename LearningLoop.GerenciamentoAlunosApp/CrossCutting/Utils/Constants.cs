namespace LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils
{
    public class Constants
    {
        public enum TipoValidacao
        {
            Registro,
            Login,
            Atualizacao
        }

        public static class MensagemErro
        {
            public const string RequestNula = "Favor preencher os dados.";
            public const string NomeObrigatorio = "Nome é obrigatório.";
            public const string EmailObrigatorio = "Email é obrigatório.";
            public const string EmailJaCadastrado = "Email já cadastrado.";
            public const string EmailInvalido = "Email inválido.";
            public const string SenhaObrigatoria = "Senha é obrigatória.";
            public const string SenhaFraca = "A senha deve ter pelo menos 8 caracteres, incluindo uma letra maiúscula, uma letra minúscula, um número e um caractere especial.";
            public const string PerfilInvalido = "Perfil inválido. Deve ser USER ou ADMIN.";
            public const string IdInvalido = "ID inválido.";
            public const string UsuarioNaoEncontrado = "Usuário não encontrado.";
            public const string NenhumUsuarioEncontrado = "Nenhum usuário encontrado.";
            public const string ConsultaVazia = "Consulta vazia.";
            public const string CredenciaisInvalidas = "Credenciais inválidas.";
            public const string ErroValidacao = "Erro de Validação";
            public const string ConflitoCadastro = "Conflito de Cadastro";
            public const string AcessoNegado = "Acesso Negado";
            public const string RecursoInexistente = "Recurso Inexistente";
            public const string RequisicaoInvalida = "Requisição Inválida.";
        }

        public static class Policies
        {
            public const string AdminOnly = "AdminOnly";
            public const string UserOrAdmin = "UserOrAdmin";
        }
    }
}
