using SimuladorFormal.Linguagens.Dominio;

namespace SimuladorFormal.Linguagens.Contratos;

/// <summary>Análise da linguagem L definida pelo usuário.</summary>
public sealed record LinguagemResponse(
    TipoLinguagem Tipo,
    string Definicao,
    bool CadeiaPertence,
    bool ConcatenacaoPertence,
    IReadOnlyList<string> Cadeias);
