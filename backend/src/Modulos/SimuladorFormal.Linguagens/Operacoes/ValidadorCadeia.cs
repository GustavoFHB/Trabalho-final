using SimuladorFormal.Linguagens.Dominio;
using SimuladorFormal.Linguagens.Resultados;

namespace SimuladorFormal.Linguagens.Operacoes;

/// <summary>Verifica se uma cadeia pertence a Σ*.</summary>
public static class ValidadorCadeia
{
    /// <summary>Percorre a cadeia uma vez listando os símbolos fora de Σ — Θ(n).</summary>
    public static ValidacaoCadeia Validar(Alfabeto sigma, string? cadeia)
    {
        cadeia ??= string.Empty;
        var invalidas = new List<int>();
        for (int i = 0; i < cadeia.Length; i++)
            if (!sigma.Contem(cadeia[i]))
                invalidas.Add(i);

        bool valida = invalidas.Count == 0;
        string mensagem = valida
            ? $"Cadeia válida para Σ = {sigma}"
            : $"Cadeia inválida: os símbolos {string.Join(", ", invalidas.Select(i => $"'{cadeia[i]}' (posição {i + 1})"))} não pertencem a Σ = {sigma}";

        return new ValidacaoCadeia(cadeia, valida, OperacoesCadeia.Tamanho(cadeia), invalidas, mensagem);
    }
}
