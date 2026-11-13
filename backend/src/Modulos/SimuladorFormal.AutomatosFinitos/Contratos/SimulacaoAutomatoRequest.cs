namespace SimuladorFormal.AutomatosFinitos.Contratos;

/// <param name="Automato">O autômato, no formato JSON de <see cref="AutomatoDto"/>.</param>
/// <param name="Cadeia">Cadeia de entrada (vazio ou λ = cadeia vazia).</param>
public sealed record SimulacaoAutomatoRequest(AutomatoDto? Automato, string? Cadeia);
