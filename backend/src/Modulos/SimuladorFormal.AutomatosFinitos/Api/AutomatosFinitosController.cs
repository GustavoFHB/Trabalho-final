using Microsoft.AspNetCore.Mvc;
using SimuladorFormal.AutomatosFinitos.Contratos;
using SimuladorFormal.AutomatosFinitos.Execucao;
using SimuladorFormal.AutomatosFinitos.Servicos;

namespace SimuladorFormal.AutomatosFinitos.Api;

/// <summary>Módulo 2 — Autômatos finitos (AFD e AFN).</summary>
[ApiController]
[Route("api/automatos")]
public sealed class AutomatosFinitosController(SimuladorAutomatoService servico) : ControllerBase
{
    /// <summary>Autômatos prontos, no mesmo formato JSON aceito por /simular.</summary>
    [HttpGet("exemplos")]
    public ActionResult<IReadOnlyList<AutomatoExemploResponse>> Exemplos() =>
        Ok(servico.ListarExemplos());

    [HttpPost("simular")]
    public ActionResult<ResultadoAutomato> Simular([FromBody] SimulacaoAutomatoRequest req) =>
        servico.Simular(req);
}
