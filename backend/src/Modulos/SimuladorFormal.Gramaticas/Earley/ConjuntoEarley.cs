using SimuladorFormal.Gramaticas.Dominio;

namespace SimuladorFormal.Gramaticas.Earley;

/// <summary>
/// Conjunto Sₖ de itens de Earley. Evita duplicatas e indexa os itens
/// pela variável que esperam (o símbolo logo após o ponto), acelerando a conclusão.
/// </summary>
public sealed class ConjuntoEarley
{
    private readonly GramaticaLivreContexto _g;
    private readonly List<ItemEarley> _itens = new();
    private readonly HashSet<(int, int, int)> _chaves = new();
    private readonly Dictionary<char, List<ItemEarley>> _aguardando = new();

    public ConjuntoEarley(GramaticaLivreContexto gramatica) => _g = gramatica;

    public IReadOnlyList<ItemEarley> Itens => _itens;

    public int Quantidade => _itens.Count;

    /// <summary>Adiciona o item se ainda não existir. Retorna false para duplicatas.</summary>
    public bool Adicionar(ItemEarley item)
    {
        if (!_chaves.Add(item.Chave)) return false;
        _itens.Add(item);

        var direita = _g.Producoes[item.Producao].Direita;
        if (item.Ponto < direita.Length && _g.EhVariavel(direita[item.Ponto]))
        {
            var variavel = direita[item.Ponto];
            if (!_aguardando.TryGetValue(variavel, out var lista))
                _aguardando[variavel] = lista = new List<ItemEarley>();
            lista.Add(item);
        }
        return true;
    }

    /// <summary>Itens com o ponto imediatamente antes da variável.</summary>
    public IReadOnlyList<ItemEarley> AguardandoVariavel(char variavel) =>
        _aguardando.TryGetValue(variavel, out var lista) ? lista : Array.Empty<ItemEarley>();
}
