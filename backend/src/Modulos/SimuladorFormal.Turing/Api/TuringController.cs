using Microsoft.AspNetCore.Mvc;
using SimuladorFormal.Turing.Contratos;
using SimuladorFormal.Turing.Execucao;
using SimuladorFormal.Turing.Servicos;

namespace SimuladorFormal.Turing.Api;

/// <summary>Módulo 5 — Máquina de Turing.</summary>
[ApiController]
[Route("api/turing")]
public sealed class TuringController(SimuladorTuringService servico) : ControllerBase
{
    /// <summary>Máquinas prontas: soma unária (opção B) e w#w (opção A).</summary>
    [HttpGet("exemplos")]
    public ActionResult<IReadOnlyList<MaquinaExemploResponse>> Exemplos() =>
        Ok(servico.ListarExemplos());

    [HttpPost("simular")]
    public ActionResult<ResultadoSimulacao> Simular([FromBody] SimulacaoRequest req) =>
        servico.Simular(req);
}
