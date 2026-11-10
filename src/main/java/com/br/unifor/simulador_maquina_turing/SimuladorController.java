package com.br.unifor.simulador_maquina_turing;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.Map;

@RestController
@RequestMapping("/api")
public class SimuladorController {

    private static final int LIMITE_PASSOS_MAXIMO = 10000;

    @PostMapping("/simular")
    public ResultadoSimulacao simular(@RequestBody SimulacaoRequest req) {
        String estadoInicial = obrigatorio(req.estadoInicial(), "Estado inicial");
        String estadoFinal = obrigatorio(req.estadoFinal(), "Estado final");
        String entrada = req.entrada() == null ? "" : req.entrada().trim();

        if (req.transicoes() == null || req.transicoes().isEmpty()) {
            throw new IllegalArgumentException("Cadastre pelo menos uma transição.");
        }

        MaquinaTuring mt = new MaquinaTuring(estadoInicial, estadoFinal, new Fita(entrada));

        int linha = 1;
        for (SimulacaoRequest.TransicaoRequest t : req.transicoes()) {
            String prefixo = "Transição " + linha + ": ";
            mt.adicionarTransicao(new Transicao(
                    obrigatorio(t.estadoAtual(), prefixo + "estado atual"),
                    simbolo(t.simboloLido(), prefixo + "símbolo lido"),
                    obrigatorio(t.novoEstado(), prefixo + "novo estado"),
                    simbolo(t.simboloEscrito(), prefixo + "símbolo escrito"),
                    direcao(t.direcao(), prefixo + "direção")));
            linha++;
        }

        if (req.limitePassos() != null) {
            if (req.limitePassos() < 1 || req.limitePassos() > LIMITE_PASSOS_MAXIMO) {
                throw new IllegalArgumentException("O limite de passos deve estar entre 1 e " + LIMITE_PASSOS_MAXIMO + ".");
            }
            mt.setLimitePassos(req.limitePassos());
        }

        return mt.executar();
    }

    @ExceptionHandler(IllegalArgumentException.class)
    public ResponseEntity<Map<String, String>> erroValidacao(IllegalArgumentException e) {
        return ResponseEntity.badRequest().body(Map.of("erro", e.getMessage()));
    }

    private String obrigatorio(String valor, String campo) {
        if (valor == null || valor.isBlank()) {
            throw new IllegalArgumentException(campo + " é obrigatório.");
        }
        return valor.trim();
    }

    private char simbolo(String valor, String campo) {
        if (valor == null || valor.trim().length() != 1) {
            throw new IllegalArgumentException(campo + " deve ter exatamente 1 caractere.");
        }
        return valor.trim().charAt(0);
    }

    // Aceita R/L/S e também D (direita) / E (esquerda).
    private char direcao(String valor, String campo) {
        char d = Character.toUpperCase(simbolo(valor, campo));
        return switch (d) {
            case 'R', 'D' -> 'R';
            case 'L', 'E' -> 'L';
            case 'S' -> 'S';
            default -> throw new IllegalArgumentException(campo + " deve ser R (direita), L (esquerda) ou S (parado).");
        };
    }
}
