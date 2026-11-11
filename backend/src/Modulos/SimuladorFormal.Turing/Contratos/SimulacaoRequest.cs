namespace SimuladorFormal.Turing.Contratos;

/// <param name="Entrada">Conteúdo inicial da fita.</param>
/// <param name="EstadoInicial">q0.</param>
/// <param name="EstadosFinais">Estados de aceitação separados por vírgula.</param>
/// <param name="Transicoes">Função δ.</param>
/// <param name="LimitePassos">Interrompe a execução após este número de passos.</param>
public sealed record SimulacaoRequest(
    string? Entrada,
    string EstadoInicial,
    string EstadosFinais,
    List<TransicaoDto>? Transicoes,
    int? LimitePassos);
