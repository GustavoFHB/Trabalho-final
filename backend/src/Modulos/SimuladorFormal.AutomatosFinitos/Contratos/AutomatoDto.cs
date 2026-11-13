namespace SimuladorFormal.AutomatosFinitos.Contratos;

/// <summary>
/// Formato JSON de um autômato finito (o mesmo usado para carregar um arquivo .json na interface).
/// <code>
/// {
///   "estados": ["q0", "q1", "q2"],
///   "alfabeto": ["a", "b"],
///   "estadoInicial": "q0",
///   "estadosFinais": ["q2"],
///   "transicoes": [ { "origem": "q0", "simbolo": "a", "destino": "q1" } ]
/// }
/// </code>
/// </summary>
/// <param name="Estados">Q (opcional — inferido das transições).</param>
/// <param name="Alfabeto">Σ (opcional — inferido das transições).</param>
/// <param name="EstadoInicial">q0.</param>
/// <param name="EstadosFinais">F.</param>
/// <param name="Transicoes">δ. Para um AFN, repita (origem, símbolo) com destinos diferentes; use λ para transição vazia.</param>
public sealed record AutomatoDto(
    List<string>? Estados,
    List<string>? Alfabeto,
    string EstadoInicial,
    List<string>? EstadosFinais,
    List<TransicaoAfDto>? Transicoes);

/// <param name="Simbolo">Um caractere de Σ, ou λ / ε / &amp; / vazio para transição λ.</param>
public sealed record TransicaoAfDto(string Origem, string? Simbolo, string Destino);
