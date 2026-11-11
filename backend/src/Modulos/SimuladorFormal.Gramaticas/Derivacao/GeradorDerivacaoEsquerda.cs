using SimuladorFormal.Compartilhado;

namespace SimuladorFormal.Gramaticas.Derivacao;

/// <summary>
/// Gera a derivação mais à esquerda a partir da árvore: em cada passo
/// expande a variável mais à esquerda da forma sentencial.
/// </summary>
public static class GeradorDerivacaoEsquerda
{
    public static List<PassoDerivacao> Gerar(NoArvore raiz)
    {
        var forma = new List<NoArvore> { raiz };
        var passos = new List<PassoDerivacao> { new(0, raiz.Simbolo, null, null) };

        while (true)
        {
            int posicao = forma.FindIndex(no => !no.EhFolha);
            if (posicao < 0) break;

            var no = forma[posicao];
            var novos = no.Filhos.Where(f => f.Simbolo != Simbolos.Lambda).ToList();
            var direita = Simbolos.Exibir(string.Concat(novos.Select(f => f.Simbolo)));

            forma.RemoveAt(posicao);
            forma.InsertRange(posicao, novos);

            var texto = Simbolos.Exibir(string.Concat(forma.Select(f => f.Simbolo)));
            passos.Add(new PassoDerivacao(passos.Count, texto, $"{no.Simbolo} → {direita}", posicao));
        }
        return passos;
    }
}
