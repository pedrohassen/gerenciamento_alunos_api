using LearningLoop.GerenciamentoAlunosApp.Mapper;

namespace LearningLoop.GerenciamentoAlunosApp.Tests.TestUtils
{
    public static class TestMapperFactory
    {
        public static IObjectConverter CriarObjectConverterReal()
        {
            AutoMapper.MapperConfiguration config = AutoMapperConfig.RegisterMappings();
            AutoMapper.IMapper mapper = new AutoMapper.Mapper(config);
            return new ObjectConverter(mapper);
        }
    }
}
