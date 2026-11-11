using SimuladorFormal.Compartilhado;
using SimuladorFormal.Linguagens.Dominio;
using SimuladorFormal.Linguagens.Resultados;

namespace SimuladorFormal.Linguagens.Operacoes;

/// <summary>Gera Σ* (fecho de Kleene) e Σ+ (fecho positivo) até um tamanho máximo.</summary>
public static class GeradorFecho
{
    /// <summary>Máximo de cadeias devolvidas (Σ* é infinito).</summary>
    public const int LimiteCadeiasListadas = 5000;

    /// <summary>Σ* = Σ⁰ ∪ Σ¹ ∪ … ∪ Σᵏ.</summary>
    public static Fecho FechoKleene(Alfabeto sigma, int k) => Gerar(sigma, k, incluirVazia: true);

    /// <summary>Σ+ = Σ¹ ∪ … ∪ Σᵏ (Σ* sem λ).</summary>
    public static Fecho FechoPositivo(Alfabeto sigma, int k) => Gerar(sigma, k, incluirVazia: false);

    /// <summary>|Σ⁰| + |Σ¹| + … + |Σᵏ| (com ou sem Σ⁰), saturando em long.MaxValue.</summary>
    public static long ContarAteTamanho(int tamanhoAlfabeto, int k, bool incluirVazia)
    {
        long total = 0, potencia = 1;
        for (int i = 0; i <= k; i++)
        {
            if (i > 0 || incluirVazia) total = SomaSegura(total, potencia);
            potencia = MultiplicacaoSegura(potencia, tamanhoAlfabeto);
        }
        return total;
    }

    /// <summary>
    /// Ordem canônica (por tamanho, depois lexicográfica segundo a ordem de Σ).
    /// Σⁱ é obtido concatenando cada símbolo às cadeias de Σⁱ⁻¹ — Θ(k·|Σ|ᵏ).
    /// </summary>
    private static Fecho Gerar(Alfabeto sigma, int k, bool incluirVazia)
    {
        if (k < 0) throw new ArgumentException("O tamanho máximo deve ser ≥ 0.");

        var resultado = new List<string>();
        bool truncado = false;
        if (incluirVazia) resultado.Add(Simbolos.Lambda);

        var nivel = new List<string> { "" };
        for (int tamanho = 1; tamanho <= k && !truncado; tamanho++)
        {
            var proximo = new List<string>();
            foreach (var prefixo in nivel)
            {
                foreach (var s in sigma.Simbolos)
                {
                    if (resultado.Count >= LimiteCadeiasListadas) { truncado = true; break; }
                    var w = prefixo + s;
                    proximo.Add(w);
                    resultado.Add(w);
                }
                if (truncado) break;
            }
            nivel = proximo;
        }

        return new Fecho(
            incluirVazia ? "Σ*" : "Σ+",
            k,
            ContarAteTamanho(sigma.Tamanho, k, incluirVazia),
            truncado,
            resultado);
    }

    private static long SomaSegura(long a, long b) => a > long.MaxValue - b ? long.MaxValue : a + b;

    private static long MultiplicacaoSegura(long a, long b) =>
        b != 0 && a > long.MaxValue / b ? long.MaxValue : a * b;
}
