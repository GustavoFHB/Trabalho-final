using Microsoft.Extensions.DependencyInjection;
using SimuladorFormal.Linguagens.Servicos;

namespace SimuladorFormal.Linguagens;

/// <summary>Registra o Módulo 1 (serviço e controller) na aplicação.</summary>
public static class ModuloLinguagens
{
    public static IServiceCollection AddModuloLinguagens(this IServiceCollection services)
    {
        services.AddSingleton<AnaliseLinguagemService>();
        services.AddControllers().AddApplicationPart(typeof(ModuloLinguagens).Assembly);
        return services;
    }
}
