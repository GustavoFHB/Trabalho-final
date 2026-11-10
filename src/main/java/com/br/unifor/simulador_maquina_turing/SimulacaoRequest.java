package com.br.unifor.simulador_maquina_turing;

import java.util.List;

public record SimulacaoRequest(String entrada, String estadoInicial, String estadoFinal,
                               List<TransicaoRequest> transicoes, Integer limitePassos) {

    public record TransicaoRequest(String estadoAtual, String simboloLido, String novoEstado,
                                   String simboloEscrito, String direcao) {
    }
}
