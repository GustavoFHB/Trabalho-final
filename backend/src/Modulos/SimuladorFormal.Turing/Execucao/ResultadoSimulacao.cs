namespace SimuladorFormal.Turing.Execucao;

/// <summary>Resultado de uma execução da Máquina de Turing, com o histórico de passos.</summary>
public sealed record ResultadoSimulacao(
    bool Aceita,
    string Motivo,
    string EstadoFinal,
    string FitaFinal,
    int CabecoteFinal,
    string Saida,
    IReadOnlyList<Passo> Passos);
