using Microsoft.Extensions.DependencyInjection;
using SimuladorFormal.Turing.Servicos;

namespace SimuladorFormal.Turing;

/// <summary>Registra o Módulo 5 (serviço e controller) na aplicação.</summary>
public static class ModuloTuring
{
    public static IServiceCollection AddModuloTuring(this IServiceCollection services)
    {
        services.AddSingleton<SimuladorTuringService>();
        services.AddControllers().AddApplicationPart(typeof(ModuloTuring).Assembly);
        return services;
    }
}
