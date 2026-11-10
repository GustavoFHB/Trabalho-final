package com.br.unifor.simulador_maquina_turing;

import java.util.List;

public record ResultadoSimulacao(boolean aceita, String motivo, String estadoFinal, String fitaFinal,
                                 int cabecoteFinal, List<Passo> passos) {
}
