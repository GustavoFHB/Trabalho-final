namespace SimuladorFormal.Gramaticas.Dominio;

/// <summary>
/// Gramática Livre de Contexto G = (V, T, P, S).
/// Variáveis e terminais são caracteres únicos.
/// </summary>
public sealed class GramaticaLivreContexto
{
    private readonly Dictionary<char, List<int>> _producoesPorVariavel;

    public IReadOnlySet<char> Variaveis { get; }
    public IReadOnlySet<char> Terminais { get; }
    public IReadOnlyList<Producao> Producoes { get; }
    public char SimboloInicial { get; }

    public GramaticaLivreContexto(
        IEnumerable<char> variaveis,
        IEnumerable<char> terminais,
        IEnumerable<Producao> producoes,
        char simboloInicial)
    {
        Variaveis = variaveis.ToHashSet();
        Terminais = terminais.ToHashSet();
        Producoes = producoes.ToList();
        SimboloInicial = simboloInicial;

        ValidadorGramatica.Validar(this);

        _producoesPorVariavel = Variaveis.ToDictionary(v => v, _ => new List<int>());
        for (int i = 0; i < Producoes.Count; i++)
            _producoesPorVariavel[Producoes[i].Esquerda].Add(i);
    }

    public bool EhVariavel(char simbolo) => Variaveis.Contains(simbolo);

    public bool EhTerminal(char simbolo) => Terminais.Contains(simbolo);

    /// <summary>Índices (em <see cref="Producoes"/>) das produções A → … de uma variável.</summary>
    public IReadOnlyList<int> IndicesProducoesDe(char variavel) => _producoesPorVariavel[variavel];

    public override string ToString() =>
        string.Join("\n", Producoes.GroupBy(p => p.Esquerda)
            .Select(g => $"{g.Key} → {string.Join(" | ", g.Select(p => p.Vazia ? "λ" : p.Direita))}"));
}
