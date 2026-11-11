using SimuladorFormal.Compartilhado;
using SimuladorFormal.Gramaticas.Analise;
using SimuladorFormal.Gramaticas.Dominio;
using SimuladorFormal.Gramaticas.Earley;

namespace SimuladorFormal.Gramaticas.Derivacao;

/// <summary>
/// Reconstrói a árvore de derivação seguindo os ponteiros dos itens de Earley — Θ(nós da árvore).
/// </summary>
public sealed class ConstrutorArvore
{
    private readonly GramaticaLivreContexto _g;
    private readonly VariaveisAnulaveis _anulaveis;

    public ConstrutorArvore(GramaticaLivreContexto gramatica, VariaveisAnulaveis anulaveis)
    {
        _g = gramatica;
        _anulaveis = anulaveis;
    }

    /// <summary>Árvore cuja raiz é a variável do item completo [A → γ•, i].</summary>
    public NoArvore Construir(ItemEarley itemCompleto)
    {
        var producao = _g.Producoes[itemCompleto.Producao];
        var no = new NoArvore(producao.Esquerda.ToString());
        if (producao.Vazia) no.Filhos.Add(new NoArvore(Simbolos.Lambda));
        else no.Filhos.AddRange(Filhos(itemCompleto));
        return no;
    }

    /// <summary>Percorre a cadeia de itens "Anterior" (da direita para a esquerda) montando os filhos.</summary>
    private List<NoArvore> Filhos(ItemEarley item)
    {
        var filhos = new List<NoArvore>();
        for (var atual = item; atual.Tipo != OrigemItem.Predicao; atual = atual.Anterior!)
        {
            var simbolo = _g.Producoes[atual.Producao].Direita[atual.Ponto - 1];
            filhos.Add(atual.Tipo switch
            {
                OrigemItem.Leitura => new NoArvore(simbolo.ToString()),
                OrigemItem.Conclusao => Construir(atual.Filho!),
                _ => ArvoreVazia(atual.VariavelAnulavel)
            });
        }
        filhos.Reverse();
        return filhos;
    }

    /// <summary>Árvore que deriva λ a partir de uma variável anulável.</summary>
    private NoArvore ArvoreVazia(char variavel)
    {
        var producao = _g.Producoes[_anulaveis.ProducaoVazia(variavel)];
        var no = new NoArvore(variavel.ToString());
        if (producao.Vazia) no.Filhos.Add(new NoArvore(Simbolos.Lambda));
        else foreach (var c in producao.Direita) no.Filhos.Add(ArvoreVazia(c));
        return no;
    }
}
