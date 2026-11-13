using SimuladorFormal.Automatos;

namespace SimuladorFormal.Tests.Automatos;

public class AutomatosTests
{
    private static readonly DefinicaoAF Afd = new("AFD", new[] { "q0", "q1", "q2" },
        new[] { "a", "b" }, "q0", new[] { "q2" },
        new RegraAF[] { new("q0", "a", "q1"), new("q1", "b", "q2") });

    [Fact]
    public void AfdRegistraCaminhoAteFinal()
    {
        var r = AutomatoFinito.Executar(new(Afd, "ab"));
        Assert.True(r.Aceita);
        Assert.Equal(new[] { "q0", "q1", "q2" }, r.Passos.Select(p => p.EstadosAtivos.Single()));
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("aba")]
    [InlineData("x")]
    public void AfdRejeitaCadeiasForaDaLinguagem(string w) =>
        Assert.False(AutomatoFinito.Executar(new(Afd, w)).Aceita);

    [Fact]
    public void AfdRejeitaConfiguracaoNaoDeterministica()
    {
        var ambigua = Afd with { Transicoes = Afd.Transicoes.Append(new("q0", "a", "q2")).ToArray() };
        Assert.Throws<ArgumentException>(() => AutomatoFinito.Executar(new(ambigua, "ab")));
    }

    [Fact]
    public void AfnSegueRamosEFechaCiclosLambdaSemConsumirEntrada()
    {
        var afn = Afd with { Tipo = "AFN", Transicoes = new RegraAF[] {
            new("q0", "λ", "q1"), new("q1", "ε", "q0"),
            new("q1", "a", "q1"), new("q1", "a", "q2")
        }};
        var r = AutomatoFinito.Executar(new(afn, "a"));
        Assert.True(r.Aceita);
        Assert.Equal(2, r.Passos[0].EstadosAtivos.Length);
        Assert.Equal(3, r.Passos[1].EstadosAtivos.Length);
        Assert.False(AutomatoFinito.Executar(new(afn, "")).Aceita);
        Assert.True(AutomatoFinito.Executar(new(afn with { EstadosFinais = new[] { "q1" } }, "λ")).Aceita);
    }

    [Theory]
    [InlineData("ab", true)]
    [InlineData("aaabbb", true)]
    [InlineData("λ", false)]
    [InlineData("b", false)]
    [InlineData("aaa", false)]
    [InlineData("aab", false)]
    [InlineData("abb", false)]
    [InlineData("abab", false)]
    [InlineData("aabbx", false)]
    public void PilhaDecideAnBn(string w, bool aceita) =>
        Assert.Equal(aceita, AutomatoPilha.Executar(new(w)).Aceita);

    [Fact]
    public void PilhaRegistraEmpilhamentoEDesempilhamento()
    {
        var r = AutomatoPilha.Executar(new("aabb"));
        Assert.Equal(new[] { "", "A", "AA", "A", "", "" }, r.Passos.Select(p => p.Pilha));
        Assert.Equal("qf", r.Passos.Last().Estado);
    }
}
