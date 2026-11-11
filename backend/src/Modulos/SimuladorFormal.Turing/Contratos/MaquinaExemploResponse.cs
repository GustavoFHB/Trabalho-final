namespace SimuladorFormal.Turing.Contratos;

/// <summary>Máquina pronta oferecida na interface.</summary>
public sealed record MaquinaExemploResponse(
    string Id,
    string Nome,
    string Descricao,
    string EstadoInicial,
    IReadOnlyList<string> EstadosFinais,
    IReadOnlyList<string> Exemplos,
    IReadOnlyList<TransicaoDto> Transicoes);
