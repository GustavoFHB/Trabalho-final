namespace SimuladorFormal.AutomatosFinitos.Contratos;

/// <summary>Autômato pronto oferecido na interface, já no formato JSON de entrada.</summary>
public sealed record AutomatoExemploResponse(
    string Id,
    string Nome,
    string Descricao,
    IReadOnlyList<string> Exemplos,
    AutomatoDto Automato);
