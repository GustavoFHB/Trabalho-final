namespace SimuladorFormal.Gramaticas.Dominio;

/// <summary>Garante que G = (V, T, P, S) é uma GLC bem formada.</summary>
internal static class ValidadorGramatica
{
    public static void Validar(GramaticaLivreContexto g)
    {
        if (g.Variaveis.Count == 0)
            throw new ArgumentException("O conjunto de variáveis V não pode ser vazio.");

        var comuns = g.Variaveis.Intersect(g.Terminais).ToList();
        if (comuns.Count > 0)
            throw new ArgumentException($"Os símbolos {string.Join(", ", comuns)} não podem ser variável e terminal ao mesmo tempo.");

        if (!g.Variaveis.Contains(g.SimboloInicial))
        {
            var opcoes = string.Join(", ", g.Variaveis);
            var motivo = g.Terminais.Contains(g.SimboloInicial)
                ? $"'{g.SimboloInicial}' é um terminal"
                : $"'{g.SimboloInicial}' não foi declarado como variável";
            throw new ArgumentException(
                $"O símbolo inicial precisa ser uma variável, mas {motivo}. Use uma das variáveis: {opcoes}.");
        }

        foreach (var p in g.Producoes)
        {
            if (!g.Variaveis.Contains(p.Esquerda))
                throw new ArgumentException($"Na produção {p}, {p.Esquerda} não foi declarada como variável.");

            foreach (var c in p.Direita)
                if (!g.Variaveis.Contains(c) && !g.Terminais.Contains(c))
                    throw new ArgumentException($"Na produção {p}, o símbolo '{c}' não é variável nem terminal.");
        }
    }
}
