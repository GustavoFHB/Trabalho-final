using SimuladorFormal.Compartilhado;

namespace SimuladorFormal.Automatos;

public sealed record RequisicaoAP(string? Cadeia);
public sealed record PassoAP(int Posicao, string? Simbolo, string Estado, string Acao, string Pilha);
public sealed record ResultadoAP(bool Aceita, string Mensagem, PassoAP[] Passos);

public static class AutomatoPilha
{
    // ⊥ é o fundo lógico, exibido separadamente; somente marcadores A são empilhados.
    public static ResultadoAP Executar(RequisicaoAP req)
    {
        var cadeia = Simbolos.NormalizarVazia(req.Cadeia);
        if (cadeia.Length > 1000) throw new ArgumentException("Use uma cadeia de até 1000 símbolos.");
        var pilha = new Stack<char>();
        var passos = new List<PassoAP> { new(0, null, "q0", "Configuração inicial", "") };
        var estado = "q0";
        for (int i = 0; i < cadeia.Length; i++)
        {
            char c = cadeia[i];
            string acao;
            if (c == 'a' && estado == "q0")
            {
                pilha.Push('A');
                acao = "Empilha A";
            }
            else if (c == 'b' && pilha.TryPop(out _))
            {
                estado = "q1";
                acao = "Desempilha A";
            }
            else
            {
                passos.Add(new(i + 1, c.ToString(), "qr", "Sem transição válida", new string(pilha.ToArray())));
                return new(false, $"Não há transição para '{c}' na posição {i + 1}.", passos.ToArray());
            }
            passos.Add(new(i + 1, c.ToString(), estado, acao, new string(pilha.ToArray())));
        }

        bool aceita = estado == "q1" && pilha.Count == 0;
        var motivo = aceita ? "Cada a foi pareado com um b, na ordem correta." :
            cadeia.Length == 0 ? "A linguagem exige pelo menos um a e um b." : "Ainda há marcadores A sem par na pilha.";
        passos.Add(new(cadeia.Length, null, aceita ? "qf" : "qr",
            aceita ? "Aceita ao ler o fim da cadeia" : "Rejeita ao ler o fim da cadeia", new string(pilha.ToArray())));
        return new(aceita, motivo, passos.ToArray());
    }
}
