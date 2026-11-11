using SimuladorFormal.Gramaticas.Derivacao;

namespace SimuladorFormal.Gramaticas.Contratos;

public sealed record DerivacaoResponse(GramaticaResumo Gramatica, ResultadoDerivacao Resultado);
