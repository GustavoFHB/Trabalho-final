using SimuladorFormal.Turing.Execucao;

namespace SimuladorFormal.Turing.Dominio;

/// <summary>Máquina de Turing determinística de uma fita: M = (Q, Σ, Γ, δ, q0, B, F).</summary>
public sealed class MaquinaTuring
{
    public const int LimitePassosPadrao = 1000;
    public const int LimitePassosMaximo = 100_000;

    private readonly HashSet<string> _estadosFinais;

    public string EstadoInicial { get; }
    public IReadOnlySet<string> EstadosFinais => _estadosFinais;
    public FuncaoTransicao Delta { get; }

    public MaquinaTuring(string estadoInicial, IEnumerable<string> estadosFinais, IEnumerable<Transicao> transicoes)
    {
        if (string.IsNullOrWhiteSpace(estadoInicial))
            throw new ArgumentException("Estado inicial é obrigatório.");

        EstadoInicial = estadoInicial.Trim();
        _estadosFinais = estadosFinais.Select(e => e.Trim()).Where(e => e.Length > 0).ToHashSet();
        if (_estadosFinais.Count == 0)
            throw new ArgumentException("Informe pelo menos um estado final.");

        Delta = new FuncaoTransicao(transicoes);
    }

    public bool EhFinal(string estado) => _estadosFinais.Contains(estado);

    /// <summary>
    /// Executa até chegar a um estado final (aceita), ficar sem transição (rejeita)
    /// ou atingir o limite de passos (a parada é indecidível em geral).
    /// </summary>
    public ResultadoSimulacao Executar(string? entrada, int limitePassos = LimitePassosPadrao)
    {
        if (limitePassos < 1 || limitePassos > LimitePassosMaximo)
            throw new ArgumentException($"O limite de passos deve estar entre 1 e {LimitePassosMaximo}.");

        var fita = new Fita(entrada);
        var estado = EstadoInicial;
        var passos = new List<Passo>();
        string motivo;

        while (true)
        {
            if (EhFinal(estado))
            {
                motivo = $"Estado final {estado} alcançado.";
                break;
            }
            if (passos.Count >= limitePassos)
            {
                motivo = $"Limite de {limitePassos} passos atingido (possível loop infinito).";
                break;
            }

            char simbolo = fita.Ler();
            if (!Delta.TentarObter(estado, simbolo, out var t))
            {
                motivo = $"A máquina parou: não há transição para ({estado}, {simbolo}).";
                break;
            }

            passos.Add(new Passo(passos.Count, estado, simbolo, fita.Conteudo(), fita.Cabecote, t));

            fita.Escrever(t.SimboloEscrito);
            fita.Mover(t.Movimento);
            estado = t.NovoEstado;
        }

        return new ResultadoSimulacao(EhFinal(estado), motivo, estado, fita.Conteudo(), fita.Cabecote,
            fita.ConteudoUtil(), passos);
    }
}
