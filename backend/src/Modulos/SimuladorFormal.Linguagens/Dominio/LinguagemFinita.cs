using SimuladorFormal.Compartilhado;
using SimuladorFormal.Linguagens.Operacoes;

namespace SimuladorFormal.Linguagens.Dominio;

/// <summary>Linguagem finita dada por enumeração: "ab, ba, λ" ou "{ab, ba, λ}". "{}" é ∅.</summary>
public sealed class LinguagemFinita : Linguagem
{
    private readonly HashSet<string> _cadeias = new();

    public override TipoLinguagem Tipo => TipoLinguagem.Finita;

    public LinguagemFinita(Alfabeto sigma, string? definicao) : base(sigma, definicao ?? "")
    {
        var limpo = (definicao ?? "").Trim().TrimStart('{').TrimEnd('}');
        if (limpo.Trim().Length == 0) return; // linguagem vazia ∅

        foreach (var parte in limpo.Split(',', StringSplitOptions.TrimEntries))
        {
            var w = Simbolos.NormalizarVazia(parte);
            if (!ValidadorCadeia.Validar(sigma, w).Valida)
                throw new ArgumentException($"A cadeia \"{w}\" de L usa símbolos fora de Σ = {sigma}.");
            _cadeias.Add(w);
        }
    }

    protected override bool ContemCadeiaDeSigmaEstrela(string w) => _cadeias.Contains(w);

    /// <summary>Não precisa gerar Σ*: basta filtrar o próprio conjunto.</summary>
    public override IReadOnlyList<string> Listar(int k) =>
        _cadeias.Where(w => w.Length <= k)
            .OrderBy(w => w.Length).ThenBy(w => w, StringComparer.Ordinal)
            .Select(Simbolos.Exibir)
            .ToList();
}
