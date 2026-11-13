using SimuladorFormal.AutomatosFinitos.Contratos;
using SimuladorFormal.AutomatosFinitos.Dominio;
using SimuladorFormal.AutomatosFinitos.Exemplos;
using SimuladorFormal.AutomatosFinitos.Servicos;

namespace SimuladorFormal.Tests.AutomatosFinitos;

public class AutomatosFinitosTests
{
    [Fact]
    public void ExemploDoEnunciado_AceitaAbComCaminho()
    {
        var r = CatalogoAutomatos.Enunciado.Automato.Executar("ab");
        Assert.True(r.Aceita);
        Assert.True(r.Deterministico);
        Assert.Equal("q0 → q1 → q2", r.CaminhoTexto);
        Assert.Equal(2, r.Passos.Count);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("abb")]
    [InlineData("ba")]
    [InlineData("")]
    public void ExemploDoEnunciado_RejeitaOutrasCadeias(string w) =>
        Assert.False(CatalogoAutomatos.Enunciado.Automato.Executar(w).Aceita);

    [Fact]
    public void Afd_TravadoMostraCaminhoAteOndeChegou()
    {
        var r = CatalogoAutomatos.Enunciado.Automato.Executar("abb");
        Assert.Equal("q0 → q1 → q2", r.CaminhoTexto);
        Assert.Contains("δ(q2, b)", r.Mensagem);
    }

    [Theory]
    [InlineData("abba", true)]
    [InlineData("", true)]
    [InlineData("aab", true)]
    [InlineData("babab", true)]
    [InlineData("a", false)]
    [InlineData("bab", false)]
    public void Afd_NumeroParDeA(string w, bool aceita) =>
        Assert.Equal(aceita, CatalogoAutomatos.ParDeA.Automato.Executar(w).Aceita);

    [Theory]
    [InlineData("aab", true)]
    [InlineData("abab", true)]
    [InlineData("ab", true)]
    [InlineData("aba", false)]
    [InlineData("b", false)]
    public void Afn_TerminaEmAb(string w, bool aceita)
    {
        var r = CatalogoAutomatos.TerminaEmAb.Automato.Executar(w);
        Assert.False(r.Deterministico);
        Assert.Equal(aceita, r.Aceita);
        if (aceita) Assert.EndsWith("q1 → q2", r.CaminhoTexto);
    }

    [Fact]
    public void Afn_MantemConjuntoDeEstados()
    {
        var r = CatalogoAutomatos.TerminaEmAb.Automato.Executar("a");
        Assert.Equal(new[] { "q0", "q1" }, r.Passos[0].EstadosDepois);
    }

    [Theory]
    [InlineData("aaa", true)]
    [InlineData("bb", true)]
    [InlineData("", true)]
    [InlineData("ab", false)]
    public void AfnLambda_FechoLambda(string w, bool aceita)
    {
        var r = CatalogoAutomatos.AEstrelaOuBEstrela.Automato.Executar(w);
        Assert.Equal(new[] { "q0", "q1", "q2" }, r.EstadosIniciais);
        Assert.Equal(aceita, r.Aceita);
    }

    [Fact]
    public void AfnLambda_CaminhoIncluiTransicaoVazia()
    {
        var r = CatalogoAutomatos.AEstrelaOuBEstrela.Automato.Executar("bb");
        Assert.Equal("q0 → q2 → q2 → q2", r.CaminhoTexto);
        Assert.Equal("λ", r.Caminho![1].Simbolo);
    }

    [Fact]
    public void SimboloForaDoAlfabeto_Rejeita()
    {
        var r = CatalogoAutomatos.ParDeA.Automato.Executar("abc");
        Assert.False(r.Aceita);
        Assert.Contains("não pertence a Σ", r.Mensagem);
    }

    [Fact]
    public void EstadoForaDeQ_GeraErro() =>
        Assert.Throws<ArgumentException>(() => new AutomatoFinito(
            new[] { "q0" }, new[] { 'a' }, "q0", new[] { "q0" }, new[] { new TransicaoAf("q0", 'a', "q9") }));

    [Fact]
    public void Servico_CarregaJsonEExecuta()
    {
        var json = new AutomatoDto(
            null, null, "q0", new List<string> { "q2" },
            new List<TransicaoAfDto> { new("q0", "a", "q1"), new("q1", "b", "q2") });
        var r = new SimuladorAutomatoService().Simular(new SimulacaoAutomatoRequest(json, "ab"));
        Assert.True(r.Aceita);
        Assert.Equal("q0 → q1 → q2", r.CaminhoTexto);
    }

    [Fact]
    public void Servico_ExemplosVoltamEmJson()
    {
        var exemplos = new SimuladorAutomatoService().ListarExemplos();
        var lambda = exemplos.Single(e => e.Id == "a-ou-b");
        Assert.Contains(lambda.Automato.Transicoes!, t => t.Simbolo == "λ");
    }

    [Fact]
    public void Servico_SimboloComMaisDeUmCaractere_GeraErro() =>
        Assert.Throws<ArgumentException>(() => new SimuladorAutomatoService().Simular(new SimulacaoAutomatoRequest(
            new AutomatoDto(null, null, "q0", null, new List<TransicaoAfDto> { new("q0", "ab", "q1") }), "ab")));
}
