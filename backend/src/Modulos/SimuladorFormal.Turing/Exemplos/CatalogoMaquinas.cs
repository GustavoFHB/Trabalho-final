namespace SimuladorFormal.Turing.Exemplos;

/// <summary>Máquinas prontas do Módulo 5.</summary>
public static class CatalogoMaquinas
{
    public static IReadOnlyList<DefinicaoMaquina> Todas { get; } = new[]
    {
        MaquinaSomaUnaria.Definicao,
        MaquinaWCercaW.Definicao
    };
}
