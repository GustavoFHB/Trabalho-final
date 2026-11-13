using SimuladorFormal.AutomatosFinitos.Dominio;

namespace SimuladorFormal.AutomatosFinitos.Execucao;

/// <summary>Resultado da execução de um autômato finito, com o passo a passo.</summary>
/// <param name="SimbolosLidos">Quantos símbolos foram consumidos antes de parar.</param>
/// <param name="EstadosIniciais">Fecho-λ de q0 (só q0 num AFD).</param>
/// <param name="TransicoesIniciais">Transições λ usadas para montar o fecho de q0.</param>
/// <param name="EstadosFinaisAlcancados">Estados finais em que o autômato terminou (vazio se rejeitou).</param>
/// <param name="Caminho">Um caminho q0 → … até a aceitação; num AFD rejeitado, o caminho percorrido. Nulo num AFN rejeitado.</param>
/// <param name="CaminhoTexto">O caminho no formato "q0 → q1 → q2".</param>
public sealed record ResultadoAutomato(
    bool Aceita,
    bool Deterministico,
    string Mensagem,
    string Cadeia,
    int SimbolosLidos,
    IReadOnlyList<string> EstadosIniciais,
    IReadOnlyList<TransicaoAf> TransicoesIniciais,
    IReadOnlyList<PassoAutomato> Passos,
    IReadOnlyList<string> EstadosFinaisAlcancados,
    IReadOnlyList<EstadoCaminho>? Caminho,
    string? CaminhoTexto);
