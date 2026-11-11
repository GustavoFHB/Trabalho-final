using System.Text.RegularExpressions;

namespace SimuladorFormal.Linguagens.Dominio;

/// <summary>Linguagem descrita por expressão regular: (a|b)*abb, a*b+, a|λ…</summary>
public sealed class LinguagemRegular : Linguagem
{
    private static readonly TimeSpan TempoMaximo = TimeSpan.FromSeconds(1);

    private readonly Regex _regex;

    public override TipoLinguagem Tipo => TipoLinguagem.Regular;

    public LinguagemRegular(Alfabeto sigma, string? expressao) : base(sigma, expressao ?? "")
    {
        if (string.IsNullOrWhiteSpace(expressao))
            throw new ArgumentException("Informe a expressão regular que define L.");
        try
        {
            // λ/ε representam a cadeia vazia: "a|λ" vira "a|", que também casa com "".
            var normalizada = expressao.Replace(" ", "").Replace("λ", "").Replace("ε", "");
            _regex = new Regex($"^(?:{normalizada})$", RegexOptions.CultureInvariant, TempoMaximo);
        }
        catch (ArgumentException e)
        {
            throw new ArgumentException($"Expressão regular inválida: {e.Message}");
        }
    }

    protected override bool ContemCadeiaDeSigmaEstrela(string w) => _regex.IsMatch(w);
}
