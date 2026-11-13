using SimuladorFormal.Gramaticas.Analise;
using SimuladorFormal.Gramaticas.Dominio;

namespace SimuladorFormal.Gramaticas.Earley;

/// <summary>
/// Reconhecedor de Earley para qualquer GLC (inclusive ambíguas, recursivas à esquerda e com λ).
///
/// Três operações preenchem os conjuntos S₀..Sₙ:
///   • Predição: item com •B → adiciona [B → •γ, k] para cada produção de B;
///   • Leitura: item com •a e entrada[k] = a → adiciona o item avançado em Sₖ₊₁;
///   • Conclusão: item [B → γ•, i] → avança os itens de Sᵢ que esperavam B.
/// λ-produções: ao prever uma variável anulável, o item que a esperava também é avançado
/// imediatamente (correção de Aycock &amp; Horspool).
///
/// Complexidade: O(n³) no pior caso, O(n²) em gramáticas não ambíguas, Θ(n) em gramáticas LR(k).
/// </summary>
public sealed class ReconhecedorEarley
{
    public const int TamanhoMaximoEntrada = 300;

    private readonly GramaticaLivreContexto _g;
    private readonly VariaveisAnulaveis _anulaveis;

    public ReconhecedorEarley(GramaticaLivreContexto gramatica, VariaveisAnulaveis anulaveis)
    {
        _g = gramatica;
        _anulaveis = anulaveis;
    }

    public ResultadoReconhecimento Reconhecer(string cadeia)
    {
        if (cadeia.Length > TamanhoMaximoEntrada)
            throw new ArgumentException($"A cadeia pode ter no máximo {TamanhoMaximoEntrada} símbolos.");

        for (int i = 0; i < cadeia.Length; i++)
            if (!_g.EhTerminal(cadeia[i]))
                return new ResultadoReconhecimento(false, null, 0,
                    $"O símbolo '{cadeia[i]}' (posição {i + 1}) não é um terminal da gramática.");

        int n = cadeia.Length;
        var S = new ConjuntoEarley[n + 1];
        for (int k = 0; k <= n; k++) S[k] = new ConjuntoEarley(_g);

        foreach (var p in _g.IndicesProducoesDe(_g.SimboloInicial))
            S[0].Adicionar(ItemEarley.Previsto(p, 0));

        for (int k = 0; k <= n; k++)
        {
            for (int idx = 0; idx < S[k].Quantidade; idx++)
            {
                var item = S[k].Itens[idx];
                var producao = _g.Producoes[item.Producao];

                if (item.Ponto == producao.Direita.Length)
                    Concluir(S, k, item, producao.Esquerda);
                else if (_g.EhVariavel(producao.Direita[item.Ponto]))
                    Prever(S[k], k, item, producao.Direita[item.Ponto]);
                else if (k < n && cadeia[k] == producao.Direita[item.Ponto])
                    S[k + 1].Adicionar(item.Avancar(OrigemItem.Leitura));
            }
        }

        int totalItens = S.Sum(c => c.Quantidade);
        var final = S[n].Itens.FirstOrDefault(EhItemFinal);
        return final is not null
            ? new ResultadoReconhecimento(true, final, totalItens, "")
            : new ResultadoReconhecimento(false, null, totalItens, MotivoRejeicao(S, cadeia));
    }

    private void Prever(ConjuntoEarley conjunto, int k, ItemEarley item, char variavel)
    {
        foreach (var p in _g.IndicesProducoesDe(variavel))
            conjunto.Adicionar(ItemEarley.Previsto(p, k));

        if (_anulaveis.Contem(variavel))
            conjunto.Adicionar(item.Avancar(OrigemItem.Anulavel, variavelAnulavel: variavel));
    }

    private static void Concluir(ConjuntoEarley[] S, int k, ItemEarley completo, char variavel)
    {
        var aguardando = S[completo.Inicio].AguardandoVariavel(variavel);
        int quantidade = aguardando.Count;
        for (int i = 0; i < quantidade; i++)
            S[k].Adicionar(aguardando[i].Avancar(OrigemItem.Conclusao, filho: completo));
    }

    /// <summary>[S → γ•, 0]: a variável inicial gerou a entrada inteira.</summary>
    private bool EhItemFinal(ItemEarley item) =>
        item.Inicio == 0 &&
        _g.Producoes[item.Producao].Esquerda == _g.SimboloInicial &&
        item.Ponto == _g.Producoes[item.Producao].Direita.Length;

    private static string MotivoRejeicao(ConjuntoEarley[] S, string cadeia)
    {
        int n = cadeia.Length;
        int ultimo = Enumerable.Range(0, n + 1).Last(k => S[k].Quantidade > 0);
        return ultimo < n
            ? $"Nenhuma derivação consegue gerar o prefixo \"{cadeia[..(ultimo + 1)]}\" (falha no símbolo {ultimo + 1})."
            : "A cadeia inteira foi lida, mas nenhuma derivação a partir do símbolo inicial a gera.";
    }
}
