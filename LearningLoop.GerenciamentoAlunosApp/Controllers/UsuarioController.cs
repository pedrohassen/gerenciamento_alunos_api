using System.Net;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions;
using LearningLoop.GerenciamentoAlunosApp.Requests;
using LearningLoop.GerenciamentoAlunosApp.Responses;
using LearningLoop.GerenciamentoAlunosApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LearningLoop.GerenciamentoAlunosApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("registrar")]
        [SwaggerOperation(
            Summary = "Registra um novo usuário no sistema.",
            Description = "Cadastro de novos usuários na aplicação.",
            OperationId = "RegistroUsuario")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CriarUsuarioAsync([FromBody] UsuarioRequest request)
        {
            try
            {
                UsuarioResponse response = await _usuarioService.CriarUsuarioAsync(request);
                return Ok(response);
            }
            catch (UsuariosErrosException ex)
            {
                return StatusCode((int)ex.StatusCode, new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao criar usuário.", detalhes = ex.Message });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Visualizar todos os usuários do sistema.",
            Description = "Obter lista de todos os usuários na aplicação.",
            OperationId = "ObterTodosUsuarios")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterTodosUsuariosAsync()
        {
            try
            {
                IEnumerable<UsuarioResponse> usuarios = await _usuarioService.ObterTodosUsuariosAsync();
                return Ok(usuarios);
            }
            catch (UsuariosErrosException ex)
            {
                return StatusCode((int)ex.StatusCode, new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao obter usuários.", detalhes = ex.Message });
            }
        }

        [Authorize(Policy = "UserOrAdmin")]
        [HttpPut]
        [SwaggerOperation(
            Summary = "Atualiza os dados de um usuário existente.",
            Description = "Modificação das informações do usuário na aplicação.",
            OperationId = "AtualizarUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AtualizarUsuarioAsync([FromBody] UsuarioRequest request)
        {
            try
            {
                UsuarioResponse usuario = await _usuarioService.AtualizarUsuarioAsync(request);
                return Ok(usuario);
            }
            catch (UsuariosErrosException ex)
            {
                return StatusCode((int)ex.StatusCode, new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao atualizar usuário.", detalhes = ex.Message });
            }
        }

        [Authorize(Policy = "UserOrAdmin")]
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Obter um usuário pelo ID.",
            Description = "Retorna os dados de um usuário específico pelo seu ID.",
            OperationId = "ObterUsuarioPorId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterUsuarioPorIdAsync([FromRoute] int id)
        {
            try
            {
                UsuarioResponse usuario = await _usuarioService.ObterUsuarioPorIdAsync(id);
                return Ok(usuario);
            }
            catch (UsuariosErrosException ex)
            {
                return StatusCode((int)ex.StatusCode, new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao obter usuário.", detalhes = ex.Message });
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Desativa um usuário existente.",
            Description = "Marca o usuário como inativo sem removê-lo do banco.",
            OperationId = "DeletarUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletarUsuarioAsync(int id)
        {
            try
            {
                UsuarioResponse usuario = await _usuarioService.DeletarUsuarioAsync(id);
                return Ok(usuario);
            }
            catch (UsuariosErrosException ex)
            {
                return StatusCode((int)ex.StatusCode, new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao deletar usuário.", detalhes = ex.Message });
            }
        }

        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Login de um usuário no sistema.",
            Description = "Valida as credenciais do usuário e retorna um token JWT.",
            OperationId = "LoginUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginAsync([FromBody] UsuarioRequest request)
        {
            try
            {
                string token = await _usuarioService.LoginAsync(request);
                return Ok(new { Token = token });
            }
            catch (UsuariosErrosException ex)
            {
                return StatusCode((int)ex.StatusCode, new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao autenticar usuário.", detalhes = ex.Message });
            }
        }
    }
}
