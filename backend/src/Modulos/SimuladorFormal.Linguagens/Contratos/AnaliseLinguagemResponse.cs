using SimuladorFormal.Linguagens.Resultados;

namespace SimuladorFormal.Linguagens.Contratos;

public sealed record AnaliseLinguagemResponse(
    IReadOnlyList<string> Alfabeto,
    string AlfabetoTexto,
    ValidacaoCadeia Cadeia,
    ValidacaoCadeia? Cadeia2,
    ConcatenacaoResponse Concatenacao,
    string Reverso,
    Fecho FechoKleene,
    Fecho FechoPositivo,
    LinguagemResponse? Linguagem);
