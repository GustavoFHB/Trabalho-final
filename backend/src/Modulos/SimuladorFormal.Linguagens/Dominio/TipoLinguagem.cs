namespace SimuladorFormal.Linguagens.Dominio;

public enum TipoLinguagem
{
    /// <summary>L dada por enumeração: L = {ab, ba, λ}.</summary>
    Finita,

    /// <summary>L dada por expressão regular: L = L(a*b+).</summary>
    Regular
}
