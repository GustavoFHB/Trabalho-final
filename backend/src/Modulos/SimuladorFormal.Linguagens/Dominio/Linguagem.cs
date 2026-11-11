using SimuladorFormal.Compartilhado;
using SimuladorFormal.Linguagens.Operacoes;

namespace SimuladorFormal.Linguagens.Dominio;

/// <summary>
/// Linguagem L ⊆ Σ*. Cada subclasse define como decidir se uma cadeia de Σ* pertence a L.
/// </summary>
public abstract class Linguagem
{
    public Alfabeto Sigma { get; }
    public string Definicao { get; }
    public abstract TipoLinguagem Tipo { get; }

    protected Linguagem(Alfabeto sigma, string definicao)
    {
        Sigma = sigma;
        Definicao = definicao;
    }

    /// <summary>w ∈ L ? Só cadeias de Σ* podem pertencer a L.</summary>
    public bool Pertence(string? cadeia)
    {
        var w = Simbolos.NormalizarVazia(cadeia);
        return ValidadorCadeia.Validar(Sigma, w).Valida && ContemCadeiaDeSigmaEstrela(w);
    }

    /// <summary>Decide w ∈ L sabendo que w ∈ Σ*.</summary>
    protected abstract bool ContemCadeiaDeSigmaEstrela(string w);

    /// <summary>L ∩ Σ^≤k em ordem canônica (a cadeia vazia aparece como λ).</summary>
    public virtual IReadOnlyList<string> Listar(int k) =>
        GeradorFecho.FechoKleene(Sigma, k).Cadeias
            .Where(w => ContemCadeiaDeSigmaEstrela(w == Simbolos.Lambda ? "" : w))
            .ToList();
}
