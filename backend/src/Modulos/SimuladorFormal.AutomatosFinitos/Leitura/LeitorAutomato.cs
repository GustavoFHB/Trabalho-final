using SimuladorFormal.AutomatosFinitos.Contratos;
using SimuladorFormal.AutomatosFinitos.Dominio;
using SimuladorFormal.Compartilhado;

namespace SimuladorFormal.AutomatosFinitos.Leitura;

/// <summary>Converte o JSON recebido (<see cref="AutomatoDto"/>) em <see cref="AutomatoFinito"/> e vice-versa.</summary>
public static class LeitorAutomato
{
    public static AutomatoFinito Ler(AutomatoDto? dto)
    {
        if (dto is null)
            throw new ArgumentException("Informe o autômato em JSON.");

        var transicoes = (dto.Transicoes ?? new List<TransicaoAfDto>())
            .Select((t, i) => LerTransicao(t, i + 1))
            .ToList();

        var alfabeto = (dto.Alfabeto ?? new List<string>()).Select(s =>
        {
            if (Simbolos.RepresentaVazia(s))
                throw new ArgumentException("λ não pode fazer parte de Σ: ele representa a cadeia vazia.");
            return Simbolo(s, $"O símbolo \"{s}\" de Σ");
        });

        return new AutomatoFinito(
            dto.Estados ?? new List<string>(),
            alfabeto.ToList(),
            dto.EstadoInicial,
            dto.EstadosFinais ?? new List<string>(),
            transicoes);
    }

    public static AutomatoDto ParaDto(AutomatoFinito a) => new(
        a.Estados.ToList(),
        a.Alfabeto.Select(c => c.ToString()).ToList(),
        a.EstadoInicial,
        a.Estados.Where(a.EhFinal).ToList(),
        a.Transicoes.Select(t => new TransicaoAfDto(t.Origem, t.SimboloTexto, t.Destino)).ToList());

    private static TransicaoAf LerTransicao(TransicaoAfDto t, int linha)
    {
        var prefixo = $"Transição {linha}: ";
        char? simbolo = Simbolos.RepresentaVazia(t.Simbolo) ? null : Simbolo(t.Simbolo, prefixo + "o símbolo");
        return new TransicaoAf(
            Obrigatorio(t.Origem, prefixo + "origem"),
            simbolo,
            Obrigatorio(t.Destino, prefixo + "destino"));
    }

    private static string Obrigatorio(string? valor, string campo) =>
        string.IsNullOrWhiteSpace(valor) ? throw new ArgumentException($"{campo} é obrigatório(a).") : valor.Trim();

    private static char Simbolo(string? valor, string campo) =>
        valor?.Trim() is { Length: 1 } s ? s[0] : throw new ArgumentException($"{campo} deve ter exatamente 1 caractere.");
}
