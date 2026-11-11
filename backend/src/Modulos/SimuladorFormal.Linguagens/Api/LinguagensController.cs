using Microsoft.AspNetCore.Mvc;
using SimuladorFormal.Linguagens.Contratos;
using SimuladorFormal.Linguagens.Servicos;

namespace SimuladorFormal.Linguagens.Api;

/// <summary>Módulo 1 — Linguagens Formais.</summary>
[ApiController]
[Route("api/linguagens")]
public sealed class LinguagensController(AnaliseLinguagemService servico) : ControllerBase
{
    [HttpPost("analisar")]
    public ActionResult<AnaliseLinguagemResponse> Analisar([FromBody] AnaliseLinguagemRequest req) =>
        servico.Analisar(req);
}
