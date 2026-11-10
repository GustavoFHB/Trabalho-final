package com.br.unifor.simulador_maquina_turing;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

public class MaquinaTuring {

    private static final int LIMITE_PASSOS_PADRAO = 1000;

    private String estadoAtual;
    private String estadoFinal;
    private Fita fita;
    private List<Transicao> transicaos = new ArrayList<>();
    private int limitePassos = LIMITE_PASSOS_PADRAO;

    public MaquinaTuring(String estadoAtual, String estadoFinal, Fita fita) {
        this.estadoAtual = estadoAtual;
        this.estadoFinal = estadoFinal;
        this.fita = fita;
    }

    public void adicionarTransicao(Transicao transicao){
        transicaos.add(transicao);
    }

    public void setLimitePassos(int limitePassos) {
        this.limitePassos = limitePassos;
    }

    public ResultadoSimulacao executar(){
        int passo =0;
        List<Passo> passos = new ArrayList<>();
        String motivo = null;

        while (!estadoAtual.equals(estadoFinal)){
            if (passo >= limitePassos){
                motivo = "Limite de " + limitePassos + " passos atingido (possível loop infinito).";
                System.out.println(motivo);
                break;
            }

            char simbolo = fita.ler();

            Optional<Transicao> transicaoEntrada = transicaos.stream()
                    .filter(t->t.getEstadoAtual().equals(estadoAtual)
                            && t.getSimboloLido()==simbolo)
                    .findFirst();

            if (transicaoEntrada.isEmpty()){
                motivo = "Máquina parou sem transição válida para (" + estadoAtual + ", " + simbolo + ").";
                System.out.println("Maquina parou sem a transição valida! ");
                break;
            }

            Transicao t = transicaoEntrada.get();

            System.out.println("Passo:  "+passo);
            System.out.println(" Estado atual: " +estadoAtual);
            System.out.println(" Simbolo lido: "+ simbolo);
            System.out.println(" Fita : "+fita.conteudo());
            System.out.println(" cabeçote : "+ fita.getCabecote());
            System.out.println("=========================================");

            passos.add(new Passo(passo, estadoAtual, simbolo, fita.conteudo(), fita.getCabecote(), t));

            fita.escrever(t.getSimboloEscrito());
            fita.mover(t.getMovimento());
            estadoAtual = t.getNovoEstado();

            passo++;
        }

        boolean aceita = estadoAtual.equals(estadoFinal);
        if (aceita){
            motivo = "Estado final " + estadoFinal + " alcançado.";
        }

        System.out.println(" Maquina finalizada!");
        System.out.println(" Estado final: "+estadoAtual);
        System.out.println(" FIta final: "+fita.conteudo());

        return new ResultadoSimulacao(aceita, motivo, estadoAtual, fita.conteudo(), fita.getCabecote(), passos);
    }
}
