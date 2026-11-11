using SimuladorFormal.Turing.Dominio;

namespace SimuladorFormal.Turing.Exemplos;

/// <summary>Descrição de uma máquina pronta: estados, δ e entradas de exemplo.</summary>
public sealed record DefinicaoMaquina(
    string Id,
    string Nome,
    string Descricao,
    string EstadoInicial,
    IReadOnlyList<string> EstadosFinais,
    IReadOnlyList<Transicao> Transicoes,
    IReadOnlyList<string> Exemplos)
{
    public MaquinaTuring Criar() => new(EstadoInicial, EstadosFinais, Transicoes);
}
