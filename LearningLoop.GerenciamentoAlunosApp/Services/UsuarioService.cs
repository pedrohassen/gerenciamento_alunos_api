using System.Net;
using LearningLoop.GerenciamentoAlunosApp.Arguments;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils;
using LearningLoop.GerenciamentoAlunosApp.Mapper;
using LearningLoop.GerenciamentoAlunosApp.Models;
using LearningLoop.GerenciamentoAlunosApp.Repositories.Interfaces;
using LearningLoop.GerenciamentoAlunosApp.Requests;
using LearningLoop.GerenciamentoAlunosApp.Responses;
using LearningLoop.GerenciamentoAlunosApp.Services.Interfaces;
using static LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils.Constants;
using static LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils.Constants.MensagemErro;

namespace LearningLoop.GerenciamentoAlunosApp.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IBCryptPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IObjectConverter _converter;

        public UsuarioService(
            IUsuarioRepository usuarioRepository,
            IBCryptPasswordHasher passwordHasher,
            IJwtService jwtService,
            IObjectConverter converter)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _converter = converter;
        }

        public async Task<UsuarioResponse> CriarUsuarioAsync(UsuarioRequest request)
        {
            ValidacoesUsuario.ValidarRequest(request, TipoValidacao.Registro);

            bool emailExistente = await _usuarioRepository.EmailExisteAsync(request.Email);
            if (emailExistente)
            {
                throw new UsuariosErrosException(
                    EmailJaCadastrado,
                    HttpStatusCode.BadRequest,
                    ErroValidacao
                );
            }

            UsuarioArgument argument = _converter.Map<UsuarioArgument>(request);
            argument.Senha = _passwordHasher.EncriptaSenha(request.Senha);

            UsuarioModel model = await _usuarioRepository.CriarUsuarioAsync(argument);

            return _converter.Map<UsuarioResponse>(model);
        }

        public async Task<IEnumerable<UsuarioResponse>> ObterTodosUsuariosAsync()
        {
            IEnumerable<UsuarioModel> usuarios = await _usuarioRepository.ObterTodosUsuariosAsync();

            if (!usuarios.Any())
            {
                throw new UsuariosErrosException(
                    NenhumUsuarioEncontrado,
                    HttpStatusCode.NotFound,
                    ConsultaVazia
                );
            }

            return usuarios.Select(u => _converter.Map<UsuarioResponse>(u));
        }

        public async Task<UsuarioResponse> ObterUsuarioPorIdAsync(int id)
        {
            ValidacoesUsuario.ValidarIdUsuario(id);

            UsuarioModel? usuario = await _usuarioRepository.ObterUsuarioPorIdAsync(id);

            if (usuario is null)
            {
                throw new UsuariosErrosException(
                    UsuarioNaoEncontrado,
                    HttpStatusCode.NotFound,
                    ErroValidacao
                );
            }

            return _converter.Map<UsuarioResponse>(usuario);
        }


        public async Task<UsuarioResponse> AtualizarUsuarioAsync(UsuarioRequest request)
        {
            ValidacoesUsuario.ValidarRequest(request, TipoValidacao.Atualizacao);

            UsuarioModel? existe = await _usuarioRepository.ObterUsuarioPorIdAsync(request.Id);
            if (existe is null)
            {
                throw new UsuariosErrosException(UsuarioNaoEncontrado, HttpStatusCode.NotFound, ErroValidacao);
            }

            UsuarioArgument argument = _converter.Map<UsuarioArgument>(request);
            argument.Senha = _passwordHasher.EncriptaSenha(request.Senha);

            UsuarioModel model = await _usuarioRepository.AtualizarUsuarioAsync(argument);

            return _converter.Map<UsuarioResponse>(model);
        }

        public async Task<UsuarioResponse> DeletarUsuarioAsync(int id)
        {
            ValidacoesUsuario.ValidarIdUsuario(id);

            UsuarioResponse? existe = await ObterUsuarioPorIdAsync(id);
            if (existe is null)
            {
                throw new UsuariosErrosException(
                    UsuarioNaoEncontrado,
                    HttpStatusCode.NotFound,
                    ErroValidacao
                );
            }

            UsuarioModel model = await _usuarioRepository.DeletarUsuarioAsync(id);
            return _converter.Map<UsuarioResponse>(model);
        }

        public async Task<string> LoginAsync(UsuarioRequest request)
        {
            ValidacoesUsuario.ValidarRequest(request, TipoValidacao.Login);

            UsuarioModel? usuario = await _usuarioRepository.ObterUsuarioPorEmailAsync(request.Email);

            if (usuario is null)
            {
                throw new UsuariosErrosException(
                    CredenciaisInvalidas,
                    HttpStatusCode.Unauthorized,
                    ErroValidacao
                );
            }

            bool senhaValida = _passwordHasher.VerificaSenha(request.Senha, usuario.Senha);
            if (!senhaValida)
            {
                throw new UsuariosErrosException(
                    CredenciaisInvalidas,
                    HttpStatusCode.Unauthorized,
                    ErroValidacao
                );
            }

            return _jwtService.GerarToken(usuario.Id, usuario.Email, usuario.Perfil);
        }
    }
}
