namespace SimuladorFormal.Linguagens.Resultados;

/// <summary>Resultado da validação de uma cadeia sobre Σ.</summary>
public sealed record ValidacaoCadeia(
    string Cadeia,
    bool Valida,
    int Tamanho,
    IReadOnlyList<int> PosicoesInvalidas,
    string Mensagem);
