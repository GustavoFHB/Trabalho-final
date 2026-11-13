using SimuladorFormal.AutomatosPilha.Execucao;
using SimuladorFormal.Compartilhado;

namespace SimuladorFormal.AutomatosPilha.Dominio;

/// <summary>
/// Autômato com pilha M = (Q, Σ, Γ, δ, q0, Z0, F), possivelmente não determinístico.
/// A execução é uma busca em largura sobre as configurações (estado, posição na entrada, pilha):
/// no caso determinístico há uma única configuração por vez; no não determinístico, todos os ramos
/// são explorados em paralelo, até encontrar um que aceite.
/// </summary>
public sealed class AutomatoPilha
{
    public const int LimiteConfiguracoesPadrao = 10_000;
    public const int LimiteConfiguracoesMaximo = 200_000;

    /// <summary>Ramos que passam desta altura são descartados (evita laços λ que empilham para sempre).</summary>
    public const int AlturaMaximaPilha = 500;

    private readonly HashSet<string> _finais;
    private readonly Dictionary<string, List<TransicaoPilha>> _porEstado;

    public string EstadoInicial { get; }
    public IReadOnlySet<string> EstadosFinais => _finais;
    public char? SimboloInicialPilha { get; }
    public CriterioAceitacao Criterio { get; }
    public IReadOnlyList<TransicaoPilha> Transicoes { get; }

    /// <summary>Verdadeiro se nenhuma configuração admite duas transições ao mesmo tempo (APD).</summary>
    public bool Deterministico { get; }

    public AutomatoPilha(
        string estadoInicial,
        IEnumerable<string> estadosFinais,
        char? simboloInicialPilha,
        CriterioAceitacao criterio,
        IEnumerable<TransicaoPilha> transicoes)
    {
        if (string.IsNullOrWhiteSpace(estadoInicial))
            throw new ArgumentException("Estado inicial é obrigatório.");

        EstadoInicial = estadoInicial.Trim();
        _finais = estadosFinais.Select(e => e.Trim()).Where(e => e.Length > 0).ToHashSet();
        if (criterio == CriterioAceitacao.EstadoFinal && _finais.Count == 0)
            throw new ArgumentException("Com aceitação por estado final, informe pelo menos um estado final.");

        SimboloInicialPilha = simboloInicialPilha;
        Criterio = criterio;
        Transicoes = transicoes.Distinct().ToList();
        if (Transicoes.Count == 0)
            throw new ArgumentException("Cadastre pelo menos uma transição.");

        _porEstado = Transicoes.GroupBy(t => t.Origem).ToDictionary(g => g.Key, g => g.ToList());
        Deterministico = _porEstado.Values.All(SemConflito);
    }

    /// <summary>
    /// Duas transições do mesmo estado conflitam se podem valer na mesma configuração:
    /// leem o mesmo símbolo (ou uma delas é λ) e exigem o mesmo topo (ou uma delas não olha a pilha).
    /// </summary>
    private static bool SemConflito(List<TransicaoPilha> ts)
    {
        for (int i = 0; i < ts.Count; i++)
            for (int j = i + 1; j < ts.Count; j++)
            {
                bool mesmaLeitura = ts[i].Simbolo is null || ts[j].Simbolo is null || ts[i].Simbolo == ts[j].Simbolo;
                bool mesmoTopo = ts[i].Desempilha is null || ts[j].Desempilha is null || ts[i].Desempilha == ts[j].Desempilha;
                if (mesmaLeitura && mesmoTopo) return false;
            }
        return true;
    }

    public bool EhFinal(string estado) => _finais.Contains(estado);

    private bool Aceita(Configuracao c, int tamanhoEntrada) =>
        c.Posicao == tamanhoEntrada &&
        (Criterio == CriterioAceitacao.EstadoFinal ? EhFinal(c.Estado) : c.Pilha.EstaVazia);

    public ResultadoPilha Executar(string? entrada, int limiteConfiguracoes = LimiteConfiguracoesPadrao)
    {
        if (limiteConfiguracoes < 1 || limiteConfiguracoes > LimiteConfiguracoesMaximo)
            throw new ArgumentException($"O limite de configurações deve estar entre 1 e {LimiteConfiguracoesMaximo}.");

        var cadeia = Simbolos.NormalizarVazia(entrada);
        int n = cadeia.Length;

        var pilhaInicial = SimboloInicialPilha is { } z ? new Pilha(z.ToString()) : Pilha.Vazia;
        var nos = new List<No> { new(new Configuracao(EstadoInicial, 0, pilhaInicial), -1, null, 0) };
        var visitadas = new HashSet<Configuracao> { nos[0].Config };
        var fila = new Queue<int>();
        fila.Enqueue(0);

        int aceito = -1, melhor = 0, descartadas = 0;
        bool interrompido = false;

        while (fila.Count > 0)
        {
            int i = fila.Dequeue();
            var no = nos[i];
            var c = no.Config;

            if (Aceita(c, n)) { aceito = i; break; }

            // Para mostrar algo útil na rejeição: o ramo que mais leu (e, empatando, o mais longo).
            var m = nos[melhor];
            if (c.Posicao > m.Config.Posicao || (c.Posicao == m.Config.Posicao && no.Profundidade > m.Profundidade))
                melhor = i;

            if (!_porEstado.TryGetValue(c.Estado, out var candidatas)) continue;

            foreach (var t in candidatas)
            {
                // Leitura: λ não consome; senão o símbolo atual precisa ser o da transição.
                int posicao = c.Posicao;
                if (t.Simbolo is { } a)
                {
                    if (posicao >= n || cadeia[posicao] != a) continue;
                    posicao++;
                }

                // Pilha: desempilha o topo exigido (se houver) e empilha a cadeia da transição.
                var pilha = c.Pilha;
                if (t.Desempilha is { } x)
                {
                    if (pilha.Topo != x) continue;
                    pilha = pilha.Desempilhar();
                }
                pilha = pilha.Empilhar(t.Empilha);

                if (pilha.Altura > AlturaMaximaPilha) { descartadas++; continue; }

                var nova = new Configuracao(t.Destino, posicao, pilha);
                if (!visitadas.Add(nova)) continue;

                if (nos.Count >= limiteConfiguracoes) { interrompido = true; break; }
                nos.Add(new No(nova, i, t, no.Profundidade + 1));
                fila.Enqueue(nos.Count - 1);
            }
            if (interrompido) break;
        }

        bool aceita = aceito >= 0;
        var passos = Caminho(nos, aceita ? aceito : melhor, cadeia);
        var fim = nos[aceita ? aceito : melhor].Config;

        string criterioTexto = Criterio == CriterioAceitacao.EstadoFinal ? "estado final" : "pilha vazia";
        string mensagem;
        if (aceita)
            mensagem = $"Aceita por {criterioTexto}: a entrada foi toda lida e o autômato terminou em {fim.Estado} com pilha {fim.Pilha}.";
        else if (interrompido)
            mensagem = $"Limite de {limiteConfiguracoes} configurações atingido antes de encontrar uma computação que aceite.";
        else
        {
            var porque = fim.Posicao < n
                ? $"parou em {fim.Estado} após ler {fim.Posicao} de {n} símbolo(s), com pilha {fim.Pilha}: não há transição para '{cadeia[fim.Posicao]}' com {Simbolos.Exibir(fim.Pilha.Topo?.ToString() ?? "")} no topo"
                : Criterio == CriterioAceitacao.EstadoFinal
                    ? $"leu toda a entrada, mas terminou em {fim.Estado}, que não é final"
                    : $"leu toda a entrada, mas a pilha terminou com {fim.Pilha}";
            mensagem = Deterministico
                ? $"Rejeitada: o autômato {porque}."
                : $"Rejeitada: nenhuma das {nos.Count} configurações exploradas aceita. O ramo que mais avançou {porque}.";
        }
        if (descartadas > 0)
            mensagem += $" {descartadas} ramo(s) descartado(s) por passar de {AlturaMaximaPilha} símbolos na pilha.";

        return new ResultadoPilha(aceita, Deterministico, Criterio, mensagem, cadeia, nos.Count,
            fim.Estado, fim.Posicao, fim.Pilha.Conteudo, passos);
    }

    /// <summary>Reconstrói a sequência de passos seguindo os ponteiros para o nó pai.</summary>
    private static List<PassoPilha> Caminho(List<No> nos, int indice, string cadeia)
    {
        var passos = new List<PassoPilha>();
        for (int i = indice; nos[i].Pai >= 0; i = nos[i].Pai)
        {
            var antes = nos[nos[i].Pai].Config;
            var depois = nos[i].Config;
            var t = nos[i].Transicao!;
            passos.Add(new PassoPilha(0, antes.Estado, antes.Posicao, cadeia[antes.Posicao..], t.SimboloTexto,
                antes.Pilha.Conteudo, t, t.Operacao, depois.Estado, depois.Pilha.Conteudo));
        }
        passos.Reverse();
        return passos.Select((p, k) => p with { Numero = k + 1 }).ToList();
    }

    /// <summary>Configuração instantânea: estado, quantos símbolos já foram lidos e a pilha.</summary>
    private sealed record Configuracao(string Estado, int Posicao, Pilha Pilha);

    /// <summary>Nó da árvore de computação: configuração, pai e a transição que levou até ela.</summary>
    private sealed record No(Configuracao Config, int Pai, TransicaoPilha? Transicao, int Profundidade);
}
