namespace SimuladorFormal.Linguagens.Dominio;

/// <summary>Cria a linguagem certa a partir do tipo escolhido pelo usuário.</summary>
public static class FabricaLinguagem
{
    public static Linguagem Criar(Alfabeto sigma, string? tipo, string? definicao) =>
        (tipo ?? "finita").Trim().ToLowerInvariant() switch
        {
            "regular" => new LinguagemRegular(sigma, definicao),
            "finita" => new LinguagemFinita(sigma, definicao),
            _ => throw new ArgumentException("O tipo da linguagem deve ser \"finita\" ou \"regular\".")
        };
}
