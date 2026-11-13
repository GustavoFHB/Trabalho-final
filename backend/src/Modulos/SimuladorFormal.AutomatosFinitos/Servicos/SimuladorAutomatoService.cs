using SimuladorFormal.AutomatosFinitos.Contratos;
using SimuladorFormal.AutomatosFinitos.Exemplos;
using SimuladorFormal.AutomatosFinitos.Execucao;
using SimuladorFormal.AutomatosFinitos.Leitura;

namespace SimuladorFormal.AutomatosFinitos.Servicos;

/// <summary>Monta o autômato a partir do JSON, executa a cadeia e lista os autômatos prontos.</summary>
public sealed class SimuladorAutomatoService
{
    public IReadOnlyList<AutomatoExemploResponse> ListarExemplos() =>
        CatalogoAutomatos.Todos.Select(d => new AutomatoExemploResponse(
            d.Id, d.Nome, d.Descricao, d.Exemplos, LeitorAutomato.ParaDto(d.Automato))).ToList();

    public ResultadoAutomato Simular(SimulacaoAutomatoRequest req) =>
        LeitorAutomato.Ler(req.Automato).Executar(req.Cadeia);
}
