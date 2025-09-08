using AutoMapper;
using LearningLoop.GerenciamentoAlunosApp.Mapper.Profiles;

namespace LearningLoop.GerenciamentoAlunosApp.Mapper
{
    public static class AutoMapperConfig
    {
        public static MapperConfiguration RegisterMappings(params Profile[] profiles)
        {
            try
            {
                MapperConfiguration config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfile(new ModelToResponse());
                    cfg.AddProfile(new RequestToArgument());
                });

                config.AssertConfigurationIsValid();

                return config;
            }
            catch (AutoMapperConfigurationException ex)
            {
                Console.WriteLine("Erro na configuração do AutoMapper:");
                Console.WriteLine(ex.Message);

                throw;
            }
        }
    }
}
