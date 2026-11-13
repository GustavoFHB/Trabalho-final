using SimuladorFormal.AutomatosPilha.Contratos;
using SimuladorFormal.AutomatosPilha.Dominio;
using SimuladorFormal.Compartilhado;

namespace SimuladorFormal.AutomatosPilha.Leitura;

/// <summary>Converte e valida as transições digitadas na interface.</summary>
public static class LeitorTransicaoPilha
{
    public static TransicaoPilha Ler(TransicaoPilhaDto dto, int linha)
    {
        var prefixo = $"Transição {linha}: ";
        return new TransicaoPilha(
            Obrigatorio(dto.Origem, prefixo + "estado de origem"),
            SimboloOuLambda(dto.Simbolo, prefixo + "o símbolo lido"),
            SimboloOuLambda(dto.Desempilha, prefixo + "o símbolo desempilhado"),
            Obrigatorio(dto.Destino, prefixo + "estado de destino"),
            Simbolos.NormalizarVazia(dto.Empilha).Replace(" ", ""));
    }

    public static TransicaoPilhaDto ParaDto(TransicaoPilha t) =>
        new(t.Origem, t.SimboloTexto, t.DesempilhaTexto, t.Destino, t.EmpilhaTexto);

    /// <summary>Lê Z0: um caractere, ou vazio/λ para começar com a pilha vazia.</summary>
    public static char? LerSimboloInicial(string? valor) =>
        SimboloOuLambda(valor, "O símbolo inicial da pilha");

    private static char? SimboloOuLambda(string? valor, string campo)
    {
        if (Simbolos.RepresentaVazia(valor)) return null;
        var s = valor!.Trim();
        return s.Length == 1 ? s[0] : throw new ArgumentException($"{campo} deve ter 1 caractere (ou λ).");
    }

    private static string Obrigatorio(string? valor, string campo) =>
        string.IsNullOrWhiteSpace(valor) ? throw new ArgumentException($"{campo} é obrigatório.") : valor.Trim();
}
