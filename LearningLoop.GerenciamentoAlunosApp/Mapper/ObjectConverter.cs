using AutoMapper;

namespace LearningLoop.GerenciamentoAlunosApp.Mapper
{
    public class ObjectConverter : IObjectConverter
    {
        private readonly IMapper _mapper;

        public ObjectConverter(IMapper mapper)
        {
            _mapper = mapper;
        }

        public T Map<T>(object source)
        {
            T model = _mapper.Map<T>(source);

            return model;
        }
    }
}
