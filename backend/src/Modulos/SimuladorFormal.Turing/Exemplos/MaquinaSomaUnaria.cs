using SimuladorFormal.Turing.Dominio;

namespace SimuladorFormal.Turing.Exemplos;

/// <summary>
/// Opção B — soma unária. Entrada 0ⁿ 1 0ᵐ (ex.: 000100 = 3 + 2), saída 0ⁿ⁺ᵐ (00000).
/// Estratégia: troca o separador 1 por 0 e apaga o último 0. Exatamente n + m + 3 passos: Θ(n).
/// </summary>
public static class MaquinaSomaUnaria
{
    private const char B = Fita.Branco;

    public static DefinicaoMaquina Definicao { get; } = new(
        "soma-unaria",
        "Soma unária",
        "Soma dois números em unário separados por 1. Ex.: 000100 representa 3 + 2 e o resultado é 00000.",
        "q0",
        new[] { "qf" },
        new[]
        {
            new Transicao("q0", '0', "q0", '0', Direcao.Direita),
            new Transicao("q0", '1', "q1", '0', Direcao.Direita),
            new Transicao("q1", '0', "q1", '0', Direcao.Direita),
            new Transicao("q1", B, "q2", B, Direcao.Esquerda),
            new Transicao("q2", '0', "qf", B, Direcao.Parado),
        },
        new[] { "000100", "01", "1000", "00001" });
}
