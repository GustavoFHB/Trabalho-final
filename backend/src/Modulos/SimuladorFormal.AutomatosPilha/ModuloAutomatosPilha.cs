using Microsoft.Extensions.DependencyInjection;
using SimuladorFormal.AutomatosPilha.Servicos;

namespace SimuladorFormal.AutomatosPilha;

/// <summary>Registra o Módulo 4 (serviço e controller) na aplicação.</summary>
public static class ModuloAutomatosPilha
{
    public static IServiceCollection AddModuloAutomatosPilha(this IServiceCollection services)
    {
        services.AddSingleton<SimuladorPilhaService>();
        services.AddControllers().AddApplicationPart(typeof(ModuloAutomatosPilha).Assembly);
        return services;
    }
}
