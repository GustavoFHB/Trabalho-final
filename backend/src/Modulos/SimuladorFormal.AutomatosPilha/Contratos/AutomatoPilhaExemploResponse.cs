using SimuladorFormal.AutomatosPilha.Dominio;

namespace SimuladorFormal.AutomatosPilha.Contratos;

/// <summary>Autômato com pilha pronto oferecido na interface.</summary>
public sealed record AutomatoPilhaExemploResponse(
    string Id,
    string Nome,
    string Descricao,
    string EstadoInicial,
    IReadOnlyList<string> EstadosFinais,
    string SimboloInicialPilha,
    CriterioAceitacao Criterio,
    IReadOnlyList<string> Exemplos,
    IReadOnlyList<TransicaoPilhaDto> Transicoes);
