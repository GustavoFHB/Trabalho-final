using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace SimuladorFormal.Automatos;

[ApiController]
[Route("api/automatos")]
public sealed class AutomatosController : ControllerBase
{
    [HttpPost("finito")]
    public ResultadoAF Finito(RequisicaoAF req) => AutomatoFinito.Executar(req);

    [HttpPost("pilha")]
    public ResultadoAP Pilha(RequisicaoAP req) => AutomatoPilha.Executar(req);
}

public static class ModuloAutomatos
{
    public static IServiceCollection AddModuloAutomatos(this IServiceCollection services)
    {
        services.AddControllers().AddApplicationPart(typeof(AutomatosController).Assembly);
        return services;
    }
}
