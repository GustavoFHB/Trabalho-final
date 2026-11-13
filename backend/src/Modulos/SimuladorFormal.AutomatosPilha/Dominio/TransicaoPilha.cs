using SimuladorFormal.Compartilhado;

namespace SimuladorFormal.AutomatosPilha.Dominio;

/// <summary>
/// δ(origem, símbolo, desempilha) ∋ (destino, empilha).
/// <see cref="Simbolo"/> nulo = não lê a entrada (λ); <see cref="Desempilha"/> nulo = não consulta a pilha (λ);
/// <see cref="Empilha"/> vazio = não empilha nada.
/// </summary>
public sealed record TransicaoPilha(string Origem, char? Simbolo, char? Desempilha, string Destino, string Empilha)
{
    public string SimboloTexto => Simbolo?.ToString() ?? Simbolos.Lambda;
    public string DesempilhaTexto => Desempilha?.ToString() ?? Simbolos.Lambda;
    public string EmpilhaTexto => Simbolos.Exibir(Empilha);

    /// <summary>Descrição da operação na pilha, para o passo a passo.</summary>
    public string Operacao
    {
        get
        {
            if (Desempilha is null)
                return Empilha.Length == 0 ? "pilha inalterada" : $"empilha {Empilha}";
            if (Empilha == Desempilha.ToString())
                return $"consulta o topo {Desempilha} (sem alterar)";
            if (Empilha.Length > 1 && Empilha[^1] == Desempilha)
                return $"empilha {Empilha[..^1]} sobre {Desempilha}";
            return Empilha.Length == 0
                ? $"desempilha {Desempilha}"
                : $"desempilha {Desempilha} e empilha {Empilha}";
        }
    }

    public override string ToString() =>
        $"δ({Origem}, {SimboloTexto}, {DesempilhaTexto}) = ({Destino}, {EmpilhaTexto})";
}
