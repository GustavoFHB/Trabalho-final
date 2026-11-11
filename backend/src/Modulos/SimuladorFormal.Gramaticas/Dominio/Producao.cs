using SimuladorFormal.Compartilhado;

namespace SimuladorFormal.Gramaticas.Dominio;

/// <summary>Produção A → α. Lado direito vazio representa A → λ.</summary>
public sealed record Producao(char Esquerda, string Direita)
{
    public bool Vazia => Direita.Length == 0;

    public override string ToString() => $"{Esquerda} → {Simbolos.Exibir(Direita)}";
}
