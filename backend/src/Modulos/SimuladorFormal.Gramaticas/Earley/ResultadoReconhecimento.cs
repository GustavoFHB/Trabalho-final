namespace SimuladorFormal.Gramaticas.Earley;

/// <summary>Resultado do reconhecimento: se aceitou, o item final [S → γ•, 0] e quantos itens foram criados.</summary>
public sealed record ResultadoReconhecimento(
    bool Aceita,
    ItemEarley? ItemFinal,
    int TotalItens,
    string Motivo);
