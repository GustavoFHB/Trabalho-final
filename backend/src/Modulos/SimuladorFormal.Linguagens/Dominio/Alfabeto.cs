using SimuladorFormal.Compartilhado;

namespace SimuladorFormal.Linguagens.Dominio;

/// <summary>
/// Alfabeto Σ: conjunto finito e não vazio de símbolos (cada símbolo é um caractere).
/// </summary>
public sealed class Alfabeto
{
    private readonly HashSet<char> _simbolos;

    /// <summary>Símbolos na ordem em que foram definidos — define a ordem lexicográfica de Σ*.</summary>
    public IReadOnlyList<char> Simbolos { get; }

    public int Tamanho => Simbolos.Count;

    public Alfabeto(IEnumerable<char> simbolos)
    {
        var lista = new List<char>();
        _simbolos = new HashSet<char>();
        foreach (var s in simbolos)
        {
            if (char.IsWhiteSpace(s))
                throw new ArgumentException("Espaços não podem ser símbolos do alfabeto.");
            if (_simbolos.Add(s))
                lista.Add(s);
        }

        if (lista.Count == 0)
            throw new ArgumentException("O alfabeto Σ não pode ser vazio.");

        Simbolos = lista;
    }

    /// <summary>Lê "a,b", "{a, b}" ou "ab".</summary>
    public static Alfabeto Parse(string? texto)
    {
        var simbolos = LeitorSimbolos.Ler(texto, "Σ");
        if (simbolos.Count == 0)
            throw new ArgumentException("Informe os símbolos do alfabeto Σ.");
        return new Alfabeto(simbolos);
    }

    /// <summary>Pertinência de um símbolo em Σ — O(1) com HashSet.</summary>
    public bool Contem(char simbolo) => _simbolos.Contains(simbolo);

    public override string ToString() => "{" + string.Join(", ", Simbolos) + "}";
}
