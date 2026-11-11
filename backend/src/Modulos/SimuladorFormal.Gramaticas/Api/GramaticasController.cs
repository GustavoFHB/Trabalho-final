using Microsoft.AspNetCore.Mvc;
using SimuladorFormal.Gramaticas.Contratos;
using SimuladorFormal.Gramaticas.Servicos;

namespace SimuladorFormal.Gramaticas.Api;

/// <summary>Módulo 3 — Gramáticas Livres de Contexto.</summary>
[ApiController]
[Route("api/gramaticas")]
public sealed class GramaticasController(DerivacaoService servico) : ControllerBase
{
    [HttpPost("derivar")]
    public ActionResult<DerivacaoResponse> Derivar([FromBody] DerivacaoRequest req) =>
        servico.Executar(req);
}
