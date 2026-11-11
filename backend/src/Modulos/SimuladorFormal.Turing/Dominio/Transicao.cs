namespace SimuladorFormal.Turing.Dominio;

/// <summary>δ(estadoAtual, simboloLido) = (novoEstado, simboloEscrito, movimento).</summary>
public sealed record Transicao(
    string EstadoAtual,
    char SimboloLido,
    string NovoEstado,
    char SimboloEscrito,
    Direcao Movimento)
{
    /// <summary>R, L ou S — forma usada nos livros e na interface.</summary>
    public string MovimentoSigla => Movimento switch
    {
        Direcao.Direita => "R",
        Direcao.Esquerda => "L",
        _ => "S"
    };

    public override string ToString() =>
        $"δ({EstadoAtual}, {SimboloLido}) = ({NovoEstado}, {SimboloEscrito}, {MovimentoSigla})";
}
