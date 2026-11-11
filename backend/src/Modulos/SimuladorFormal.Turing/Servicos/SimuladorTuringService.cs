using SimuladorFormal.Turing.Contratos;
using SimuladorFormal.Turing.Dominio;
using SimuladorFormal.Turing.Exemplos;
using SimuladorFormal.Turing.Execucao;
using SimuladorFormal.Turing.Leitura;

namespace SimuladorFormal.Turing.Servicos;

/// <summary>Monta a máquina a partir da requisição, executa e lista as máquinas prontas.</summary>
public sealed class SimuladorTuringService
{
    public IReadOnlyList<MaquinaExemploResponse> ListarExemplos() =>
        CatalogoMaquinas.Todas.Select(m => new MaquinaExemploResponse(
            m.Id,
            m.Nome,
            m.Descricao,
            m.EstadoInicial,
            m.EstadosFinais,
            m.Exemplos,
            m.Transicoes.Select(LeitorTransicao.ParaDto).ToList())).ToList();

    public ResultadoSimulacao Simular(SimulacaoRequest req)
    {
        if (req.Transicoes is null || req.Transicoes.Count == 0)
            throw new ArgumentException("Cadastre pelo menos uma transição.");

        var transicoes = req.Transicoes.Select((t, i) => LeitorTransicao.Ler(t, i + 1)).ToList();
        var finais = (req.EstadosFinais ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var maquina = new MaquinaTuring(req.EstadoInicial, finais, transicoes);

        return maquina.Executar(req.Entrada?.Trim() ?? "", req.LimitePassos ?? MaquinaTuring.LimitePassosPadrao);
    }
}
