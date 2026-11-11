namespace SimuladorFormal.Gramaticas.Derivacao;

/// <summary>Nó da árvore de derivação. Folhas são terminais ou λ.</summary>
public sealed class NoArvore
{
    public string Simbolo { get; }
    public List<NoArvore> Filhos { get; } = new();

    public NoArvore(string simbolo) => Simbolo = simbolo;

    public bool EhFolha => Filhos.Count == 0;
}
