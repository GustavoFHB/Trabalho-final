using SimuladorFormal.AutomatosPilha.Dominio;

namespace SimuladorFormal.AutomatosPilha.Execucao;

/// <summary>
/// Um movimento do AP: configuração antes (estado, posição, pilha), o que foi lido,
/// a transição aplicada e a configuração depois. Pilhas com o topo à esquerda.
/// </summary>
/// <param name="Restante">Parte da entrada ainda não lida antes do movimento.</param>
/// <param name="Leitura">Símbolo consumido, ou λ se a transição não leu a entrada.</param>
public sealed record PassoPilha(
    int Numero,
    string Estado,
    int Posicao,
    string Restante,
    string Leitura,
    string Pilha,
    TransicaoPilha Transicao,
    string Operacao,
    string NovoEstado,
    string PilhaDepois);
