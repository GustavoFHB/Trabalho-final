using SimuladorFormal.Turing.Dominio;

namespace SimuladorFormal.Turing.Exemplos;

/// <summary>
/// Opção A — reconhece L = { w#w | w ∈ {0,1,a,b}* }.
/// Estratégia em "zigue-zague": marca o 1º símbolo não marcado à esquerda de #, guarda-o no estado,
/// atravessa o # e compara com o 1º não marcado à direita. Θ(n²) passos.
/// </summary>
public static class MaquinaWCercaW
{
    private const char B = Fita.Branco;
    private const char Marca = 'X';
    private static readonly char[] Sigma = { '0', '1', 'a', 'b' };

    public static DefinicaoMaquina Definicao { get; } = new(
        "w-cerca-w",
        "Reconhecedor de w#w",
        "Aceita cadeias da forma w#w sobre {0, 1, a, b}: a parte antes de # é igual à parte depois.",
        "q0",
        new[] { "qaceita" },
        GerarTransicoes(),
        new[] { "ab#ab", "0110#0110", "ab#ba", "#", "abb#ab" });

    private static List<Transicao> GerarTransicoes()
    {
        var t = new List<Transicao>();
        foreach (var s in Sigma)
        {
            var levando = $"q_{s}";
            var comparando = $"qc_{s}";

            t.Add(new Transicao("q0", s, levando, Marca, Direcao.Direita));
            foreach (var y in Sigma)
                t.Add(new Transicao(levando, y, levando, y, Direcao.Direita));
            t.Add(new Transicao(levando, '#', comparando, '#', Direcao.Direita));
            t.Add(new Transicao(comparando, Marca, comparando, Marca, Direcao.Direita));
            t.Add(new Transicao(comparando, s, "qvolta", Marca, Direcao.Esquerda));
        }

        foreach (var y in Sigma.Append(Marca))
            t.Add(new Transicao("qvolta", y, "qvolta", y, Direcao.Esquerda));
        t.Add(new Transicao("qvolta", '#', "qvolta2", '#', Direcao.Esquerda));
        foreach (var y in Sigma)
            t.Add(new Transicao("qvolta2", y, "qvolta2", y, Direcao.Esquerda));
        t.Add(new Transicao("qvolta2", Marca, "q0", Marca, Direcao.Direita));

        t.Add(new Transicao("q0", '#', "qverifica", '#', Direcao.Direita));
        t.Add(new Transicao("qverifica", Marca, "qverifica", Marca, Direcao.Direita));
        t.Add(new Transicao("qverifica", B, "qaceita", B, Direcao.Parado));
        return t;
    }
}
