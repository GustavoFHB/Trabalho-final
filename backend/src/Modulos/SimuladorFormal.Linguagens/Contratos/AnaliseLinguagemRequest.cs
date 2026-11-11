namespace SimuladorFormal.Linguagens.Contratos;

/// <param name="Alfabeto">Ex.: "a,b" ou "{a, b}".</param>
/// <param name="Cadeia">Cadeia w a validar.</param>
/// <param name="Cadeia2">Cadeia v usada na concatenação w·v (opcional).</param>
/// <param name="TamanhoMaximo">k: enumera Σ* e Σ+ até cadeias de tamanho k.</param>
/// <param name="TipoLinguagem">"finita" ou "regular" (opcional).</param>
/// <param name="Linguagem">Definição de L: "ab, ba, λ" ou "(a|b)*abb" (opcional).</param>
public sealed record AnaliseLinguagemRequest(
    string Alfabeto,
    string? Cadeia,
    string? Cadeia2,
    int? TamanhoMaximo,
    string? TipoLinguagem,
    string? Linguagem);
