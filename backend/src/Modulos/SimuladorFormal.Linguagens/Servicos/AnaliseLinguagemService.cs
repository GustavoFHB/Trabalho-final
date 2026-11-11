using SimuladorFormal.Compartilhado;
using SimuladorFormal.Linguagens.Contratos;
using SimuladorFormal.Linguagens.Dominio;
using SimuladorFormal.Linguagens.Operacoes;

namespace SimuladorFormal.Linguagens.Servicos;

/// <summary>Executa as operações do Módulo 1 sobre uma requisição.</summary>
public sealed class AnaliseLinguagemService
{
    public const int TamanhoMaximoPermitido = 12;
    public const int TamanhoMaximoPadrao = 3;

    public AnaliseLinguagemResponse Analisar(AnaliseLinguagemRequest req)
    {
        var sigma = Alfabeto.Parse(req.Alfabeto);
        var w = Simbolos.NormalizarVazia(req.Cadeia);
        var v = Simbolos.NormalizarVazia(req.Cadeia2);

        int k = req.TamanhoMaximo ?? TamanhoMaximoPadrao;
        if (k < 0 || k > TamanhoMaximoPermitido)
            throw new ArgumentException($"O tamanho máximo para listar Σ* deve estar entre 0 e {TamanhoMaximoPermitido}.");

        var concatenacao = OperacoesCadeia.Concatenar(w, v);

        return new AnaliseLinguagemResponse(
            sigma.Simbolos.Select(c => c.ToString()).ToList(),
            sigma.ToString(),
            ValidadorCadeia.Validar(sigma, w),
            req.Cadeia2 is null ? null : ValidadorCadeia.Validar(sigma, v),
            new ConcatenacaoResponse(
                Simbolos.Exibir(concatenacao),
                OperacoesCadeia.Tamanho(concatenacao),
                ValidadorCadeia.Validar(sigma, concatenacao).Valida),
            Simbolos.Exibir(OperacoesCadeia.Reverso(w)),
            GeradorFecho.FechoKleene(sigma, k),
            GeradorFecho.FechoPositivo(sigma, k),
            AnalisarLinguagem(req, sigma, w, concatenacao, k));
    }

    /// <summary>L é opcional: só é analisada se o usuário escreveu algo ("{}" define a linguagem vazia ∅).</summary>
    private static LinguagemResponse? AnalisarLinguagem(
        AnaliseLinguagemRequest req, Alfabeto sigma, string w, string concatenacao, int k)
    {
        if (string.IsNullOrWhiteSpace(req.Linguagem)) return null;

        var L = FabricaLinguagem.Criar(sigma, req.TipoLinguagem, req.Linguagem);
        return new LinguagemResponse(L.Tipo, L.Definicao, L.Pertence(w), L.Pertence(concatenacao), L.Listar(k));
    }
}
