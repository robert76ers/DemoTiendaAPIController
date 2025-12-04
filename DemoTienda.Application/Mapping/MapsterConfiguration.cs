using DemoTienda.Application.DTOs.Request;
using DemoTienda.Application.DTOs.Response;
using DemoTienda.Domain.Entites;
using Mapster;

namespace DemoTienda.Application.Mapping
{
    public static class MapsterConfiguration
    {
        public static void Register(TypeAdapterConfig config) 
        {
            config.NewConfig<Categoria, CategoriaResponseDTO>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Nombre, src => src.Nombre);

            config.NewConfig<CreateCategoriaRequestDTO, Categoria>()
                .Map(dest => dest.Id, src => 0)
                .Map(dest => dest.Nombre, src => src.Nombre);

            config.NewConfig<UpdateCategoriaRequestDTO, Categoria>()
                .Map(dest => dest.Nombre, src => src.Nombre);
        }
    }
}
