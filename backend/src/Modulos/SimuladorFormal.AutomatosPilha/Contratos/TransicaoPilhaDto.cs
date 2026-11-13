namespace SimuladorFormal.AutomatosPilha.Contratos;

/// <summary>Transição como texto, do jeito que o usuário digita (λ, ε, &amp; ou vazio = não lê / não desempilha / não empilha).</summary>
public sealed record TransicaoPilhaDto(
    string Origem,
    string? Simbolo,
    string? Desempilha,
    string Destino,
    string? Empilha);
