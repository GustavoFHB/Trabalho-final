using SimuladorFormal.Compartilhado;

namespace SimuladorFormal.AutomatosFinitos.Dominio;

/// <summary>
/// Uma seta do diagrama: δ(origem, símbolo) ∋ destino.
/// <see cref="Simbolo"/> nulo representa uma transição λ (só permitida em AFN).
/// </summary>
public sealed record TransicaoAf(string Origem, char? Simbolo, string Destino)
{
    public bool Vazia => Simbolo is null;

    /// <summary>O símbolo como texto, com λ para a transição vazia.</summary>
    public string SimboloTexto => Simbolo?.ToString() ?? Simbolos.Lambda;

    public override string ToString() => $"{Origem} –{SimboloTexto}→ {Destino}";
}
