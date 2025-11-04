using FluentValidation;
using FluentValidation.AspNetCore;
using GestaoDeVendas.Application.IServices;
using GestaoDeVendas.Application.Services;
using GestaoDeVendas.Application.Validators.Cliente;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoDeVendas.Application
{
    public static class ApplicationModule
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddServices()
                    .AddFluentValidationAutoValidation()
                    .AddValidatorsFromAssemblyContaining<AddClienteRequestValidator>();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IProdutoService, ProdutoService>();
            services.AddScoped<IVendaService, VendaService>();

            return services;
        }
    }
}