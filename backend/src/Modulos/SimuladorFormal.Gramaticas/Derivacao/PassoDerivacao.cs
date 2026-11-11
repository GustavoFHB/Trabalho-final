namespace SimuladorFormal.Gramaticas.Derivacao;

/// <summary>Um passo da derivação: forma sentencial após aplicar a produção na posição indicada.</summary>
public sealed record PassoDerivacao(int Numero, string FormaSentencial, string? Producao, int? Posicao);
