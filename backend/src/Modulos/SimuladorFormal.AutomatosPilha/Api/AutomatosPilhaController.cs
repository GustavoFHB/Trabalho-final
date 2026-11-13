using Microsoft.AspNetCore.Mvc;
using SimuladorFormal.AutomatosPilha.Contratos;
using SimuladorFormal.AutomatosPilha.Execucao;
using SimuladorFormal.AutomatosPilha.Servicos;

namespace SimuladorFormal.AutomatosPilha.Api;

/// <summary>Módulo 4 — Autômato com pilha.</summary>
[ApiController]
[Route("api/pilha")]
public sealed class AutomatosPilhaController(SimuladorPilhaService servico) : ControllerBase
{
    /// <summary>Autômatos prontos: aⁿbⁿ, palíndromos pares e parênteses balanceados.</summary>
    [HttpGet("exemplos")]
    public ActionResult<IReadOnlyList<AutomatoPilhaExemploResponse>> Exemplos() =>
        Ok(servico.ListarExemplos());

    [HttpPost("simular")]
    public ActionResult<ResultadoPilha> Simular([FromBody] SimulacaoPilhaRequest req) =>
        servico.Simular(req);
}
