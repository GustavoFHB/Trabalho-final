using SimuladorFormal.AutomatosFinitos.Execucao;
using SimuladorFormal.Compartilhado;

namespace SimuladorFormal.AutomatosFinitos.Dominio;

/// <summary>
/// Autômato finito M = (Q, Σ, δ, q0, F). O mesmo modelo representa AFD e AFN (com ou sem λ):
/// δ: Q × (Σ ∪ {λ}) → P(Q). É determinístico quando não há transições λ e cada (q, a) tem no máximo um destino.
/// </summary>
public sealed class AutomatoFinito
{
    private readonly Dictionary<(string Estado, char? Simbolo), List<string>> _delta = new();
    private readonly HashSet<string> _estados;
    private readonly HashSet<char> _alfabeto;
    private readonly HashSet<string> _finais;

    public IReadOnlyList<string> Estados { get; }
    public IReadOnlyList<char> Alfabeto { get; }
    public string EstadoInicial { get; }
    public IReadOnlySet<string> EstadosFinais => _finais;
    public IReadOnlyList<TransicaoAf> Transicoes { get; }

    /// <summary>Sem transições λ e no máximo um destino por (estado, símbolo).</summary>
    public bool Deterministico { get; }

    public bool PossuiTransicoesVazias { get; }

    /// <param name="estados">Q. Se vazio, é inferido de q0, F e das transições.</param>
    /// <param name="alfabeto">Σ. Se vazio, é inferido dos símbolos das transições.</param>
    public AutomatoFinito(
        IEnumerable<string> estados,
        IEnumerable<char> alfabeto,
        string estadoInicial,
        IEnumerable<string> estadosFinais,
        IEnumerable<TransicaoAf> transicoes)
    {
        if (string.IsNullOrWhiteSpace(estadoInicial))
            throw new ArgumentException("Estado inicial é obrigatório.");
        EstadoInicial = estadoInicial.Trim();

        var listaTransicoes = transicoes.Distinct().ToList();
        var finais = estadosFinais.Select(e => e.Trim()).Where(e => e.Length > 0).Distinct().ToList();

        var listaEstados = estados.Select(e => e.Trim()).Where(e => e.Length > 0).Distinct().ToList();
        bool estadosInformados = listaEstados.Count > 0;
        if (!estadosInformados)
            listaEstados = new[] { EstadoInicial }
                .Concat(listaTransicoes.SelectMany(t => new[] { t.Origem, t.Destino }))
                .Concat(finais)
                .Distinct().ToList();
        _estados = listaEstados.ToHashSet();

        if (estadosInformados)
        {
            ExigirEstado(EstadoInicial, "O estado inicial");
            foreach (var f in finais) ExigirEstado(f, "O estado final");
            foreach (var t in listaTransicoes)
            {
                ExigirEstado(t.Origem, $"A origem da transição {t}");
                ExigirEstado(t.Destino, $"O destino da transição {t}");
            }
        }

        var listaAlfabeto = alfabeto.Distinct().ToList();
        if (listaAlfabeto.Count == 0)
            listaAlfabeto = listaTransicoes.Where(t => !t.Vazia).Select(t => t.Simbolo!.Value).Distinct().ToList();
        else
            foreach (var t in listaTransicoes.Where(t => !t.Vazia))
                if (!listaAlfabeto.Contains(t.Simbolo!.Value))
                    throw new ArgumentException($"A transição {t} usa o símbolo '{t.Simbolo}', que não está em Σ = {{{string.Join(", ", listaAlfabeto)}}}.");
        _alfabeto = listaAlfabeto.ToHashSet();

        foreach (var t in listaTransicoes)
        {
            if (!_delta.TryGetValue((t.Origem, t.Simbolo), out var destinos))
                _delta[(t.Origem, t.Simbolo)] = destinos = new List<string>();
            destinos.Add(t.Destino);
        }

        Estados = listaEstados;
        Alfabeto = listaAlfabeto;
        _finais = finais.ToHashSet();
        Transicoes = listaTransicoes;
        PossuiTransicoesVazias = listaTransicoes.Any(t => t.Vazia);
        Deterministico = !PossuiTransicoesVazias && _delta.Values.All(d => d.Count == 1);
    }

    private void ExigirEstado(string estado, string descricao)
    {
        if (!_estados.Contains(estado))
            throw new ArgumentException($"{descricao} usa \"{estado}\", que não está em Q = {{{string.Join(", ", _estados)}}}.");
    }

    public bool EhFinal(string estado) => _finais.Contains(estado);

    /// <summary>
    /// Executa a cadeia simulando o AFN pelo método dos subconjuntos: mantém o conjunto de estados
    /// em que o autômato pode estar. Num AFD esse conjunto tem sempre um único estado.
    /// Também guarda, para cada estado alcançado, de onde ele veio — assim é possível
    /// mostrar o caminho q0 → … → qf que levou à aceitação.
    /// </summary>
    public ResultadoAutomato Executar(string? entrada)
    {
        var cadeia = Simbolos.NormalizarVazia(entrada);

        var niveis = new List<Dictionary<string, Origem>>();

        var inicial = new Dictionary<string, Origem> { [EstadoInicial] = Origem.Inicio };
        var transicoesIniciais = FechoLambda(inicial);
        niveis.Add(inicial);

        var passos = new List<PassoAutomato>();
        string? motivoParada = null;

        for (int i = 0; i < cadeia.Length; i++)
        {
            char a = cadeia[i];
            var atual = niveis[^1];

            if (!_alfabeto.Contains(a))
            {
                motivoParada = $"O símbolo '{a}' (posição {i}) não pertence a Σ = {{{string.Join(", ", Alfabeto)}}}.";
                break;
            }

            var proximo = new Dictionary<string, Origem>();
            var usadas = new List<TransicaoAf>();
            foreach (var q in atual.Keys)
                if (_delta.TryGetValue((q, a), out var destinos))
                    foreach (var p in destinos)
                    {
                        usadas.Add(new TransicaoAf(q, a, p));
                        proximo.TryAdd(p, new Origem(q, a, MesmoNivel: false));
                    }

            usadas.AddRange(FechoLambda(proximo));
            passos.Add(new PassoAutomato(i + 1, i, a, Ordenar(atual.Keys), usadas, Ordenar(proximo.Keys)));

            if (proximo.Count == 0)
            {
                motivoParada = Deterministico
                    ? $"Não existe δ({atual.Keys.Single()}, {a}): o autômato trava e a cadeia é rejeitada."
                    : $"Nenhum dos estados {{{string.Join(", ", Ordenar(atual.Keys))}}} tem transição com '{a}': a cadeia é rejeitada.";
                break;
            }
            niveis.Add(proximo);
        }

        var ultimo = niveis[^1];
        bool leuTudo = motivoParada is null;
        var finaisAlcancados = leuTudo ? Ordenar(ultimo.Keys.Where(EhFinal)) : new List<string>();
        bool aceita = finaisAlcancados.Count > 0;

        string mensagem = motivoParada ?? (aceita
            ? $"A cadeia foi toda lida e o autômato terminou no estado final {string.Join(", ", finaisAlcancados)}."
            : $"A cadeia foi toda lida, mas nenhum estado alcançado ({{{string.Join(", ", Ordenar(ultimo.Keys))}}}) é final.");

        List<EstadoCaminho>? caminho = null;
        if (aceita) caminho = Reconstruir(niveis, finaisAlcancados[0]);
        else if (ultimo.Count == 1) caminho = Reconstruir(niveis, ultimo.Keys.Single());

        return new ResultadoAutomato(
            aceita,
            Deterministico,
            mensagem,
            cadeia,
            passos.Count,
            Ordenar(niveis[0].Keys),
            transicoesIniciais,
            passos,
            finaisAlcancados,
            caminho,
            caminho is null ? null : string.Join(" → ", caminho.Select(c => c.Estado)));
    }

    /// <summary>Acrescenta ao conjunto todos os estados alcançáveis só por transições λ (busca em largura).</summary>
    private List<TransicaoAf> FechoLambda(Dictionary<string, Origem> conjunto)
    {
        var usadas = new List<TransicaoAf>();
        if (!PossuiTransicoesVazias) return usadas;

        var fila = new Queue<string>(conjunto.Keys);
        while (fila.Count > 0)
        {
            var q = fila.Dequeue();
            if (!_delta.TryGetValue((q, null), out var destinos)) continue;
            foreach (var p in destinos)
            {
                usadas.Add(new TransicaoAf(q, null, p));
                if (conjunto.TryAdd(p, new Origem(q, null, MesmoNivel: true)))
                    fila.Enqueue(p);
            }
        }
        return usadas;
    }

    /// <summary>Segue os predecessores de trás para a frente, do estado no último nível até q0.</summary>
    private static List<EstadoCaminho> Reconstruir(List<Dictionary<string, Origem>> niveis, string estado)
    {
        var caminho = new List<EstadoCaminho>();
        int nivel = niveis.Count - 1;
        while (true)
        {
            var origem = niveis[nivel][estado];
            if (origem.Estado is null)
            {
                caminho.Add(new EstadoCaminho(estado, null));
                break;
            }
            caminho.Add(new EstadoCaminho(estado, origem.Simbolo?.ToString() ?? Simbolos.Lambda));
            estado = origem.Estado;
            if (!origem.MesmoNivel) nivel--;
        }
        caminho.Reverse();
        return caminho;
    }

    private List<string> Ordenar(IEnumerable<string> estados)
    {
        var conjunto = estados.ToHashSet();
        return Estados.Where(conjunto.Contains).ToList();
    }

    /// <summary>De onde um estado foi alcançado: estado anterior e símbolo lido (nulo = λ).</summary>
    private sealed record Origem(string? Estado, char? Simbolo, bool MesmoNivel)
    {
        public static readonly Origem Inicio = new(null, null, true);
    }
}
