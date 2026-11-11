namespace SimuladorFormal.Linguagens.Resultados;

/// <summary>Enumeração de Σ* ou Σ+ limitada a cadeias de tamanho ≤ k.</summary>
public sealed record Fecho(
    string Notacao,
    int ComprimentoMaximo,
    long TotalTeorico,
    bool Truncado,
    IReadOnlyList<string> Cadeias);
