# Simulador de Linguagens Formais e Modelos Computacionais

Projeto prático de **Aspectos Teóricos da Computação** (AV3). Implementa todos os módulos do enunciado em **C# (.NET 8)**, com interface web em **React** (ponto extra):

| Módulo | O que faz |
|---|---|
| **1. Linguagens formais** | Define Σ e L (conjunto finito ou expressão regular), valida w ∈ Σ*, calcula \|w\|, w·v, wᴿ e enumera Σ* e Σ+ até um tamanho k. |
| **2. Autômato finito** | AFD, AFN e AFN-λ carregados por **JSON** (editor ou arquivo .json). Mostra o caminho `q0 → q1 → q2`, o passo a passo (conjunto de estados no AFN) e o diagrama animado. |
| **3. Gramática livre de contexto** | Define V, T, P e S; reconhece a cadeia com o algoritmo de **Earley**, mostra a **derivação mais à esquerda passo a passo** e a **árvore de derivação**. |
| **4. Autômato com pilha** | Pilha com empilhar/desempilhar, aceitação por estado final ou pilha vazia, APs determinísticos e não determinísticos, montados na tabela de transições (ex.: **aⁿbⁿ, n ≥ 1**). Mostra leitura, estado e conteúdo da pilha a cada passo. |
| **5. Máquina de Turing** | **Opção A** (reconhece w#w) **e opção B** (soma unária), além de máquinas personalizadas. Fita, cabeçote, diagrama de estados e histórico animados. |
| **6. Complexidade** | Aba com a análise O / Ω / Θ (melhor, médio e pior caso) de cada algoritmo — detalhes em [docs/COMPLEXIDADE.md](docs/COMPLEXIDADE.md). |

O módulo 5 é uma evolução de um simulador Java/Spring anterior da dupla: as classes `Fita`, `Transicao`, `Passo`, `ResultadoSimulacao`, `SimulacaoRequest` e `MaquinaTuring` foram portadas para C#, com a função δ trocada de lista (busca linear a cada passo) para dicionário indexado por (estado, símbolo).

## Estrutura de pastas

O backend é dividido **por módulo**: cada módulo é um projeto próprio com seu domínio, serviços, contratos (JSON) e controller. A API só hospeda os módulos. Detalhes em [docs/ARQUITETURA.md](docs/ARQUITETURA.md).

```
backend/
├── SimuladorFormal.sln
├── src/
│   ├── SimuladorFormal.Api/                  # host: Program.cs registra os módulos
│   │   └── Infra/                            # erros de validação → HTTP 400
│   ├── SimuladorFormal.Compartilhado/        # λ e leitura de conjuntos de símbolos
│   └── Modulos/
│       ├── SimuladorFormal.Linguagens/       # Módulo 1
│       │   ├── Dominio/                      # Alfabeto, Linguagem, LinguagemFinita, LinguagemRegular, FabricaLinguagem
│       │   ├── Operacoes/                    # ValidadorCadeia, OperacoesCadeia, GeradorFecho
│       │   ├── Resultados/                   # ValidacaoCadeia, Fecho
│       │   ├── Contratos/                    # requisição e resposta da API
│       │   ├── Servicos/                     # AnaliseLinguagemService
│       │   └── Api/                          # LinguagensController
│       ├── SimuladorFormal.AutomatosFinitos/ # Módulo 2
│       │   ├── Dominio/                      # TransicaoAf, AutomatoFinito (AFD/AFN, fecho-λ, subconjuntos)
│       │   ├── Execucao/                     # PassoAutomato, EstadoCaminho, ResultadoAutomato
│       │   ├── Leitura/                      # LeitorAutomato (JSON → domínio)
│       │   ├── Exemplos/                     # CatalogoAutomatos
│       │   ├── Contratos/  Servicos/  Api/
│       ├── SimuladorFormal.Gramaticas/       # Módulo 3
│       │   ├── Dominio/                      # Producao, GramaticaLivreContexto, ValidadorGramatica
│       │   ├── Leitura/                      # LeitorGramatica ("S -> aSb | λ")
│       │   ├── Analise/                      # VariaveisAnulaveis
│       │   ├── Earley/                       # ItemEarley, ConjuntoEarley, ReconhecedorEarley…
│       │   ├── Derivacao/                    # NoArvore, ConstrutorArvore, GeradorDerivacaoEsquerda…
│       │   ├── Contratos/  Servicos/  Api/
│       ├── SimuladorFormal.AutomatosPilha/   # Módulo 4
│       │   ├── Dominio/                      # Pilha, TransicaoPilha, CriterioAceitacao, AutomatoPilha
│       │   ├── Execucao/                     # PassoPilha, ResultadoPilha
│       │   ├── Leitura/                      # LeitorTransicaoPilha
│       │   ├── Exemplos/                     # CatalogoAutomatosPilha (aⁿbⁿ, wwᴿ, parênteses)
│       │   ├── Contratos/  Servicos/  Api/
│       └── SimuladorFormal.Turing/           # Módulo 5
│           ├── Dominio/                      # Fita, Transicao, Direcao, FuncaoTransicao, MaquinaTuring
│           ├── Execucao/                     # Passo, ResultadoSimulacao
│           ├── Leitura/                      # LeitorTransicao
│           ├── Exemplos/                     # MaquinaSomaUnaria, MaquinaWCercaW, CatalogoMaquinas
│           ├── Contratos/  Servicos/  Api/
└── tests/SimuladorFormal.Tests/              # xUnit (85 testes), uma pasta por módulo
frontend/                                     # React + Vite + TypeScript
└── src/
    ├── modulos/                       # uma tela por módulo
    └── componentes/                   # fitas, pilha, diagrama de estados, árvore, controles
docs/                                  # complexidade e arquitetura (UML)
exemplos/                              # requisições JSON prontas
```

## Como executar

Pré-requisitos: **.NET SDK 8+** e **Node.js 20+**.

### Opção 1 — tudo pela API (mais simples)

```bash
cd frontend
npm install
npm run build              # gera o front em backend/src/SimuladorFormal.Api/wwwroot

cd ../backend/src/SimuladorFormal.Api
dotnet run                 # abre em http://localhost:5080
```

### Opção 2 — desenvolvimento (recarregamento automático)

```bash
# terminal 1
cd backend/src/SimuladorFormal.Api
dotnet run                 # API em http://localhost:5080

# terminal 2
cd frontend
npm install
npm run dev                # front em http://localhost:5173 (encaminha /api para a 5080)
```

### Testes

```bash
cd backend
dotnet test
```

## Exemplos de entrada e saída

### Módulo 1 — Linguagens formais (exemplo do enunciado)

Entrada: Σ = `{a, b}`, w = `abba`

```
Cadeia válida para Σ = {a, b}
Tamanho |w| = 4
w·v (v = ab) = abbaab    |w·v| = 6
Σ* até tamanho 2 = { λ, a, b, aa, ab, ba, bb }
Σ+ até tamanho 2 = { a, b, aa, ab, ba, bb }
```

Via API:

```bash
curl -X POST http://localhost:5080/api/linguagens/analisar \
  -H "Content-Type: application/json" -d @exemplos/linguagem.json
```

### Módulo 2 — Autômato finito (exemplo do enunciado)

Entrada: o JSON abaixo (q0 –a→ q1, q1 –b→ q2) e a cadeia `ab`

```json
{
  "estados": ["q0", "q1", "q2"],
  "alfabeto": ["a", "b"],
  "estadoInicial": "q0",
  "estadosFinais": ["q2"],
  "transicoes": [
    { "origem": "q0", "simbolo": "a", "destino": "q1" },
    { "origem": "q1", "simbolo": "b", "destino": "q2" }
  ]
}
```

```
q0 → q1 → q2
ACEITA
```

Com `abb` a saída é `REJEITADA — Não existe δ(q2, b)`. Para um AFN, repita a mesma origem e símbolo com destinos diferentes; para uma transição λ use `"simbolo": "λ"`. No AFN "termina em ab" com `aab`, cada passo mostra o conjunto de estados ativos (ex.: `{q0, q1}`) e o caminho de aceitação é `q0 → q0 → q1 → q2`.

```bash
curl -X POST http://localhost:5080/api/automatos/simular \
  -H "Content-Type: application/json" -d @exemplos/automato-finito.json
```

### Módulo 3 — Gramática (exemplo do enunciado)

Entrada: `S -> aSb | λ`, cadeia `aaabbb`

```
Aceita
S ⇒ aSb ⇒ aaSbb ⇒ aaaSbbb ⇒ aaabbb

        S
      / | \
     a  S  b
      / | \
     a  S  b
      / | \
     a  S  b
        |
        λ
```

Cadeias como `aab` ou `abab` são rejeitadas com o ponto em que a análise falhou.

```bash
curl -X POST http://localhost:5080/api/gramaticas/derivar \
  -H "Content-Type: application/json" -d @exemplos/gramatica-anbn.json
```

### Módulo 4 — Autômato com pilha (L = { aⁿbⁿ | n ≥ 1 })

Entrada: `aaabbb` (Z começa no fundo da pilha; o topo é o símbolo mais à esquerda)

```
#  Estado  Leitura  Pilha   Transição                 Operação
1  q0      a        Z       δ(q0, a, Z) = (q0, AZ)    empilha A sobre Z
2  q0      a        AZ      δ(q0, a, A) = (q0, AA)    empilha A sobre A
3  q0      a        AAZ     δ(q0, a, A) = (q0, AA)    empilha A sobre A
4  q0      b        AAAZ    δ(q0, b, A) = (q1, λ)     desempilha A
5  q1      b        AAZ     δ(q1, b, A) = (q1, λ)     desempilha A
6  q1      b        AZ      δ(q1, b, A) = (q1, λ)     desempilha A
7  q1      λ        Z       δ(q1, λ, Z) = (q2, Z)     consulta o topo Z (sem alterar)
ACEITA por estado final: terminou em q2 com pilha Z
```

`aabbb`, `abab` e λ são rejeitadas. Os palíndromos wwᴿ mostram um AP **não determinístico**: o simulador explora todos os ramos em largura e exibe o que aceitou.

```bash
curl -X POST http://localhost:5080/api/pilha/simular \
  -H "Content-Type: application/json" -d @exemplos/automato-pilha-anbn.json
```

### Módulo 5 — Máquina de Turing

**Opção B (soma unária)** — entrada `000100` (3 + 2):

```
Passo 1: q0, lê 0  →  δ(q0, 0) = (q0, 0, R)
...
Passo 4: q0, lê 1  →  δ(q0, 1) = (q1, 0, R)
...
Passo 8: q2, lê 0  →  δ(q2, 0) = (qf, B, S)
ACEITA — fita final: 00000   (8 passos)
```

**Opção A (w#w)** — `ab#ab` e `0110#0110` são aceitas; `ab#ba`, `abb#ab` são rejeitadas.

```bash
curl -X POST http://localhost:5080/api/turing/simular \
  -H "Content-Type: application/json" -d @exemplos/turing-soma-unaria.json
```

## Endpoints

| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/linguagens/analisar` | Validação, tamanho, concatenação, Σ*, Σ+ e pertinência em L |
| GET | `/api/automatos/exemplos` | Autômatos finitos prontos (em JSON) |
| POST | `/api/automatos/simular` | Executa um AFD/AFN carregado por JSON |
| POST | `/api/gramaticas/derivar` | Reconhecimento, derivação passo a passo e árvore |
| GET | `/api/pilha/exemplos` | Autômatos com pilha prontos |
| POST | `/api/pilha/simular` | Executa um AP e devolve leitura, estado e pilha a cada passo |
| GET | `/api/turing/exemplos` | Máquinas prontas (soma unária e w#w) |
| POST | `/api/turing/simular` | Executa uma máquina e devolve todos os passos |

Erros de configuração (símbolo fora do alfabeto, gramática mal escrita, transição duplicada…) retornam **HTTP 400** com `{ "erro": "mensagem" }`.

## Convenções

- Símbolos são caracteres únicos. `λ`, `ε` ou `&` representam a cadeia vazia.
- No AP a pilha é escrita com o topo à esquerda (`AAZ`: A no topo); ao empilhar `AZ`, A fica sobre Z. Campo vazio ou λ = não lê / não desempilha / não empilha.
- O AP não determinístico explora no máximo 10 000 configurações (configurável até 200 000) e 500 símbolos na pilha.
- Na Máquina de Turing o branco é `B`; direções `R`, `L` e `S` (parado).
- A MT para quando chega a um estado final (aceita), quando não há transição (rejeita) ou ao atingir o limite de passos.
