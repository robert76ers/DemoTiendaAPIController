using DemoTienda.Application.Interfaces;
using DemoTienda.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoTienda.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            return services;
        }
    }
}
