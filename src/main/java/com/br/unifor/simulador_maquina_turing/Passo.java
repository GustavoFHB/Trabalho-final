package com.br.unifor.simulador_maquina_turing;


public record Passo(int numero, String estado, char simboloLido, String fita, int cabecote, Transicao transicao) {
}
