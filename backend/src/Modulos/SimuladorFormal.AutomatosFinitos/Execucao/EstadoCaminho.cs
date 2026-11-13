namespace SimuladorFormal.AutomatosFinitos.Execucao;

/// <summary>Um estado do caminho q0 → … e o símbolo lido para chegar nele (nulo no estado inicial, λ em transição vazia).</summary>
public sealed record EstadoCaminho(string Estado, string? Simbolo);
