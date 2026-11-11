namespace SimuladorFormal.Linguagens.Contratos;

/// <summary>Resultado de w·v.</summary>
public sealed record ConcatenacaoResponse(string Resultado, int Tamanho, bool Valida);
