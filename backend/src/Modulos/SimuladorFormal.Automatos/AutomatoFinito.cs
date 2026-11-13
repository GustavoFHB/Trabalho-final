using SimuladorFormal.Compartilhado;

namespace SimuladorFormal.Automatos;

public sealed record RegraAF(string Origem, string Simbolo, string Destino);
public sealed record DefinicaoAF(string Tipo, string[] Estados, string[] Alfabeto,
    string EstadoInicial, string[] EstadosFinais, RegraAF[] Transicoes);
public sealed record RequisicaoAF(DefinicaoAF Automato, string? Cadeia);
public sealed record PassoAF(int Posicao, string? Simbolo, string[] EstadosAtivos, RegraAF[] Transicoes);
public sealed record ResultadoAF(bool Aceita, string Mensagem, PassoAF[] Passos);

public static class AutomatoFinito
{
    public static ResultadoAF Executar(RequisicaoAF req)
    {
        var a = req.Automato ?? throw new ArgumentException("Informe o autômato.");
        bool afd = a.Tipo?.Trim().Equals("AFD", StringComparison.OrdinalIgnoreCase) == true;
        if (!afd && a.Tipo?.Trim().Equals("AFN", StringComparison.OrdinalIgnoreCase) != true)
            throw new ArgumentException("O tipo deve ser AFD ou AFN.");
        if (a.Estados is null || a.Alfabeto is null || a.EstadosFinais is null || a.Transicoes is null)
            throw new ArgumentException("Informe estados, alfabeto, estadosFinais e transicoes.");
        if (a.Estados.Length is < 1 or > 50 || a.Transicoes.Length > 500 || a.Alfabeto.Length > 50)
            throw new ArgumentException("Use de 1 a 50 estados, até 50 símbolos e até 500 transições.");
        if (a.Estados.Any(e => string.IsNullOrWhiteSpace(e) || e.Length > 32) ||
            a.Estados.Distinct().Count() != a.Estados.Length)
            throw new ArgumentException("Use nomes de estados distintos e não vazios, de até 32 caracteres.");
        var estados = a.Estados.ToHashSet();
        if (!estados.Contains(a.EstadoInicial) || a.EstadosFinais.Any(e => !estados.Contains(e)))
            throw new ArgumentException("O estado inicial e os finais devem estar na lista de estados.");
        if (a.Alfabeto.Any(s => s is null || s.Length != 1 || char.IsSurrogate(s[0]) || Simbolos.RepresentaVazia(s)) ||
            a.Alfabeto.Distinct().Count() != a.Alfabeto.Length)
            throw new ArgumentException("O alfabeto deve ter símbolos distintos de um caractere, sem λ, ε ou &.");
        var alfabeto = a.Alfabeto.Select(s => s[0]).ToHashSet();
        var finais = a.EstadosFinais.ToHashSet();
        var delta = new Dictionary<(string, string), List<RegraAF>>();
        var regras = new HashSet<RegraAF>();
        foreach (var t in a.Transicoes)
        {
            if (t is null || !estados.Contains(t.Origem) || !estados.Contains(t.Destino) ||
                string.IsNullOrWhiteSpace(t.Simbolo))
                throw new ArgumentException("Cada transição deve informar origem, símbolo e destino válidos.");
            var simbolo = Simbolos.RepresentaVazia(t.Simbolo) ? Simbolos.Lambda : t.Simbolo;
            if (simbolo == Simbolos.Lambda ? afd : simbolo.Length != 1 || !alfabeto.Contains(simbolo[0]))
                throw new ArgumentException("Símbolo de transição fora do alfabeto ou transição vazia em AFD.");
            var regra = t with { Simbolo = simbolo };
            if (!regras.Add(regra)) throw new ArgumentException("Há uma transição duplicada.");
            var chave = (regra.Origem, simbolo);
            if (!delta.TryGetValue(chave, out var lista)) delta[chave] = lista = new();
            if (afd && lista.Count > 0)
                throw new ArgumentException("No AFD, cada par (estado, símbolo) tem um único destino.");
            lista.Add(regra);
        }
        // AFD parcial: a ausência de transição equivale a um estado poço não desenhado.
        var cadeia = Simbolos.NormalizarVazia(req.Cadeia);
        if (cadeia.Length > 1000) throw new ArgumentException("Use uma cadeia de até 1000 símbolos.");
        var ativos = new HashSet<string> { a.EstadoInicial };
        var usadas = new List<RegraAF>();
        FecharLambda(ativos, usadas);
        var passos = new List<PassoAF> { new(0, null, ativos.ToArray(), usadas.ToArray()) };
        for (int i = 0; i < cadeia.Length; i++)
        {
            var c = cadeia[i];
            usadas.Clear();
            var destinos = new HashSet<string>();
            if (alfabeto.Contains(c))
            {
                foreach (var estado in ativos)
                    if (delta.TryGetValue((estado, c.ToString()), out var saidas))
                        foreach (var t in saidas)
                        {
                            usadas.Add(t);
                            destinos.Add(t.Destino);
                        }
                FecharLambda(destinos, usadas);
            }
            passos.Add(new(i + 1, c.ToString(), destinos.ToArray(), usadas.ToArray()));
            if (destinos.Count == 0)
            {
                var motivo = alfabeto.Contains(c) ? "Nenhuma transição disponível" : "Símbolo fora do alfabeto";
                return new(false, $"{motivo}: '{c}' na posição {i + 1}.", passos.ToArray());
            }
            ativos = destinos;
        }
        bool aceita = ativos.Any(finais.Contains);
        return new(aceita, aceita ? "A cadeia foi consumida e há um estado final ativo." :
            "A cadeia foi consumida, mas nenhum estado ativo é final.", passos.ToArray());

        void FecharLambda(HashSet<string> conjunto, List<RegraAF> percorridas)
        {
            if (afd) return;
            var fila = new Queue<string>(conjunto);
            while (fila.TryDequeue(out var estado))
                if (delta.TryGetValue((estado, Simbolos.Lambda), out var saidas))
                    foreach (var t in saidas)
                    {
                        percorridas.Add(t);
                        if (conjunto.Add(t.Destino)) fila.Enqueue(t.Destino);
                    }
        }
    }
}
