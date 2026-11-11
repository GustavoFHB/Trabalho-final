using SimuladorFormal.Compartilhado;
using SimuladorFormal.Gramaticas.Dominio;

namespace SimuladorFormal.Gramaticas.Leitura;

/// <summary>
/// Monta uma gramática a partir do texto digitado. Cada linha: "S -> aSb | λ".
/// Se V, T ou S vierem em branco, são inferidos: V = lados esquerdos,
/// T = demais símbolos usados nas produções, S = lado esquerdo da 1ª produção.
/// </summary>
public static class LeitorGramatica
{
    private static readonly string[] Setas = { "->", "→", "::=" };

    public static GramaticaLivreContexto Ler(
        string? producoesTexto,
        string? variaveisTexto = null,
        string? terminaisTexto = null,
        string? inicialTexto = null)
    {
        var producoes = LerProducoes(producoesTexto);

        var variaveis = LeitorSimbolos.Ler(variaveisTexto, "V");
        if (variaveis.Count == 0)
            variaveis = producoes.Select(p => p.Esquerda).Distinct().ToList();

        var terminais = LeitorSimbolos.Ler(terminaisTexto, "T");
        if (terminais.Count == 0)
            terminais = producoes.SelectMany(p => p.Direita).Where(c => !variaveis.Contains(c)).Distinct().ToList();

        return new GramaticaLivreContexto(variaveis, terminais, producoes, LerInicial(inicialTexto, producoes));
    }

    private static List<Producao> LerProducoes(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new ArgumentException("Informe pelo menos uma produção (ex.: S -> aSb | λ).");

        var producoes = new List<Producao>();
        var linhas = texto.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        for (int n = 0; n < linhas.Length; n++)
        {
            var linha = linhas[n];
            var seta = Setas.FirstOrDefault(linha.Contains)
                       ?? throw new ArgumentException($"Linha {n + 1}: use \"->\" para separar os lados da produção.");

            var lados = linha.Split(seta, 2);
            var esquerda = lados[0].Trim();
            if (esquerda.Length != 1)
                throw new ArgumentException($"Linha {n + 1}: o lado esquerdo deve ser uma única variável (encontrado \"{esquerda}\").");

            foreach (var alternativa in lados[1].Split('|'))
            {
                var direita = Simbolos.RepresentaVazia(alternativa) ? "" : alternativa.Replace(" ", "");
                producoes.Add(new Producao(esquerda[0], direita));
            }
        }
        return producoes;
    }

    private static char LerInicial(string? texto, List<Producao> producoes)
    {
        if (string.IsNullOrWhiteSpace(texto)) return producoes[0].Esquerda;
        var limpo = texto.Trim();
        if (limpo.Length != 1)
            throw new ArgumentException("O símbolo inicial deve ser uma única variável.");
        return limpo[0];
    }
}
