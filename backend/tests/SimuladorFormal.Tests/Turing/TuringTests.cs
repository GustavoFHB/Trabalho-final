using SimuladorFormal.Turing.Contratos;
using SimuladorFormal.Turing.Dominio;
using SimuladorFormal.Turing.Exemplos;
using SimuladorFormal.Turing.Servicos;

namespace SimuladorFormal.Tests.Turing;

public class TuringTests
{
    [Theory]
    [InlineData("000100", "00000")] // 3 + 2 (exemplo do enunciado)
    [InlineData("01", "0")]         // 1 + 0
    [InlineData("1000", "000")]     // 0 + 3
    [InlineData("1", "")]           // 0 + 0
    public void SomaUnaria(string entrada, string esperado)
    {
        var r = MaquinaSomaUnaria.Definicao.Criar().Executar(entrada);
        Assert.True(r.Aceita);
        Assert.Equal(esperado, r.Saida);
    }

    [Theory]
    [InlineData("ab#ab", true)]
    [InlineData("0110#0110", true)]
    [InlineData("#", true)]
    [InlineData("ab#ba", false)]
    [InlineData("abb#ab", false)]
    [InlineData("ab#abb", false)]
    [InlineData("abab", false)]
    public void ReconhecedorWCercaW(string entrada, bool aceita)
    {
        var r = MaquinaWCercaW.Definicao.Criar().Executar(entrada, 10_000);
        Assert.Equal(aceita, r.Aceita);
    }

    [Fact]
    public void LimiteDePassos_InterrompeLoopInfinito()
    {
        var mt = new MaquinaTuring("q0", new[] { "qf" }, new[] { new Transicao("q0", 'B', "q0", 'B', Direcao.Direita) });
        var r = mt.Executar("", 50);
        Assert.False(r.Aceita);
        Assert.Equal(50, r.Passos.Count);
    }

    [Fact]
    public void TransicoesDuplicadas_SaoRejeitadas() =>
        Assert.Throws<ArgumentException>(() => new FuncaoTransicao(new[]
        {
            new Transicao("q0", '0', "q1", '0', Direcao.Direita),
            new Transicao("q0", '0', "q2", '1', Direcao.Direita),
        }));

    [Fact]
    public void Fita_CresceParaAEsquerda()
    {
        var f = new Fita("0");
        f.Mover(Direcao.Esquerda);
        f.Mover(Direcao.Esquerda);
        Assert.Equal(0, f.Cabecote);
        Assert.Equal("BB0B", f.Conteudo());
    }

    [Fact]
    public void Servico_ConverteRequisicaoEExecuta()
    {
        var servico = new SimuladorTuringService();
        var soma = servico.ListarExemplos().Single(m => m.Id == "soma-unaria");
        var r = servico.Simular(new SimulacaoRequest("000100", soma.EstadoInicial, "qf", soma.Transicoes.ToList(), null));
        Assert.Equal("00000", r.Saida);
    }

    [Fact]
    public void Servico_DirecaoInvalida_GeraErro() =>
        Assert.Throws<ArgumentException>(() => new SimuladorTuringService().Simular(new SimulacaoRequest(
            "0", "q0", "qf", new List<TransicaoDto> { new("q0", "0", "qf", "0", "X") }, null)));
}
