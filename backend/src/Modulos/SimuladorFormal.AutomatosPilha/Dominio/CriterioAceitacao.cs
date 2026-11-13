namespace SimuladorFormal.AutomatosPilha.Dominio;

/// <summary>Quando o AP aceita, depois de ler toda a entrada.</summary>
public enum CriterioAceitacao
{
    /// <summary>Está em um estado de F.</summary>
    EstadoFinal,

    /// <summary>A pilha ficou vazia.</summary>
    PilhaVazia
}
