using SimuladorFormal.AutomatosPilha.Dominio;

namespace SimuladorFormal.AutomatosPilha.Contratos;

/// <param name="Entrada">Cadeia de entrada (vazio ou λ = cadeia vazia).</param>
/// <param name="EstadoInicial">q0.</param>
/// <param name="EstadosFinais">Estados de aceitação separados por vírgula.</param>
/// <param name="SimboloInicialPilha">Z0 — símbolo que começa na pilha (vazio = pilha começa vazia).</param>
/// <param name="Criterio">EstadoFinal (padrão) ou PilhaVazia.</param>
/// <param name="Transicoes">Função δ.</param>
/// <param name="LimiteConfiguracoes">Máximo de configurações exploradas na busca.</param>
public sealed record SimulacaoPilhaRequest(
    string? Entrada,
    string EstadoInicial,
    string? EstadosFinais,
    string? SimboloInicialPilha,
    CriterioAceitacao? Criterio,
    List<TransicaoPilhaDto>? Transicoes,
    int? LimiteConfiguracoes);
