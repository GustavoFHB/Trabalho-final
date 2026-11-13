using SimuladorFormal.AutomatosFinitos.Dominio;

namespace SimuladorFormal.AutomatosFinitos.Execucao;

/// <summary>Leitura de um símbolo: de quais estados partiu, que transições usou (inclusive λ) e onde chegou.</summary>
public sealed record PassoAutomato(
    int Numero,
    int Posicao,
    char Simbolo,
    IReadOnlyList<string> EstadosAntes,
    IReadOnlyList<TransicaoAf> Transicoes,
    IReadOnlyList<string> EstadosDepois);
