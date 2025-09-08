using AutoMapper;
using LearningLoop.GerenciamentoAlunosApp.Mapper;

namespace LearningLoop.GerenciamentoAlunosApp.Extensions
{
    public static class AutoMapperExtensions
    {
        public static IServiceCollection AddAutoMapperConfig(this IServiceCollection services)
        {
            MapperConfiguration mapperConfiguration = AutoMapperConfig.RegisterMappings();

            mapperConfiguration.AssertConfigurationIsValid();

            IMapper mapperInstance = new AutoMapper.Mapper(mapperConfiguration);

            services.AddSingleton<IMapper>(mapperInstance);

            return services;
        }
    }
}
