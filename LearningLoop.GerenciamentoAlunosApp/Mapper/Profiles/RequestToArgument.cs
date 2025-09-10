using AutoMapper;
using LearningLoop.GerenciamentoAlunosApp.Arguments;
using LearningLoop.GerenciamentoAlunosApp.Requests;

namespace LearningLoop.GerenciamentoAlunosApp.Mapper.Profiles
{
    public class RequestToArgument : Profile
    {
        public RequestToArgument()
        {
            CreateMap<UsuarioRequest, UsuarioArgument>();

            CreateMap<AlunoRequest, AlunoArgument>();
        }
    }
}
