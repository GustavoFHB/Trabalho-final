namespace SimuladorFormal.Compartilhado;

/// <summary>Convenções de escrita compartilhadas por todos os módulos.</summary>
public static class Simbolos
{
    /// <summary>Símbolo usado para exibir a cadeia vazia.</summary>
    public const string Lambda = "λ";

    private static readonly HashSet<string> RepresentacoesVazias = new() { "λ", "ε", "&" };

    /// <summary>λ, ε, &amp;, texto vazio ou nulo representam a cadeia vazia.</summary>
    public static bool RepresentaVazia(string? texto) =>
        string.IsNullOrWhiteSpace(texto) || RepresentacoesVazias.Contains(texto.Trim());

    /// <summary>Converte as representações de λ em "" e remove espaços das pontas.</summary>
    public static string NormalizarVazia(string? texto) => RepresentaVazia(texto) ? "" : texto!.Trim();

    /// <summary>Mostra "" como λ.</summary>
    public static string Exibir(string cadeia) => string.IsNullOrEmpty(cadeia) ? Lambda : cadeia;
}
