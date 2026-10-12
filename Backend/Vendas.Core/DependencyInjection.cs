using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Vendas.Abstractions.Interfaces.DbSession;
using Vendas.Abstractions.Interfaces.Repositories;
using Vendas.Abstractions.Interfaces.Services;
using Vendas.Business.Services;
using Vendas.DB.Repositories;
using Vendas.DB.Sessions;
using Vendas.Model.Configs;

namespace Vendas.Core;

public static class DependencyInjection
{
    public static IServiceCollection ResolveDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BancoConfig>(configuration.GetSection("BancoConfig"));
        services.AddScoped(sp => sp.GetRequiredService<IOptions<BancoConfig>>().Value);

        services.AddScoped<IDbSession, DbSession>();

        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IVendaRepository, VendaRepository>();

        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<IVendaService, VendaService>();

        return services;
    }
}
