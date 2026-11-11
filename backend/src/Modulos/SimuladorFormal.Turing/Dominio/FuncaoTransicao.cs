namespace SimuladorFormal.Turing.Dominio;

/// <summary>
/// Função de transição δ: Q × Γ → Q × Γ × {L, R, S}.
/// Indexada por (estado, símbolo) — consulta O(1) por passo
/// (a versão Java original fazia busca linear O(|δ|) a cada passo).
/// </summary>
public sealed class FuncaoTransicao
{
    private readonly Dictionary<(string Estado, char Simbolo), Transicao> _delta = new();

    public FuncaoTransicao(IEnumerable<Transicao> transicoes)
    {
        foreach (var t in transicoes)
            if (!_delta.TryAdd((t.EstadoAtual, t.SimboloLido), t))
                throw new ArgumentException(
                    $"Máquina não determinística: há mais de uma transição para ({t.EstadoAtual}, {t.SimboloLido}).");
    }

    public int Quantidade => _delta.Count;

    public IReadOnlyCollection<Transicao> Todas => _delta.Values;

    public bool TentarObter(string estado, char simbolo, out Transicao transicao) =>
        _delta.TryGetValue((estado, simbolo), out transicao!);
}
