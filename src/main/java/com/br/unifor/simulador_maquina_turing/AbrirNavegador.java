package com.br.unifor.simulador_maquina_turing;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.context.event.ApplicationReadyEvent;
import org.springframework.context.event.EventListener;
import org.springframework.core.env.Environment;
import org.springframework.stereotype.Component;

import java.io.IOException;

// Abre a página do simulador no navegador padrão do sistema quando a aplicação termina de subir.
@Component
public class AbrirNavegador {

    private final Environment environment;

    @Value("${simulador.abrir-navegador:true}")
    private boolean abrirNavegador;

    public AbrirNavegador(Environment environment) {
        this.environment = environment;
    }

    @EventListener(ApplicationReadyEvent.class)
    public void aoIniciar() {
        String porta = environment.getProperty("local.server.port");
        if (!abrirNavegador || porta == null) {
            return;
        }

        String url = "http://localhost:" + porta;
        String os = System.getProperty("os.name").toLowerCase();

        ProcessBuilder comando;
        if (os.contains("win")) {
            comando = new ProcessBuilder("rundll32", "url.dll,FileProtocolHandler", url);
        } else if (os.contains("mac")) {
            comando = new ProcessBuilder("open", url);
        } else {
            comando = new ProcessBuilder("xdg-open", url);
        }

        try {
            comando.start();
            System.out.println("Simulador aberto no navegador: " + url);
        } catch (IOException e) {
            System.out.println("Não foi possível abrir o navegador. Acesse manualmente: " + url);
        }
    }
}
