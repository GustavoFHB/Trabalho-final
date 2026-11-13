# Arquitetura

O backend é organizado **por módulo** (fatias verticais). Cada módulo do enunciado é um projeto .NET independente que contém tudo o que precisa: domínio, operações, contratos JSON, serviço e controller. A API é apenas o host que junta os módulos.

| Projeto | Papel |
|---|---|
| `SimuladorFormal.Api` | Host ASP.NET Core: configura JSON, CORS, arquivos estáticos e chama `AddModuloX()` de cada módulo. |
| `SimuladorFormal.Compartilhado` | Convenções usadas por mais de um módulo: λ (`Simbolos`) e leitura de conjuntos como `{a, b}` (`LeitorSimbolos`). |
| `SimuladorFormal.Linguagens` | Módulo 1 — Linguagens formais. |
| `SimuladorFormal.AutomatosFinitos` | Módulo 2 — Autômatos finitos (AFD, AFN, AFN-λ) carregados por JSON. |
| `SimuladorFormal.Gramaticas` | Módulo 3 — Gramáticas livres de contexto. |
| `SimuladorFormal.AutomatosPilha` | Módulo 4 — Autômato com pilha. |
| `SimuladorFormal.Turing` | Módulo 5 — Máquina de Turing. |
| `SimuladorFormal.Tests` | Testes xUnit, uma pasta por módulo. |

```mermaid
flowchart LR
    U[Estudante] --> F[Front React]
    F -- JSON / HTTP --> API[SimuladorFormal.Api]
    API --> L[Módulo Linguagens]
    API --> A[Módulo Autômatos Finitos]
    API --> G[Módulo Gramáticas]
    API --> P[Módulo Autômatos com Pilha]
    API --> T[Módulo Turing]
    L --> C[Compartilhado]
    A --> C
    G --> C
    P --> C
    T --> C
```

Os módulos não dependem uns dos outros: dá para remover ou trocar um sem mexer nos demais.

## Camadas dentro de cada módulo

Todos os módulos seguem as mesmas pastas, e o fluxo de uma requisição é sempre o mesmo:

```
Api/Controller  →  Servicos/Service  →  Leitura (texto → domínio)  →  Dominio / algoritmos  →  Contratos (resposta JSON)
```

| Pasta | Responsabilidade |
|---|---|
| `Dominio/` | Os modelos teóricos (Alfabeto, Gramática, Máquina de Turing…) e suas regras de validação. |
| `Leitura/` | Converte o texto ou JSON recebido (ex.: `S -> aSb \| λ`, `R/L/S`, o JSON do autômato) em objetos do domínio. |
| `Operacoes/`, `Earley/`, `Derivacao/`, `Analise/`, `Execucao/` | Os algoritmos do módulo e seus resultados. |
| `Exemplos/` | Autômatos e máquinas prontos (módulos 2, 4 e 5). |
| `Contratos/` | Requisições e respostas da API (formato do JSON). |
| `Servicos/` | Orquestra um caso de uso de ponta a ponta. |
| `Api/` | Controller HTTP (fino: só chama o serviço). |
| `ModuloX.cs` | Registra o serviço e o controller do módulo na aplicação. |

## Diagramas de classes (UML)

### Módulo 1 — Linguagens formais

```mermaid
classDiagram
    direction LR
    class Alfabeto {
        +IReadOnlyList~char~ Simbolos
        +Parse(texto) Alfabeto$
        +Contem(simbolo) bool
    }
    class Linguagem {
        <<abstract>>
        +Alfabeto Sigma
        +string Definicao
        +TipoLinguagem Tipo
        +Pertence(w) bool
        +Listar(k) List~string~
        #ContemCadeiaDeSigmaEstrela(w) bool
    }
    class LinguagemFinita
    class LinguagemRegular
    class FabricaLinguagem {
        <<static>>
        +Criar(sigma, tipo, definicao) Linguagem
    }
    class ValidadorCadeia {
        <<static>>
        +Validar(sigma, w) ValidacaoCadeia
    }
    class OperacoesCadeia {
        <<static>>
        +Tamanho(w) int
        +Concatenar(w, v) string
        +Reverso(w) string
        +Potencia(w, n) string
    }
    class GeradorFecho {
        <<static>>
        +FechoKleene(sigma, k) Fecho
        +FechoPositivo(sigma, k) Fecho
    }
    class AnaliseLinguagemService {
        +Analisar(req) AnaliseLinguagemResponse
    }
    class LinguagensController
    Linguagem <|-- LinguagemFinita
    Linguagem <|-- LinguagemRegular
    Linguagem --> Alfabeto
    FabricaLinguagem ..> Linguagem
    ValidadorCadeia ..> Alfabeto
    GeradorFecho ..> Alfabeto
    LinguagensController --> AnaliseLinguagemService
    AnaliseLinguagemService ..> FabricaLinguagem
    AnaliseLinguagemService ..> ValidadorCadeia
    AnaliseLinguagemService ..> OperacoesCadeia
    AnaliseLinguagemService ..> GeradorFecho
```

### Módulo 2 — Autômato finito

```mermaid
classDiagram
    direction LR
    class TransicaoAf {
        +string Origem
        +char? Simbolo
        +string Destino
        +bool Vazia
    }
    class AutomatoFinito {
        +Estados
        +Alfabeto
        +string EstadoInicial
        +EstadosFinais
        +bool Deterministico
        +Executar(cadeia) ResultadoAutomato
        -FechoLambda(conjunto)
        -Reconstruir(niveis, estado)
    }
    class PassoAutomato {
        +int Numero
        +char Simbolo
        +EstadosAntes
        +Transicoes
        +EstadosDepois
    }
    class EstadoCaminho {
        +string Estado
        +string Simbolo
    }
    class ResultadoAutomato {
        +bool Aceita
        +string Mensagem
        +List~PassoAutomato~ Passos
        +List~EstadoCaminho~ Caminho
        +string CaminhoTexto
    }
    class AutomatoDto {
        +Estados
        +Alfabeto
        +EstadoInicial
        +EstadosFinais
        +Transicoes
    }
    class LeitorAutomato {
        <<static>>
        +Ler(dto) AutomatoFinito
        +ParaDto(automato) AutomatoDto
    }
    class CatalogoAutomatos {
        <<static>>
        +Todos
    }
    class SimuladorAutomatoService {
        +ListarExemplos()
        +Simular(req) ResultadoAutomato
    }
    class AutomatosFinitosController
    AutomatoFinito "1" *-- "*" TransicaoAf
    AutomatoFinito ..> ResultadoAutomato
    ResultadoAutomato "1" *-- "*" PassoAutomato
    ResultadoAutomato "1" *-- "*" EstadoCaminho
    LeitorAutomato ..> AutomatoDto
    LeitorAutomato ..> AutomatoFinito
    CatalogoAutomatos ..> AutomatoFinito
    AutomatosFinitosController --> SimuladorAutomatoService
    SimuladorAutomatoService ..> LeitorAutomato
    SimuladorAutomatoService ..> CatalogoAutomatos
```

### Módulo 3 — Gramática livre de contexto

```mermaid
classDiagram
    direction LR
    class Producao {
        +char Esquerda
        +string Direita
        +bool Vazia
    }
    class GramaticaLivreContexto {
        +Variaveis
        +Terminais
        +List~Producao~ Producoes
        +char SimboloInicial
        +IndicesProducoesDe(variavel)
    }
    class ValidadorGramatica {
        <<static>>
        +Validar(g)
    }
    class LeitorGramatica {
        <<static>>
        +Ler(producoes, V, T, S) GramaticaLivreContexto
    }
    class VariaveisAnulaveis {
        +Contem(variavel) bool
        +ProducaoVazia(variavel) int
        +Calcular(g) VariaveisAnulaveis$
    }
    class ItemEarley {
        +int Producao
        +int Ponto
        +int Inicio
        +OrigemItem Tipo
        +ItemEarley Anterior
        +ItemEarley Filho
        +Avancar(tipo) ItemEarley
    }
    class ConjuntoEarley {
        +Adicionar(item) bool
        +AguardandoVariavel(v)
    }
    class ReconhecedorEarley {
        +Reconhecer(cadeia) ResultadoReconhecimento
    }
    class ConstrutorArvore {
        +Construir(itemFinal) NoArvore
    }
    class GeradorDerivacaoEsquerda {
        <<static>>
        +Gerar(raiz) List~PassoDerivacao~
    }
    class NoArvore {
        +string Simbolo
        +List~NoArvore~ Filhos
    }
    class DerivacaoService {
        +Executar(req) DerivacaoResponse
        +Derivar(g, cadeia) ResultadoDerivacao
    }
    class GramaticasController
    GramaticaLivreContexto "1" *-- "*" Producao
    GramaticaLivreContexto ..> ValidadorGramatica
    LeitorGramatica ..> GramaticaLivreContexto
    ReconhecedorEarley --> GramaticaLivreContexto
    ReconhecedorEarley --> VariaveisAnulaveis
    ReconhecedorEarley ..> ConjuntoEarley
    ConjuntoEarley "1" o-- "*" ItemEarley
    ConstrutorArvore ..> ItemEarley
    ConstrutorArvore ..> NoArvore
    GeradorDerivacaoEsquerda ..> NoArvore
    GramaticasController --> DerivacaoService
    DerivacaoService ..> LeitorGramatica
    DerivacaoService ..> ReconhecedorEarley
    DerivacaoService ..> ConstrutorArvore
    DerivacaoService ..> GeradorDerivacaoEsquerda
```

### Módulo 4 — Autômato com pilha

```mermaid
classDiagram
    direction LR
    class Pilha {
        +string Conteudo
        +char? Topo
        +bool EstaVazia
        +int Altura
        +Empilhar(simbolos) Pilha
        +Desempilhar() Pilha
    }
    class TransicaoPilha {
        +string Origem
        +char? Simbolo
        +char? Desempilha
        +string Destino
        +string Empilha
        +string Operacao
    }
    class CriterioAceitacao {
        <<enumeration>>
        EstadoFinal
        PilhaVazia
    }
    class AutomatoPilha {
        +string EstadoInicial
        +EstadosFinais
        +char? SimboloInicialPilha
        +CriterioAceitacao Criterio
        +bool Deterministico
        +Executar(cadeia, limite) ResultadoPilha
    }
    class PassoPilha {
        +string Estado
        +string Leitura
        +string Pilha
        +TransicaoPilha Transicao
        +string Operacao
        +string PilhaDepois
    }
    class ResultadoPilha {
        +bool Aceita
        +string Mensagem
        +int ConfiguracoesExploradas
        +List~PassoPilha~ Passos
    }
    class LeitorTransicaoPilha {
        <<static>>
        +Ler(dto, linha) TransicaoPilha
    }
    class CatalogoAutomatosPilha {
        <<static>>
        +AnBn
        +Palindromos
        +Parenteses
    }
    class SimuladorPilhaService {
        +ListarExemplos()
        +Simular(req) ResultadoPilha
    }
    class AutomatosPilhaController
    AutomatoPilha "1" *-- "*" TransicaoPilha
    AutomatoPilha --> CriterioAceitacao
    AutomatoPilha ..> Pilha : uma por configuração
    AutomatoPilha ..> ResultadoPilha
    ResultadoPilha "1" *-- "*" PassoPilha
    CatalogoAutomatosPilha ..> AutomatoPilha
    AutomatosPilhaController --> SimuladorPilhaService
    SimuladorPilhaService ..> LeitorTransicaoPilha
    SimuladorPilhaService ..> AutomatoPilha
```

### Módulo 5 — Máquina de Turing

```mermaid
classDiagram
    direction LR
    class Fita {
        +char Branco$
        +int Cabecote
        +Ler() char
        +Escrever(c)
        +Mover(direcao)
        +Conteudo() string
    }
    class Transicao {
        +string EstadoAtual
        +char SimboloLido
        +string NovoEstado
        +char SimboloEscrito
        +Direcao Movimento
    }
    class Direcao {
        <<enumeration>>
        Esquerda
        Direita
        Parado
    }
    class FuncaoTransicao {
        +TentarObter(estado, simbolo, out t) bool
        +Todas
    }
    class MaquinaTuring {
        +string EstadoInicial
        +EstadosFinais
        +FuncaoTransicao Delta
        +Executar(entrada, limite) ResultadoSimulacao
    }
    class Passo {
        +int Numero
        +string Estado
        +char SimboloLido
        +string Fita
        +int Cabecote
        +Transicao Transicao
    }
    class ResultadoSimulacao {
        +bool Aceita
        +string Motivo
        +string Saida
        +List~Passo~ Passos
    }
    class LeitorTransicao {
        <<static>>
        +Ler(dto, linha) Transicao
        +LerDirecao(valor) Direcao
    }
    class DefinicaoMaquina {
        +Criar() MaquinaTuring
    }
    class MaquinaSomaUnaria {
        <<static>>
        +Definicao
    }
    class MaquinaWCercaW {
        <<static>>
        +Definicao
    }
    class SimuladorTuringService {
        +ListarExemplos()
        +Simular(req) ResultadoSimulacao
    }
    class TuringController
    Transicao --> Direcao
    FuncaoTransicao "1" *-- "*" Transicao
    MaquinaTuring *-- FuncaoTransicao
    MaquinaTuring ..> Fita : cria por execução
    MaquinaTuring ..> ResultadoSimulacao
    ResultadoSimulacao "1" *-- "*" Passo
    MaquinaSomaUnaria ..> DefinicaoMaquina
    MaquinaWCercaW ..> DefinicaoMaquina
    DefinicaoMaquina ..> MaquinaTuring
    TuringController --> SimuladorTuringService
    SimuladorTuringService ..> LeitorTransicao
    SimuladorTuringService ..> MaquinaTuring
```

## Casos de uso

```mermaid
flowchart LR
    E((Estudante))
    E --> UC1[Definir alfabeto e linguagem]
    E --> UC2[Validar cadeia e calcular tamanho / concatenação]
    E --> UC3[Listar Σ* e Σ+]
    E --> UC9[Carregar autômato finito por JSON]
    E --> UC10[Executar cadeia no AFD/AFN e ver o caminho]
    E --> UC4[Definir gramática]
    E --> UC5[Derivar cadeia passo a passo e ver a árvore]
    E --> UC11[Escolher ou montar um autômato com pilha]
    E --> UC12[Acompanhar leitura, estado e pilha]
    E --> UC6[Escolher ou montar uma Máquina de Turing]
    E --> UC7[Executar e acompanhar fita e estados]
    E --> UC8[Consultar análise de complexidade]
```

## Estruturas de dados

| Onde | Estrutura | Por quê |
|---|---|---|
| `Alfabeto` | `HashSet<char>` + lista ordenada | Pertinência O(1); a ordem define a ordem lexicográfica de Σ*. |
| `LinguagemFinita` | `HashSet<string>` | Pertinência em tempo proporcional a \|w\|. |
| `AutomatoFinito` | `Dictionary<(estado, símbolo?), List<destino>>` | δ de AFN em O(1) por (estado, símbolo); a chave com símbolo nulo guarda as transições λ. |
| `AutomatoFinito.Executar` | lista de níveis `Dictionary<estado, origem>` | O conjunto de estados ativos de cada passo, com o predecessor de cada um para reconstruir o caminho. |
| `Pilha` | `string` imutável, topo à esquerda | Cada ramo não determinístico tem sua pilha sem cópias defensivas; igualdade por valor para o `HashSet` de configurações. |
| `AutomatoPilha.Executar` | fila (BFS) + `HashSet<Configuracao>` + lista de nós com ponteiro para o pai | Explora todos os ramos sem repetir configurações e reconstrói a computação que aceitou. |
| `ConjuntoEarley` | `List<ItemEarley>`, `HashSet` de chaves `(produção, ponto, início)` e `Dictionary<variável, itens que a esperam>` | Evita itens duplicados e torna a conclusão proporcional só aos itens relevantes. |
| `ItemEarley` | ponteiros para o item anterior e para o filho | Permite reconstruir a árvore sem nova busca. |
| `Fita` | `List<char>` que cresce nas duas pontas | Simula a fita infinita. |
| `FuncaoTransicao` | `Dictionary<(string, char), Transicao>` | δ em O(1) e detecção de não determinismo na construção. |

### Módulos 2 e 4 — Autômatos

`AutomatosController` recebe o JSON e delega a `AutomatoFinito` ou
`AutomatoPilha`. Ambos devolvem veredito e configurações para o React.
O AF indexa as regras por (origem, símbolo). O AFD avança um estado por vez;
o AFN guarda um conjunto de estados ativos e aplica fecho λ com fila e
conjunto de visitados, evitando repetir ciclos. O AP usa `Stack<char>` e
registra a ação e o conteúdo da pilha depois de cada símbolo.

```mermaid
classDiagram
    AutomatosController --> AutomatoFinito
    AutomatosController --> AutomatoPilha
    AutomatoFinito --> DefinicaoAF
    AutomatoFinito --> ResultadoAF
    AutomatoPilha --> ResultadoAP
    ResultadoAF *-- PassoAF
    ResultadoAP *-- PassoAP
```

Caso de uso AF: importar configuração, informar cadeia, executar e
percorrer estados ativos. Caso de uso AP: informar cadeia, executar e
percorrer operações e configurações da pilha.
