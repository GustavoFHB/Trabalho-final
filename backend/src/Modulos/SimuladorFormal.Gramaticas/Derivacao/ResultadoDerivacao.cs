namespace SimuladorFormal.Gramaticas.Derivacao;

public sealed record ResultadoDerivacao(
    bool Aceita,
    string Cadeia,
    string Mensagem,
    IReadOnlyList<PassoDerivacao> Passos,
    NoArvore? Arvore,
    int ItensEarley);
