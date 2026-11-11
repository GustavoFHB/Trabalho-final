namespace SimuladorFormal.Compartilhado;

/// <summary>Lê conjuntos de símbolos escritos como "a,b", "{a, b}" ou "ab".</summary>
public static class LeitorSimbolos
{
    /// <param name="texto">Texto digitado pelo usuário.</param>
    /// <param name="nomeConjunto">Nome usado nas mensagens de erro (ex.: "Σ", "V").</param>
    /// <returns>Símbolos distintos na ordem em que aparecem; lista vazia se o texto estiver em branco.</returns>
    public static List<char> Ler(string? texto, string nomeConjunto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return new List<char>();

        var limpo = texto.Trim().TrimStart('{').TrimEnd('}');
        var partes = limpo.Contains(',')
            ? limpo.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            : limpo.Where(c => !char.IsWhiteSpace(c)).Select(c => c.ToString()).ToArray();

        foreach (var parte in partes)
            if (parte.Length != 1)
                throw new ArgumentException($"O símbolo \"{parte}\" de {nomeConjunto} deve ter exatamente 1 caractere.");

        return partes.Select(p => p[0]).Distinct().ToList();
    }
}
