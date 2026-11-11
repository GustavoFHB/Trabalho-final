namespace SimuladorFormal.Linguagens.Operacoes;

/// <summary>Operações sobre cadeias: tamanho, concatenação, reverso e potência.</summary>
public static class OperacoesCadeia
{
    /// <summary>|w|. A cadeia vazia λ tem tamanho 0.</summary>
    public static int Tamanho(string? w) => string.IsNullOrEmpty(w) ? 0 : w.Length;

    /// <summary>w·v — Θ(|w| + |v|).</summary>
    public static string Concatenar(string? w, string? v) => (w ?? "") + (v ?? "");

    /// <summary>wᴿ — Θ(n).</summary>
    public static string Reverso(string? w)
    {
        var simbolos = (w ?? "").ToCharArray();
        Array.Reverse(simbolos);
        return new string(simbolos);
    }

    /// <summary>wⁿ: w concatenada n vezes (w⁰ = λ).</summary>
    public static string Potencia(string? w, int n)
    {
        if (n < 0) throw new ArgumentException("O expoente deve ser ≥ 0.");
        return string.Concat(Enumerable.Repeat(w ?? "", n));
    }
}
