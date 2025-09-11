using System.Net;
using LearningLoop.GerenciamentoAlunosApp.Arguments;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Exceptions;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils;
using LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils.Constants;
using LearningLoop.GerenciamentoAlunosApp.Mapper;
using LearningLoop.GerenciamentoAlunosApp.Models;
using LearningLoop.GerenciamentoAlunosApp.Repositories.Interfaces;
using LearningLoop.GerenciamentoAlunosApp.Requests;
using LearningLoop.GerenciamentoAlunosApp.Responses;
using LearningLoop.GerenciamentoAlunosApp.Services.Interfaces;
using static LearningLoop.GerenciamentoAlunosApp.CrossCutting.Utils.Constants.MensagemErro;

namespace LearningLoop.GerenciamentoAlunosApp.Services
{
    public class AlunoService : IAlunoService
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly IObjectConverter _objectConverter;

        public AlunoService(IAlunoRepository repository, IObjectConverter converter)
        {
            _alunoRepository = repository;
            _objectConverter = converter;
        }

        public async Task<AlunoResponse> CriarAlunoAsync(AlunoRequest request)
        {
            ValidacoesAluno.ValidarRequest(request, TipoValidacao.Registro);

            bool emailExistente = await _alunoRepository.EmailExisteAsync(request.Email);
            if (emailExistente)
            {
                throw new AlunosErrosException(EmailJaCadastrado, HttpStatusCode.BadRequest, ErroValidacao);
            }

            AlunoArgument argument = _objectConverter.Map<AlunoArgument>(request);
            AlunoModel model = await _alunoRepository.CriarAlunoAsync(argument);

            AlunoResponse response = _objectConverter.Map<AlunoResponse>(model);
            return response;
        }

        public async Task<AlunoResponse> AtualizarAlunoAsync(AlunoRequest request)
        {
            ValidacoesAluno.ValidarRequest(request, TipoValidacao.Atualizacao);

            AlunoModel? alunoExistente = await _alunoRepository.ObterAlunoPorIdAsync(request.Id);
            if (alunoExistente == null)
            {
                throw new AlunosErrosException(AlunoNaoEncontrado, HttpStatusCode.NotFound, ErroValidacao);
            }

            AlunoArgument argument = _objectConverter.Map<AlunoArgument>(request);
            AlunoModel model = await _alunoRepository.AtualizarAlunoAsync(argument);

            AlunoResponse response = _objectConverter.Map<AlunoResponse>(model);
            return response;
        }

        public async Task<AlunoResponse?> ObterAlunoPorIdAsync(int id)
        {
            AlunoModel? model = await _alunoRepository.ObterAlunoPorIdAsync(id);
            if (model == null)
            {
                return null;
            }

            AlunoResponse response = _objectConverter.Map<AlunoResponse>(model);
            return response;
        }

        public async Task<IEnumerable<AlunoResponse>> ObterAlunosAsync(FiltrosRequisicaoAlunoRequest filtros)
        {
            ValidacoesAluno.ValidarFiltrosEPaginacao(filtros);

            FiltrosRequisicaoAlunoArgument filtrosArgument = _objectConverter.Map<FiltrosRequisicaoAlunoArgument>(filtros);

            IEnumerable<AlunoModel> alunos = await _alunoRepository.ObterAlunosAsync(filtrosArgument);

            if (!alunos.Any())
            {
                throw new AlunosErrosException(NenhumAlunoEncontrado, HttpStatusCode.NotFound, ConsultaVazia);
            }

            IEnumerable<AlunoResponse> response = alunos.Select(a => _objectConverter.Map<AlunoResponse>(a));
            return response;
        }

        public async Task<AlunoResponse> DeletarAlunoAsync(int id)
        {
            AlunoModel? alunoExistente = await _alunoRepository.ObterAlunoPorIdAsync(id);
            if (alunoExistente == null)
            {
                throw new AlunosErrosException(AlunoNaoEncontrado, HttpStatusCode.NotFound, ErroValidacao);
            }

            AlunoModel model = await _alunoRepository.DeletarAlunoAsync(id);
            AlunoResponse response = _objectConverter.Map<AlunoResponse>(model);
            return response;
        }
    }
}
