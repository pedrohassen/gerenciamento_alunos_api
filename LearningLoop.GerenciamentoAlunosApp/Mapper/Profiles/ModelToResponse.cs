using AutoMapper;
using LearningLoop.GerenciamentoAlunosApp.Models;
using LearningLoop.GerenciamentoAlunosApp.Responses;

namespace LearningLoop.GerenciamentoAlunosApp.Mapper.Profiles
{
    public class ModelToResponse : Profile
    {
        public ModelToResponse()
        {
            CreateMap<UsuarioModel, UsuarioResponse>();
        }
    }
}
