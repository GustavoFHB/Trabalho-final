using SimuladorFormal.Compartilhado;

namespace SimuladorFormal.AutomatosPilha.Dominio;

/// <summary>
/// Pilha imutável do AP. O conteúdo é guardado como texto com o <b>topo à esquerda</b>
/// (convenção dos livros: em "AAZ", A está no topo e Z no fundo).
/// Por ser imutável, cada ramo da computação não determinística tem a sua própria pilha.
/// </summary>
public sealed record Pilha(string Conteudo)
{
    public static Pilha Vazia { get; } = new("");

    public bool EstaVazia => Conteudo.Length == 0;
    public int Altura => Conteudo.Length;
    public char? Topo => EstaVazia ? null : Conteudo[0];

    /// <summary>Remove o topo. Pré-condição: a pilha não está vazia.</summary>
    public Pilha Desempilhar() =>
        EstaVazia ? throw new InvalidOperationException("Pilha vazia.") : new Pilha(Conteudo[1..]);

    /// <summary>Empilha uma cadeia; o primeiro símbolo dela fica no topo ("AZ" deixa A no topo).</summary>
    public Pilha Empilhar(string simbolos) => simbolos.Length == 0 ? this : new Pilha(simbolos + Conteudo);

    public override string ToString() => Simbolos.Exibir(Conteudo);
}
