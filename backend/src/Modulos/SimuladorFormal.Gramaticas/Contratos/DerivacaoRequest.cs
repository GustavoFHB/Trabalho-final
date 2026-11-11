namespace SimuladorFormal.Gramaticas.Contratos;

/// <param name="Producoes">Uma produção por linha: "S -> aSb | λ".</param>
/// <param name="Variaveis">V (opcional — inferido dos lados esquerdos).</param>
/// <param name="Terminais">T (opcional — inferido dos demais símbolos).</param>
/// <param name="SimboloInicial">S (opcional — lado esquerdo da 1ª produção).</param>
/// <param name="Cadeia">Cadeia a derivar (vazio ou λ = cadeia vazia).</param>
public sealed record DerivacaoRequest(
    string Producoes,
    string? Variaveis,
    string? Terminais,
    string? SimboloInicial,
    string? Cadeia);
