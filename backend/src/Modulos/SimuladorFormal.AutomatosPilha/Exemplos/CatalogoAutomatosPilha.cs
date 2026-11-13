using SimuladorFormal.AutomatosPilha.Dominio;

namespace SimuladorFormal.AutomatosPilha.Exemplos;

/// <summary>Descrição de um AP pronto e das cadeias sugeridas para testá-lo.</summary>
public sealed record DefinicaoAutomatoPilha(
    string Id,
    string Nome,
    string Descricao,
    AutomatoPilha Automato,
    IReadOnlyList<string> Exemplos);

/// <summary>Autômatos com pilha prontos do Módulo 4.</summary>
public static class CatalogoAutomatosPilha
{
    private static TransicaoPilha T(string de, char? a, char? desempilha, string para, string empilha) =>
        new(de, a, desempilha, para, empilha);

    /// <summary>
    /// Linguagem mínima do enunciado: L = { aⁿbⁿ | n ≥ 1 }. Determinístico.
    /// Empilha um A para cada a; desempilha um A para cada b; aceita quando só resta Z.
    /// </summary>
    public static DefinicaoAutomatoPilha AnBn { get; } = new(
        "anbn",
        "aⁿbⁿ (n ≥ 1)",
        "Empilha um A para cada a e desempilha um A para cada b. Aceita quando sobra só o Z no fundo.",
        new AutomatoPilha("q0", new[] { "q2" }, 'Z', CriterioAceitacao.EstadoFinal, new[]
        {
            T("q0", 'a', 'Z', "q0", "AZ"),  // primeiro a
            T("q0", 'a', 'A', "q0", "AA"),  // demais a
            T("q0", 'b', 'A', "q1", ""),    // primeiro b
            T("q1", 'b', 'A', "q1", ""),    // demais b
            T("q1", null, 'Z', "q2", "Z"),  // contou igual: aceita
        }),
        new[] { "aaabbb", "ab", "aabbb", "aaabb", "abab", "" });

    /// <summary>Palíndromos de tamanho par, L = { wwᴿ }. Não determinístico: "adivinha" o meio da cadeia.</summary>
    public static DefinicaoAutomatoPilha Palindromos { get; } = new(
        "palindromos",
        "Palíndromos pares wwᴿ",
        "Não determinístico: empilha a 1ª metade e, num ponto que ele \"adivinha\" ser o meio, passa a comparar.",
        new AutomatoPilha("q0", new[] { "q2" }, 'Z', CriterioAceitacao.EstadoFinal, new[]
        {
            T("q0", 'a', null, "q0", "A"),
            T("q0", 'b', null, "q0", "B"),
            T("q0", null, null, "q1", ""),  // palpite: chegou ao meio
            T("q1", 'a', 'A', "q1", ""),
            T("q1", 'b', 'B', "q1", ""),
            T("q1", null, 'Z', "q2", "Z"),
        }),
        new[] { "abba", "aabbaa", "abab", "aba" });

    /// <summary>Parênteses balanceados, aceitando por pilha vazia.</summary>
    public static DefinicaoAutomatoPilha Parenteses { get; } = new(
        "parenteses",
        "Parênteses balanceados",
        "Aceita por pilha vazia: cada ( empilha X, cada ) desempilha X e no fim o Z é removido.",
        new AutomatoPilha("q0", Array.Empty<string>(), 'Z', CriterioAceitacao.PilhaVazia, new[]
        {
            T("q0", '(', null, "q0", "X"),
            T("q0", ')', 'X', "q0", ""),
            T("q0", null, 'Z', "q0", ""),
        }),
        new[] { "(()())", "(()", "())(", "" });

    public static IReadOnlyList<DefinicaoAutomatoPilha> Todos { get; } = new[] { AnBn, Palindromos, Parenteses };
}
