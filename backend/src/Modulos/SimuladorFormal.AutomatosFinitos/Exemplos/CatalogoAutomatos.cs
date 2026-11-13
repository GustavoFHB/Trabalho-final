using SimuladorFormal.AutomatosFinitos.Dominio;

namespace SimuladorFormal.AutomatosFinitos.Exemplos;

/// <summary>Descrição de um autômato pronto e das cadeias sugeridas para testá-lo.</summary>
public sealed record DefinicaoAutomato(string Id, string Nome, string Descricao, AutomatoFinito Automato, IReadOnlyList<string> Exemplos);

/// <summary>Autômatos prontos do Módulo 2: dois AFD, um AFN e um AFN com transições λ.</summary>
public static class CatalogoAutomatos
{
    private static TransicaoAf T(string de, char? a, string para) => new(de, a, para);

    /// <summary>Exemplo do enunciado: q0 –a→ q1, q1 –b→ q2. Reconhece só "ab".</summary>
    public static DefinicaoAutomato Enunciado { get; } = new(
        "enunciado",
        "AFD do enunciado",
        "q0 –a→ q1 –b→ q2. Reconhece apenas a cadeia ab.",
        new AutomatoFinito(
            new[] { "q0", "q1", "q2" }, new[] { 'a', 'b' }, "q0", new[] { "q2" },
            new[] { T("q0", 'a', "q1"), T("q1", 'b', "q2") }),
        new[] { "ab", "a", "abb", "ba" });

    /// <summary>AFD completo: aceita cadeias sobre {a, b} com número par de a.</summary>
    public static DefinicaoAutomato ParDeA { get; } = new(
        "par-de-a",
        "AFD: número par de a",
        "AFD completo sobre {a, b}: q0 = \"vi um número par de a\", q1 = \"ímpar\".",
        new AutomatoFinito(
            new[] { "q0", "q1" }, new[] { 'a', 'b' }, "q0", new[] { "q0" },
            new[] { T("q0", 'a', "q1"), T("q0", 'b', "q0"), T("q1", 'a', "q0"), T("q1", 'b', "q1") }),
        new[] { "abba", "aab", "", "babab" });

    /// <summary>AFN: termina em ab. Em q0, ao ler a, "adivinha" se é o penúltimo símbolo.</summary>
    public static DefinicaoAutomato TerminaEmAb { get; } = new(
        "termina-em-ab",
        "AFN: termina em ab",
        "Não determinístico: com a, q0 pode ficar em q0 ou apostar que começou o sufixo ab.",
        new AutomatoFinito(
            new[] { "q0", "q1", "q2" }, new[] { 'a', 'b' }, "q0", new[] { "q2" },
            new[] { T("q0", 'a', "q0"), T("q0", 'b', "q0"), T("q0", 'a', "q1"), T("q1", 'b', "q2") }),
        new[] { "aab", "abab", "aba", "b" });

    /// <summary>AFN-λ: a* ∪ b*. Duas transições λ escolhem o ramo antes de ler qualquer símbolo.</summary>
    public static DefinicaoAutomato AEstrelaOuBEstrela { get; } = new(
        "a-ou-b",
        "AFN-λ: a* ∪ b*",
        "Transições λ levam q0 a q1 (só a) e a q2 (só b): o autômato segue os dois ramos ao mesmo tempo.",
        new AutomatoFinito(
            new[] { "q0", "q1", "q2" }, new[] { 'a', 'b' }, "q0", new[] { "q1", "q2" },
            new[] { T("q0", null, "q1"), T("q0", null, "q2"), T("q1", 'a', "q1"), T("q2", 'b', "q2") }),
        new[] { "aaa", "bb", "", "ab" });

    public static IReadOnlyList<DefinicaoAutomato> Todos { get; } = new[] { Enunciado, ParDeA, TerminaEmAb, AEstrelaOuBEstrela };
}
