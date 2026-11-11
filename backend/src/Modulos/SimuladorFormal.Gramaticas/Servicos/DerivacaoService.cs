using SimuladorFormal.Compartilhado;
using SimuladorFormal.Gramaticas.Analise;
using SimuladorFormal.Gramaticas.Contratos;
using SimuladorFormal.Gramaticas.Derivacao;
using SimuladorFormal.Gramaticas.Dominio;
using SimuladorFormal.Gramaticas.Earley;
using SimuladorFormal.Gramaticas.Leitura;

namespace SimuladorFormal.Gramaticas.Servicos;

/// <summary>
/// Orquestra o Módulo 3: lê a gramática, calcula anuláveis, reconhece a cadeia com Earley,
/// reconstrói a árvore e gera a derivação mais à esquerda.
/// </summary>
public sealed class DerivacaoService
{
    public DerivacaoResponse Executar(DerivacaoRequest req)
    {
        var g = LeitorGramatica.Ler(req.Producoes, req.Variaveis, req.Terminais, req.SimboloInicial);
        var anulaveis = VariaveisAnulaveis.Calcular(g);
        return new DerivacaoResponse(Resumir(g, anulaveis), Derivar(g, anulaveis, req.Cadeia));
    }

    public ResultadoDerivacao Derivar(GramaticaLivreContexto g, string? cadeia) =>
        Derivar(g, VariaveisAnulaveis.Calcular(g), cadeia);

    private static ResultadoDerivacao Derivar(GramaticaLivreContexto g, VariaveisAnulaveis anulaveis, string? cadeia)
    {
        var w = Simbolos.NormalizarVazia(cadeia);
        var reconhecimento = new ReconhecedorEarley(g, anulaveis).Reconhecer(w);

        if (!reconhecimento.Aceita)
            return new ResultadoDerivacao(false, w, reconhecimento.Motivo,
                Array.Empty<PassoDerivacao>(), null, reconhecimento.TotalItens);

        var arvore = new ConstrutorArvore(g, anulaveis).Construir(reconhecimento.ItemFinal!);
        var passos = GeradorDerivacaoEsquerda.Gerar(arvore);
        return new ResultadoDerivacao(true, w, $"Cadeia gerada pela gramática em {passos.Count - 1} passo(s).",
            passos, arvore, reconhecimento.TotalItens);
    }

    private static GramaticaResumo Resumir(GramaticaLivreContexto g, VariaveisAnulaveis anulaveis) => new(
        g.Variaveis.Select(c => c.ToString()).ToList(),
        g.Terminais.Select(c => c.ToString()).ToList(),
        g.SimboloInicial.ToString(),
        g.Producoes.Select(p => p.ToString()).ToList(),
        anulaveis.Variaveis.Select(c => c.ToString()).ToList());
}
