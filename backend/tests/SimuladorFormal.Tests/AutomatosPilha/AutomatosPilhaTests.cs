using SimuladorFormal.AutomatosPilha.Contratos;
using SimuladorFormal.AutomatosPilha.Dominio;
using SimuladorFormal.AutomatosPilha.Exemplos;
using SimuladorFormal.AutomatosPilha.Servicos;

namespace SimuladorFormal.Tests.AutomatosPilha;

public class AutomatosPilhaTests
{
    [Theory]
    [InlineData("ab", true)]
    [InlineData("aaabbb", true)]
    [InlineData("", false)]       // n ≥ 1
    [InlineData("aabbb", false)]
    [InlineData("aaabb", false)]
    [InlineData("abab", false)]
    [InlineData("ba", false)]
    public void AnBn(string w, bool aceita)
    {
        var r = CatalogoAutomatosPilha.AnBn.Automato.Executar(w);
        Assert.True(r.Deterministico);
        Assert.Equal(aceita, r.Aceita);
    }

    [Fact]
    public void AnBn_MostraLeituraEstadoEPilha()
    {
        var r = CatalogoAutomatosPilha.AnBn.Automato.Executar("aabb");
        Assert.Equal(5, r.Passos.Count); // 4 leituras + o movimento λ final
        Assert.Equal(new[] { "Z", "AZ", "AAZ", "AZ", "Z" }, r.Passos.Select(p => p.Pilha));
        Assert.Equal(new[] { "a", "a", "b", "b", "λ" }, r.Passos.Select(p => p.Leitura));
        Assert.Equal("q2", r.EstadoFinal);
        Assert.Equal("desempilha A", r.Passos[2].Operacao);
    }

    [Theory]
    [InlineData("abba", true)]
    [InlineData("aabbaa", true)]
    [InlineData("", true)]
    [InlineData("abab", false)]
    [InlineData("aba", false)]
    public void Palindromos_NaoDeterministico(string w, bool aceita)
    {
        var r = CatalogoAutomatosPilha.Palindromos.Automato.Executar(w);
        Assert.False(r.Deterministico);
        Assert.Equal(aceita, r.Aceita);
    }

    [Theory]
    [InlineData("(()())", true)]
    [InlineData("", true)]
    [InlineData("(()", false)]
    [InlineData("())(", false)]
    public void Parenteses_PorPilhaVazia(string w, bool aceita)
    {
        var r = CatalogoAutomatosPilha.Parenteses.Automato.Executar(w);
        Assert.Equal(aceita, r.Aceita);
        if (aceita) Assert.Equal("", r.PilhaFinal);
    }

    [Fact]
    public void Pilha_EmpilhaComTopoAEsquerda()
    {
        var p = new Pilha("Z").Empilhar("AB");
        Assert.Equal('A', p.Topo);
        Assert.Equal("BZ", p.Desempilhar().Conteudo);
        Assert.Throws<InvalidOperationException>(() => Pilha.Vazia.Desempilhar());
    }

    [Fact]
    public void LacoLambdaQueEmpilha_EhInterrompido()
    {
        var ap = new AutomatoPilha("q0", new[] { "qf" }, 'Z', CriterioAceitacao.EstadoFinal,
            new[] { new TransicaoPilha("q0", null, null, "q0", "A") });
        var r = ap.Executar("a");
        Assert.False(r.Aceita);
        Assert.Contains("descartado", r.Mensagem);
    }

    [Fact]
    public void Servico_ConverteRequisicaoEExecuta()
    {
        var servico = new SimuladorPilhaService();
        var anbn = servico.ListarExemplos().Single(e => e.Id == "anbn");
        var r = servico.Simular(new SimulacaoPilhaRequest(
            "aaabbb", anbn.EstadoInicial, string.Join(",", anbn.EstadosFinais), anbn.SimboloInicialPilha,
            anbn.Criterio, anbn.Transicoes.ToList(), null));
        Assert.True(r.Aceita);
    }

    [Fact]
    public void Servico_SimboloInvalido_GeraErro() =>
        Assert.Throws<ArgumentException>(() => new SimuladorPilhaService().Simular(new SimulacaoPilhaRequest(
            "a", "q0", "qf", "Z", null, new List<TransicaoPilhaDto> { new("q0", "ab", "Z", "qf", "Z") }, null)));
}
