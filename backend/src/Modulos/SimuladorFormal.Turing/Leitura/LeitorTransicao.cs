using SimuladorFormal.Turing.Contratos;
using SimuladorFormal.Turing.Dominio;

namespace SimuladorFormal.Turing.Leitura;

/// <summary>Converte e valida as transições digitadas na interface (o que o SimuladorController fazia no Java).</summary>
public static class LeitorTransicao
{
    public static Transicao Ler(TransicaoDto dto, int linha)
    {
        var prefixo = $"Transição {linha}: ";
        return new Transicao(
            Obrigatorio(dto.EstadoAtual, prefixo + "estado atual"),
            Simbolo(dto.SimboloLido, prefixo + "símbolo lido"),
            Obrigatorio(dto.NovoEstado, prefixo + "novo estado"),
            Simbolo(dto.SimboloEscrito, prefixo + "símbolo escrito"),
            LerDirecao(dto.Direcao, prefixo + "direção"));
    }

    public static TransicaoDto ParaDto(Transicao t) =>
        new(t.EstadoAtual, t.SimboloLido.ToString(), t.NovoEstado, t.SimboloEscrito.ToString(), t.MovimentoSigla);

    /// <summary>Aceita R/L/S e também D (direita), E (esquerda) e P (parado).</summary>
    public static Direcao LerDirecao(string? valor, string campo = "Direção") =>
        (valor ?? "").Trim().ToUpperInvariant() switch
        {
            "R" or "D" => Direcao.Direita,
            "L" or "E" => Direcao.Esquerda,
            "S" or "P" => Direcao.Parado,
            _ => throw new ArgumentException($"{campo} deve ser R (direita), L (esquerda) ou S (parado).")
        };

    private static string Obrigatorio(string? valor, string campo) =>
        string.IsNullOrWhiteSpace(valor) ? throw new ArgumentException($"{campo} é obrigatório.") : valor.Trim();

    private static char Simbolo(string? valor, string campo) =>
        valor?.Trim() is { Length: 1 } s ? s[0] : throw new ArgumentException($"{campo} deve ter exatamente 1 caractere.");
}
