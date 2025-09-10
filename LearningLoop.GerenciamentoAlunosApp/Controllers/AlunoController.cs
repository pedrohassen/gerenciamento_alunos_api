using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions;
using LearningLoop.GerenciamentoAlunosApp.Models;
using LearningLoop.GerenciamentoAlunosApp.Requests;
using LearningLoop.GerenciamentoAlunosApp.Responses;
using LearningLoop.GerenciamentoAlunosApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils.Constants.Policies;

namespace LearningLoop.GerenciamentoAlunosApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlunoController : ControllerBase
    {
        private readonly IAlunoService _alunoService;

        public AlunoController(IAlunoService alunoService)
        {
            _alunoService = alunoService;
        }

        [Authorize(Policy = AdminOnly)]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Cria um novo aluno.",
            Description = "Cadastro de novos alunos na aplicação.",
            OperationId = "CriarAluno")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CriarAlunoAsync([FromBody] AlunoRequest request)
        {
            try
            {
                AlunoResponse response = await _alunoService.CriarAlunoAsync(request);
                return Ok(response);
            }
            catch (AlunosErrosException ex)
            {
                return StatusCode((int)ex.StatusCode, new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao criar aluno.", detalhes = ex.Message });
            }
        }

        [Authorize(Policy = AdminOnly)]
        [HttpPut]
        [SwaggerOperation(
            Summary = "Atualiza os dados de um aluno existente.",
            Description = "Modificação das informações do aluno na aplicação.",
            OperationId = "AtualizarAluno")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AtualizarAlunoAsync([FromBody] AlunoRequest request)
        {
            try
            {
                AlunoResponse response = await _alunoService.AtualizarAlunoAsync(request);
                return Ok(response);
            }
            catch (AlunosErrosException ex)
            {
                return StatusCode((int)ex.StatusCode, new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao atualizar aluno.", detalhes = ex.Message });
            }
        }

        [Authorize(Policy = AdminOnly)]
        [HttpGet("{id}")]
        [SwaggerOperation(
            Summary = "Obter um aluno pelo ID.",
            Description = "Retorna os dados de um aluno específico pelo seu ID.",
            OperationId = "ObterAlunoPorId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterAlunoPorIdAsync([FromRoute] int id)
        {
            try
            {
                AlunoResponse? response = await _alunoService.ObterAlunoPorIdAsync(id);
                if (response == null)
                {
                    return NotFound(new { mensagem = "Aluno não encontrado." });
                }
                return Ok(response);
            }
            catch (AlunosErrosException ex)
            {
                return StatusCode((int)ex.StatusCode, new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao obter aluno.", detalhes = ex.Message });
            }
        }

        [Authorize(Policy = AdminOnly)]
        [HttpGet]
        [SwaggerOperation(
            Summary = "Obter lista de alunos.",
            Description = "Retorna uma lista de alunos, com filtros opcionais e paginação.",
            OperationId = "ObterAlunos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterAlunosAsync([FromQuery] FiltrosRequisicaoAlunoModel filtros)
        {
            try
            {
                IEnumerable<AlunoResponse> alunos = await _alunoService.ObterAlunosAsync(filtros);
                return Ok(alunos);
            }
            catch (AlunosErrosException ex)
            {
                return StatusCode((int)ex.StatusCode, new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao obter alunos.", detalhes = ex.Message });
            }
        }

        [Authorize(Policy = AdminOnly)]
        [HttpDelete("{id}")]
        [SwaggerOperation(
            Summary = "Desativa um aluno existente.",
            Description = "Marca o aluno como inativo sem removê-lo do banco.",
            OperationId = "DeletarAluno")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletarAlunoAsync([FromRoute] int id)
        {
            try
            {
                AlunoResponse response = await _alunoService.DeletarAlunoAsync(id);
                return Ok(response);
            }
            catch (AlunosErrosException ex)
            {
                return StatusCode((int)ex.StatusCode, new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao deletar aluno.", detalhes = ex.Message });
            }
        }
    }
}
