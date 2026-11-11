namespace SimuladorFormal.Gramaticas.Earley;

/// <summary>
/// Item [A → α•β, i]: "já lemos α, que gerou entrada[i..k)".
/// Os ponteiros <see cref="Anterior"/> e <see cref="Filho"/> permitem reconstruir a árvore depois.
/// </summary>
public sealed class ItemEarley
{
    /// <summary>Índice da produção A → αβ na gramática.</summary>
    public required int Producao { get; init; }

    /// <summary>Posição do ponto (•) no lado direito.</summary>
    public required int Ponto { get; init; }

    /// <summary>Posição da entrada em que o item começou (i).</summary>
    public required int Inicio { get; init; }

    public required OrigemItem Tipo { get; init; }

    /// <summary>Item antes de avançar o ponto (nulo em itens previstos).</summary>
    public ItemEarley? Anterior { get; init; }

    /// <summary>Item completo que justificou uma conclusão.</summary>
    public ItemEarley? Filho { get; init; }

    /// <summary>Variável anulável pulada (quando <see cref="Tipo"/> é Anulavel).</summary>
    public char VariavelAnulavel { get; init; }

    public (int, int, int) Chave => (Producao, Ponto, Inicio);

    public static ItemEarley Previsto(int producao, int inicio) =>
        new() { Producao = producao, Ponto = 0, Inicio = inicio, Tipo = OrigemItem.Predicao };

    /// <summary>Cria o item com o ponto uma posição à frente.</summary>
    public ItemEarley Avancar(OrigemItem tipo, ItemEarley? filho = null, char variavelAnulavel = '\0') => new()
    {
        Producao = Producao,
        Ponto = Ponto + 1,
        Inicio = Inicio,
        Tipo = tipo,
        Anterior = this,
        Filho = filho,
        VariavelAnulavel = variavelAnulavel
    };
}
