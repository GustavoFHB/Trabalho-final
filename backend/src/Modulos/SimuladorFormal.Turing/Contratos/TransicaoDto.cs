namespace SimuladorFormal.Turing.Contratos;

/// <summary>Transição como texto, do jeito que o usuário digita (direção: R, L ou S).</summary>
public sealed record TransicaoDto(
    string EstadoAtual,
    string SimboloLido,
    string NovoEstado,
    string SimboloEscrito,
    string Direcao);
