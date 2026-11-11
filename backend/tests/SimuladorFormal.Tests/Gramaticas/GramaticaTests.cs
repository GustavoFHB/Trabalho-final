using SimuladorFormal.Gramaticas.Analise;
using SimuladorFormal.Gramaticas.Derivacao;
using SimuladorFormal.Gramaticas.Leitura;
using SimuladorFormal.Gramaticas.Servicos;

namespace SimuladorFormal.Tests.Gramaticas;

public class GramaticaTests
{
    private static ResultadoDerivacao Derivar(string producoes, string cadeia) =>
        new DerivacaoService().Derivar(LeitorGramatica.Ler(producoes), cadeia);

    [Fact]
    public void ExemploDoEnunciado_aaabbb_EhAceita()
    {
        var r = Derivar("S -> aSb | λ", "aaabbb");
        Assert.True(r.Aceita);
        Assert.Equal(new[] { "S", "aSb", "aaSbb", "aaaSbbb", "aaabbb" }, r.Passos.Select(p => p.FormaSentencial));
        Assert.Equal("S → λ", r.Passos[^1].Producao);
    }

    [Theory]
    [InlineData("aab")]
    [InlineData("ba")]
    [InlineData("abab")]
    public void CadeiasForaDeAnBn_SaoRejeitadas(string w) => Assert.False(Derivar("S -> aSb | λ", w).Aceita);

    [Fact]
    public void CadeiaVazia_EhAceitaQuandoSDerivaLambda() => Assert.True(Derivar("S -> aSb | λ", "").Aceita);

    [Fact]
    public void GramaticaAmbiguaERecursivaAEsquerda_Funciona()
    {
        var r = Derivar("E -> E+E | E*E | (E) | x", "x+x*(x+x)");
        Assert.True(r.Aceita);
        Assert.Equal("x+x*(x+x)", r.Passos[^1].FormaSentencial);
    }

    [Fact]
    public void VariaveisAnulaveisEncadeadas()
    {
        const string g = "S -> AB\nA -> aA | λ\nB -> bB | λ";
        var r = Derivar(g, "aab");
        Assert.True(r.Aceita);
        Assert.Equal("aab", r.Passos[^1].FormaSentencial);
        Assert.True(Derivar(g, "").Aceita);
        Assert.False(Derivar(g, "ba").Aceita);
    }

    [Fact]
    public void Anulaveis_CalculadasPorPontoFixo()
    {
        var anulaveis = VariaveisAnulaveis.Calcular(LeitorGramatica.Ler("S -> AB | c\nA -> λ\nB -> A"));
        Assert.Equal(new[] { 'A', 'B', 'S' }, anulaveis.Variaveis.OrderBy(c => c));
    }

    [Fact]
    public void ArvoreTemFolhasIguaisACadeia()
    {
        var r = Derivar("S -> aSb | ab", "aabb");
        static IEnumerable<string> Folhas(NoArvore n) =>
            n.EhFolha ? new[] { n.Simbolo } : n.Filhos.SelectMany(Folhas);
        Assert.Equal("aabb", string.Concat(Folhas(r.Arvore!)));
    }

    [Fact]
    public void SimboloNaoDeclarado_GeraErro() =>
        Assert.Throws<ArgumentException>(() => LeitorGramatica.Ler("S -> aSb", "S", "a"));

    [Fact]
    public void LinhaSemSeta_GeraErro() =>
        Assert.Throws<ArgumentException>(() => LeitorGramatica.Ler("S aSb"));
}
