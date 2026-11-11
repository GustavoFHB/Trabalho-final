using Microsoft.Extensions.DependencyInjection;
using SimuladorFormal.Gramaticas.Servicos;

namespace SimuladorFormal.Gramaticas;

/// <summary>Registra o Módulo 3 (serviço e controller) na aplicação.</summary>
public static class ModuloGramaticas
{
    public static IServiceCollection AddModuloGramaticas(this IServiceCollection services)
    {
        services.AddSingleton<DerivacaoService>();
        services.AddControllers().AddApplicationPart(typeof(ModuloGramaticas).Assembly);
        return services;
    }
}
