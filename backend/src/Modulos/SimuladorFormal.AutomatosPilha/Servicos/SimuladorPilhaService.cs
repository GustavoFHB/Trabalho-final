using SimuladorFormal.AutomatosPilha.Contratos;
using SimuladorFormal.AutomatosPilha.Dominio;
using SimuladorFormal.AutomatosPilha.Exemplos;
using SimuladorFormal.AutomatosPilha.Execucao;
using SimuladorFormal.AutomatosPilha.Leitura;

namespace SimuladorFormal.AutomatosPilha.Servicos;

/// <summary>Monta o AP a partir da requisição, executa e lista os autômatos prontos.</summary>
public sealed class SimuladorPilhaService
{
    public IReadOnlyList<AutomatoPilhaExemploResponse> ListarExemplos() =>
        CatalogoAutomatosPilha.Todos.Select(d => new AutomatoPilhaExemploResponse(
            d.Id,
            d.Nome,
            d.Descricao,
            d.Automato.EstadoInicial,
            d.Automato.EstadosFinais.ToList(),
            d.Automato.SimboloInicialPilha?.ToString() ?? "",
            d.Automato.Criterio,
            d.Exemplos,
            d.Automato.Transicoes.Select(LeitorTransicaoPilha.ParaDto).ToList())).ToList();

    public ResultadoPilha Simular(SimulacaoPilhaRequest req)
    {
        if (req.Transicoes is null || req.Transicoes.Count == 0)
            throw new ArgumentException("Cadastre pelo menos uma transição.");

        var transicoes = req.Transicoes.Select((t, i) => LeitorTransicaoPilha.Ler(t, i + 1)).ToList();
        var finais = (req.EstadosFinais ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var automato = new AutomatoPilha(
            req.EstadoInicial,
            finais,
            LeitorTransicaoPilha.LerSimboloInicial(req.SimboloInicialPilha),
            req.Criterio ?? CriterioAceitacao.EstadoFinal,
            transicoes);

        return automato.Executar(req.Entrada, req.LimiteConfiguracoes ?? AutomatoPilha.LimiteConfiguracoesPadrao);
    }
}
