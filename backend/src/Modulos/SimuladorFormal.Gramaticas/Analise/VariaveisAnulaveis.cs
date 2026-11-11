using SimuladorFormal.Gramaticas.Dominio;

namespace SimuladorFormal.Gramaticas.Analise;

/// <summary>
/// Conjunto das variáveis anuláveis (A ⇒* λ).
/// Para cada uma guarda a produção que primeiro provou isso, o que garante
/// uma árvore λ finita mesmo com ciclos como A → B, B → A.
/// </summary>
public sealed class VariaveisAnulaveis
{
    private readonly Dictionary<char, int> _producaoVazia;

    private VariaveisAnulaveis(Dictionary<char, int> producaoVazia) => _producaoVazia = producaoVazia;

    public IReadOnlyCollection<char> Variaveis => _producaoVazia.Keys;

    public bool Contem(char variavel) => _producaoVazia.ContainsKey(variavel);

    /// <summary>Índice da produção usada para derivar λ a partir da variável.</summary>
    public int ProducaoVazia(char variavel) => _producaoVazia[variavel];

    /// <summary>
    /// Ponto fixo: A é anulável se tem produção cujo lado direito só contém anuláveis.
    /// O(|V|·|G|) no pior caso.
    /// </summary>
    public static VariaveisAnulaveis Calcular(GramaticaLivreContexto g)
    {
        var producao = new Dictionary<char, int>();
        bool mudou = true;
        while (mudou)
        {
            mudou = false;
            for (int i = 0; i < g.Producoes.Count; i++)
            {
                var p = g.Producoes[i];
                if (producao.ContainsKey(p.Esquerda)) continue;
                if (p.Direita.All(producao.ContainsKey))
                {
                    producao[p.Esquerda] = i;
                    mudou = true;
                }
            }
        }
        return new VariaveisAnulaveis(producao);
    }
}
