using Microsoft.Extensions.DependencyInjection;
using SimuladorFormal.AutomatosFinitos.Servicos;

namespace SimuladorFormal.AutomatosFinitos;

/// <summary>Registra o Módulo 2 (serviço e controller) na aplicação.</summary>
public static class ModuloAutomatosFinitos
{
    public static IServiceCollection AddModuloAutomatosFinitos(this IServiceCollection services)
    {
        services.AddSingleton<SimuladorAutomatoService>();
        services.AddControllers().AddApplicationPart(typeof(ModuloAutomatosFinitos).Assembly);
        return services;
    }
}
