using System.Text;

namespace SimuladorFormal.Turing.Dominio;

/// <summary>
/// Fita infinita nos dois sentidos, simulada com uma lista que cresce sob demanda.
/// A entrada é cercada por brancos (B) e o cabeçote começa no primeiro símbolo da entrada.
/// </summary>
public sealed class Fita
{
    public const char Branco = 'B';

    private readonly List<char> _celulas = new();

    public int Cabecote { get; private set; }

    public Fita(string? entrada)
    {
        _celulas.Add(Branco);
        _celulas.AddRange(entrada ?? "");
        _celulas.Add(Branco);
        Cabecote = 1;
    }

    public char Ler() => _celulas[Cabecote];

    public void Escrever(char simbolo) => _celulas[Cabecote] = simbolo;

    public void Mover(Direcao direcao)
    {
        switch (direcao)
        {
            case Direcao.Direita:
                Cabecote++;
                if (Cabecote >= _celulas.Count) _celulas.Add(Branco);
                break;
            case Direcao.Esquerda:
                Cabecote--;
                if (Cabecote < 0)
                {
                    _celulas.Insert(0, Branco);
                    Cabecote = 0;
                }
                break;
        }
    }

    public string Conteudo() => new(_celulas.ToArray());

    /// <summary>Conteúdo sem os brancos das pontas — o "resultado" deixado na fita.</summary>
    public string ConteudoUtil() => Conteudo().Trim(Branco);

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < _celulas.Count; i++)
            sb.Append(i == Cabecote ? $"[{_celulas[i]}]" : _celulas[i].ToString());
        return sb.ToString();
    }
}
