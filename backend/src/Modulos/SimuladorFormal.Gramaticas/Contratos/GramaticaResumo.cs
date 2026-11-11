namespace SimuladorFormal.Gramaticas.Contratos;

/// <summary>A gramática como foi entendida (útil quando V, T e S foram inferidos).</summary>
public sealed record GramaticaResumo(
    IReadOnlyList<string> Variaveis,
    IReadOnlyList<string> Terminais,
    string SimboloInicial,
    IReadOnlyList<string> Producoes,
    IReadOnlyList<string> Anulaveis);
