using SimuladorFormal.AutomatosPilha.Dominio;

namespace SimuladorFormal.AutomatosPilha.Execucao;

/// <summary>
/// Resultado de uma execução do AP. <see cref="Passos"/> é a computação que aceitou ou,
/// se nenhuma aceitou, o ramo que mais avançou na entrada.
/// </summary>
public sealed record ResultadoPilha(
    bool Aceita,
    bool Deterministico,
    CriterioAceitacao Criterio,
    string Mensagem,
    string Cadeia,
    int ConfiguracoesExploradas,
    string EstadoFinal,
    int PosicaoFinal,
    string PilhaFinal,
    IReadOnlyList<PassoPilha> Passos);
