namespace SimuladorFormal.Gramaticas.Earley;

/// <summary>Qual operação do algoritmo de Earley criou o item.</summary>
public enum OrigemItem
{
    /// <summary>[B → •γ, k]: previsto porque um item esperava B.</summary>
    Predicao,

    /// <summary>Ponto avançado sobre um terminal lido da entrada.</summary>
    Leitura,

    /// <summary>Ponto avançado sobre uma variável que foi completada.</summary>
    Conclusao,

    /// <summary>Ponto avançado sobre uma variável anulável (correção de Aycock &amp; Horspool).</summary>
    Anulavel
}
