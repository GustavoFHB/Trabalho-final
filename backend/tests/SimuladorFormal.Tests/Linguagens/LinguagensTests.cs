using SimuladorFormal.Linguagens.Contratos;
using SimuladorFormal.Linguagens.Dominio;
using SimuladorFormal.Linguagens.Operacoes;
using SimuladorFormal.Linguagens.Servicos;

namespace SimuladorFormal.Tests.Linguagens;

public class LinguagensTests
{
    private readonly Alfabeto _ab = Alfabeto.Parse("{a,b}");

    [Fact]
    public void ExemploDoEnunciado_abba_EhValidaComTamanho4()
    {
        var r = ValidadorCadeia.Validar(_ab, "abba");
        Assert.True(r.Valida);
        Assert.Equal(4, r.Tamanho);
        Assert.Equal("Cadeia válida para Σ = {a, b}", r.Mensagem);
    }

    [Fact]
    public void CadeiaComSimboloForaDoAlfabeto_EhInvalida()
    {
        var r = ValidadorCadeia.Validar(_ab, "abca");
        Assert.False(r.Valida);
        Assert.Equal(new[] { 2 }, r.PosicoesInvalidas);
    }

    [Fact]
    public void FechoDeKleene_IncluiLambda_EmOrdemCanonica()
    {
        var f = GeradorFecho.FechoKleene(_ab, 2);
        Assert.Equal(new[] { "λ", "a", "b", "aa", "ab", "ba", "bb" }, f.Cadeias);
        Assert.Equal(7, f.TotalTeorico);
    }

    [Fact]
    public void FechoPositivo_NaoIncluiLambda()
    {
        var f = GeradorFecho.FechoPositivo(_ab, 2);
        Assert.DoesNotContain("λ", f.Cadeias);
        Assert.Equal(6, f.Cadeias.Count);
    }

    [Fact]
    public void ConcatenacaoTamanhoEReverso()
    {
        Assert.Equal("abba", OperacoesCadeia.Concatenar("ab", "ba"));
        Assert.Equal(0, OperacoesCadeia.Tamanho(""));
        Assert.Equal("ba", OperacoesCadeia.Reverso("ab"));
        Assert.Equal("ababab", OperacoesCadeia.Potencia("ab", 3));
    }

    [Fact]
    public void LinguagemFinita_Pertinencia()
    {
        var L = new LinguagemFinita(_ab, "{ab, ba, λ}");
        Assert.True(L.Pertence("ab"));
        Assert.True(L.Pertence(""));
        Assert.False(L.Pertence("aa"));
    }

    [Fact]
    public void LinguagemRegular_ListaCadeiasAteK()
    {
        var L = new LinguagemRegular(_ab, "a*b");
        Assert.Equal(new[] { "b", "ab", "aab" }, L.Listar(3));
    }

    [Fact]
    public void Fabrica_CriaOTipoCerto()
    {
        Assert.IsType<LinguagemRegular>(FabricaLinguagem.Criar(_ab, "regular", "a*"));
        Assert.IsType<LinguagemFinita>(FabricaLinguagem.Criar(_ab, "finita", "a"));
        Assert.Throws<ArgumentException>(() => FabricaLinguagem.Criar(_ab, "outra", "a"));
    }

    [Fact]
    public void Servico_ReuneTodasAsOperacoes()
    {
        var r = new AnaliseLinguagemService().Analisar(
            new AnaliseLinguagemRequest("a, b", "abba", "ab", 2, "regular", "(a|b)*a"));
        Assert.True(r.Cadeia.Valida);
        Assert.Equal("abbaab", r.Concatenacao.Resultado);
        Assert.True(r.Linguagem!.CadeiaPertence);
        Assert.False(r.Linguagem.ConcatenacaoPertence);
    }
}
