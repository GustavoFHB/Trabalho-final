using SimuladorFormal.Turing.Dominio;

namespace SimuladorFormal.Turing.Execucao;

/// <summary>Configuração instantânea da máquina antes de aplicar uma transição.</summary>
public sealed record Passo(int Numero, string Estado, char SimboloLido, string Fita, int Cabecote, Transicao Transicao);
